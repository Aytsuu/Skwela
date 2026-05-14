using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using esecai.Application.DTOs;
using esecai.Infrastructure.Services;

namespace esecai.Tests;

public class AIClientServiceTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private sealed record SectionChunk(string Title, string Content, string AnswerKeyContent);

    private readonly ITestOutputHelper _output;

    public AIClientServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task GenerateAsync_WithExtractedAssessment_ReturnsText()
    {
        // Arrange
        // Load environment variables from .env file
        try
        {
            DotNetEnv.Env.TraversePath().Load();
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Warning: .env parsing failed, continuing with existing environment vars. {ex.Message}");
        }
        
        var options = Options.Create(new AIOptions
        {
            GeminiApiKey = Environment.GetEnvironmentVariable("AI__GeminiApiKey")!,
            GeminiModel = Environment.GetEnvironmentVariable("AI__GeminiModel")!,
            OllamaApiKey = Environment.GetEnvironmentVariable("AI__OllamaApiKey")!,
            OllamaApiEndpoint = Environment.GetEnvironmentVariable("AI__OllamaApiEndpoint")!,
            OllamaBaseUrl = Environment.GetEnvironmentVariable("AI__OllamaBaseUrl")!,
            OllamaModel = Environment.GetEnvironmentVariable("AI__OllamaModel")!,
            PrimaryProvider = "Ollama",
            EnableFallback = false
        });
        
        var loggerMock = new Mock<ILogger<AIClientService>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient());
        
        // This is more of an integration test if we use the real client.
        // For a unit test, normally the GenAI Client would be mocked or abstracted.
        // Assuming we are doing an integration test structure here that uses the real library
        var service = new AIClientService(options, loggerMock.Object, httpClientFactoryMock.Object);

        string filePath = "TestOutput/extracted_assessment.txt";
        Assert.True(File.Exists(filePath), $"Test input file not found: {filePath}");
        
        var extractedAssessment = await File.ReadAllTextAsync(filePath);
        var sections = BuildSectionChunksWithAnswerKeys(extractedAssessment);
        Assert.NotEmpty(sections);

        // Pointing directly to the project root directory rather than the bin folder
        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
        string outputDirectory = Path.Combine(projectRoot, "TestOutput");

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        foreach (var existingJson in Directory.GetFiles(outputDirectory, "*.json"))
        {
            File.Delete(existingJson);
        }

        var merged = new List<JsonElement>(sections.Count);

        // Act: process each section independently, then merge.
        for (var i = 0; i < sections.Count; i++)
        {
            var section = sections[i];
            try
            {
                var sectionPrompt = BuildSectionPrompt(section);
                var sectionResult = await service.GenerateAsync(sectionPrompt, maxTokens: 8192);

                Assert.False(string.IsNullOrWhiteSpace(sectionResult));

                var jsonPayload = ExtractJsonPayload(sectionResult!);
                using var doc = JsonDocument.Parse(jsonPayload);
                var sectionJson = doc.RootElement.Clone();
                merged.Add(sectionJson);

                var outputFilePath = Path.Combine(
                    outputDirectory,
                    $"{SanitizeFileName(section.Title)}.json");

                await File.WriteAllTextAsync(outputFilePath, JsonSerializer.Serialize(sectionJson, JsonOptions));
                _output.WriteLine($"Section file saved: {outputFilePath}");
            }
            catch (Exception ex)
            {
                if (IsOllamaUnauthorized(ex))
                {
                    _output.WriteLine($"Ollama unauthorized for section '{section.Title}', retrying with Gemini.");
                    var retried = await TryGenerateSectionWithGeminiFallbackAsync(
                        section,
                        outputDirectory,
                        loggerMock.Object,
                        httpClientFactoryMock.Object,
                        merged);

                    if (retried)
                    {
                        continue;
                    }
                }

                var fallbackJson = BuildFallbackSectionJson(section);
                merged.Add(fallbackJson);

                var fallbackPath = Path.Combine(outputDirectory, $"{SanitizeFileName(section.Title)}.json");
                await File.WriteAllTextAsync(fallbackPath, JsonSerializer.Serialize(fallbackJson, JsonOptions));

                _output.WriteLine($"Section fallback used ({section.Title}): {ex.Message}");
            }
        }

        var mergedOutputPath = Path.Combine(outputDirectory, "structured_output_merged.json");
        await File.WriteAllTextAsync(mergedOutputPath, JsonSerializer.Serialize(merged, JsonOptions));
        _output.WriteLine($"Merged file saved: {mergedOutputPath}");

        // Assert
        Assert.True(merged.Count > 0, "No section outputs were generated.");
    }

    private static List<SectionChunk> BuildSectionChunksWithAnswerKeys(string fullContent)
    {
        // Match the actual answer-key header only (e.g., "ANSWER KEY" or "ANSWER KEY -- ...").
        var answerKeyStart = Regex.Match(
            fullContent,
            @"^\s*answer\b.*\bkey\b.*$",
            RegexOptions.Multiline | RegexOptions.IgnoreCase);

        var assessmentContent = answerKeyStart.Success
            ? fullContent[..answerKeyStart.Index]
            : fullContent;

        var answerKeyContent = answerKeyStart.Success
            ? fullContent[answerKeyStart.Index..]
            : string.Empty;

        var assessmentSections = SplitIntoSections(assessmentContent);
        if (assessmentSections.Count == 0)
        {
            return new List<SectionChunk>
            {
                new("section_01", fullContent.Trim(), string.Empty)
            };
        }

        var answerKeySections = SplitIntoSections(answerKeyContent);
        var answerKeyById = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var answerKeyByTopic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var answerKeySection in answerKeySections)
        {
            var sectionId = ExtractSectionId(answerKeySection.Title);
            if (string.IsNullOrWhiteSpace(sectionId))
            {
                continue;
            }

            answerKeyById[sectionId] = answerKeySection.Content;

            var sectionTopic = ExtractSectionTopic(answerKeySection.Title);
            if (!string.IsNullOrWhiteSpace(sectionTopic))
            {
                answerKeyByTopic[sectionTopic] = answerKeySection.Content;
            }
        }

        var result = new List<SectionChunk>(assessmentSections.Count);
        foreach (var assessmentSection in assessmentSections)
        {
            var sectionId = ExtractSectionId(assessmentSection.Title);
            answerKeyById.TryGetValue(sectionId, out var matchedAnswerKey);

            // Secondary matching: align by words after the section label (e.g., "Multiple Choice").
            if (string.IsNullOrWhiteSpace(matchedAnswerKey))
            {
                var sectionTopic = ExtractSectionTopic(assessmentSection.Title);
                if (!string.IsNullOrWhiteSpace(sectionTopic))
                {
                    answerKeyByTopic.TryGetValue(sectionTopic, out matchedAnswerKey);
                }
            }

            result.Add(new SectionChunk(
                assessmentSection.Title,
                assessmentSection.Content,
                matchedAnswerKey ?? string.Empty));
        }

        return result;
    }

    private static List<SectionChunk> SplitIntoSections(string content)
    {
        var headingRegex = new Regex(
            @"^(?<header>\s*(?:test|quiz|activity)\s*(?:[ivxlcdm\d]+)?\s*[:\-].*)$",
            RegexOptions.Multiline | RegexOptions.IgnoreCase);

        var matches = headingRegex.Matches(content);
        if (matches.Count == 0)
        {
            return new List<SectionChunk>();
        }

        var sections = new List<SectionChunk>(matches.Count);
        for (var i = 0; i < matches.Count; i++)
        {
            var start = matches[i].Index;
            var end = i + 1 < matches.Count ? matches[i + 1].Index : content.Length;
            var chunk = content[start..end].Trim();
            var title = matches[i].Groups["header"].Value.Trim();

            sections.Add(new SectionChunk(title, chunk, string.Empty));
        }

        return sections;
    }

    private static string ExtractSectionId(string title)
    {
        var match = Regex.Match(
            title,
            @"^\s*(test|quiz|activity)\s*(?<id>[ivxlcdm\d]+)",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return string.Empty;
        }

        return $"{match.Groups[1].Value.ToLowerInvariant()}-{match.Groups["id"].Value.ToLowerInvariant()}";
    }

    private static string ExtractSectionTopic(string title)
    {
        var colonIndex = title.IndexOf(':');
        if (colonIndex < 0 || colonIndex + 1 >= title.Length)
        {
            return string.Empty;
        }

        var topic = title[(colonIndex + 1)..].ToLowerInvariant();
        topic = Regex.Replace(topic, @"\([^\)]*\)", string.Empty);
        topic = Regex.Replace(topic, @"\s+", " ").Trim();
        return topic;
    }

    private static string BuildSectionPrompt(SectionChunk section)
    {
        return """
        You are processing one assessment section only.

        Return strictly valid JSON with this exact shape:
        {
            "sec": "section title",
        "inst": "section instructions as plain text",
        "qts": [
            {
            "number": 1,
            "question": "...",
            "choices": ["A", "B", "C", "D"],
            "answer": "..."
            }
        ]
        }

        Rules:
        - Keep only data from this section.
        - Use the answer key section as source of truth for correct answers.
        - Do not include markdown fences.
        - If a field is not found, use empty string or empty array.

        Section title:
        """ + section.Title + """

        Section text:
        """ + section.Content + """

        Answer key section:
        """ + section.AnswerKeyContent;
    }

    private static string ExtractJsonPayload(string raw)
    {
        var fenced = Regex.Match(raw, @"```(?:json)?\s*(?<json>[\s\S]*?)\s*```", RegexOptions.IgnoreCase);
        return fenced.Success ? fenced.Groups["json"].Value.Trim() : raw.Trim();
    }

    private static bool IsOllamaUnauthorized(Exception ex)
    {
        return ex.Message.Contains("Ollama request failed", StringComparison.OrdinalIgnoreCase)
            && ex.Message.Contains("401", StringComparison.OrdinalIgnoreCase);
    }

    private static JsonElement BuildFallbackSectionJson(SectionChunk section)
    {
        var fallback = new
        {
            sec = section.Title,
            inst = ExtractInstructions(section.Content),
            qts = Array.Empty<object>()
        };

        return JsonSerializer.SerializeToElement(fallback, JsonOptions);
    }

    private static string ExtractInstructions(string sectionContent)
    {
        var lines = sectionContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.Length == 0)
            {
                continue;
            }

            if (line.StartsWith("instruction", StringComparison.OrdinalIgnoreCase)
                || line.StartsWith("directions", StringComparison.OrdinalIgnoreCase)
                || line.StartsWith("circle", StringComparison.OrdinalIgnoreCase)
                || line.StartsWith("identify", StringComparison.OrdinalIgnoreCase)
                || line.StartsWith("complete", StringComparison.OrdinalIgnoreCase)
                || line.StartsWith("match", StringComparison.OrdinalIgnoreCase)
                || line.StartsWith("solve", StringComparison.OrdinalIgnoreCase)
                || line.StartsWith("answer", StringComparison.OrdinalIgnoreCase)
                || line.StartsWith("write", StringComparison.OrdinalIgnoreCase))
            {
                return line;
            }
        }

        return string.Empty;
    }

    private async Task<bool> TryGenerateSectionWithGeminiFallbackAsync(
        SectionChunk section,
        string outputDirectory,
        ILogger<AIClientService> logger,
        IHttpClientFactory httpClientFactory,
        List<JsonElement> merged)
    {
        var geminiApiKey = Environment.GetEnvironmentVariable("AI__GeminiApiKey") ?? string.Empty;
        var geminiModel = Environment.GetEnvironmentVariable("AI__GeminiModel") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(geminiApiKey) || string.IsNullOrWhiteSpace(geminiModel))
        {
            _output.WriteLine("Gemini fallback skipped: AI__GeminiApiKey or AI__GeminiModel is missing.");
            return false;
        }

        var geminiOptions = Options.Create(new AIOptions
        {
            GeminiApiKey = geminiApiKey,
            GeminiModel = geminiModel,
            PrimaryProvider = "Gemini",
            EnableFallback = false
        });

        var geminiService = new AIClientService(geminiOptions, logger, httpClientFactory);
        var sectionPrompt = BuildSectionPrompt(section);
        var sectionResult = await geminiService.GenerateAsync(sectionPrompt, maxTokens: 8192);
        var jsonPayload = ExtractJsonPayload(sectionResult);
        using var doc = JsonDocument.Parse(jsonPayload);
        var sectionJson = doc.RootElement.Clone();
        merged.Add(sectionJson);

        var outputFilePath = Path.Combine(outputDirectory, $"{SanitizeFileName(section.Title)}.json");
        await File.WriteAllTextAsync(outputFilePath, JsonSerializer.Serialize(sectionJson, JsonOptions));
        _output.WriteLine($"Section file saved via Gemini fallback: {outputFilePath}");

        return true;
    }

    private static string SanitizeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "section";
        }

        foreach (var c in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(c, '_');
        }

        return value.Replace(' ', '_').Trim('_');
    }
}
