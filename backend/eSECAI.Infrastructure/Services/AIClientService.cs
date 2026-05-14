using esecai.Application.Interfaces;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using esecai.Application.DTOs;
using esecai.Infrastructure.Services.Prompts;
using System.Text;
using System.Text.Json;

namespace esecai.Infrastructure.Services;

public class AIClientService : IAIClientService
{   
    private readonly Client? _client;
    private readonly AIOptions _options;
    private readonly string _geminiApiKey;
    private readonly string _geminiModel;
    private readonly ILogger<AIClientService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly bool _geminiAvailable;

    public AIClientService(
        IOptions<AIOptions> options,
        ILogger<AIClientService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _geminiApiKey = !string.IsNullOrWhiteSpace(_options.GeminiApiKey)
            ? _options.GeminiApiKey
            : _options.ApiKey;
        _geminiModel = !string.IsNullOrWhiteSpace(_options.GeminiModel)
            ? _options.GeminiModel
            : _options.Model;
        
        if (!string.IsNullOrWhiteSpace(_geminiApiKey))
        {
            _client = new Client(apiKey: _geminiApiKey);
            _geminiAvailable = true;
        }
        else
        {
            _geminiAvailable = false;
            _logger.LogWarning("Gemini API key is not configured. Gemini fallback will be unavailable.");
        }
    }

    // ── TEXT-ONLY GENERATION ──────────────────────────────────────────────────
 
    /// <summary>
    /// Sends a structured extraction prompt (plain text) to Gemini.
    /// Used after PdfPig extracts text from a digital PDF — no vision needed.
    /// </summary>
    public async Task<string> GenerateAsync(
        string prompt,
        int maxTokens = 2048,
        CancellationToken ct = default)
    {
        _logger.LogInformation  ("Provider: {provider}", _options.PrimaryProvider);
        var primaryProvider = (_options.PrimaryProvider ?? "ollama").Trim().ToLowerInvariant();
        if (primaryProvider == "gemini")
        {
            try
            {
                return await GenerateWithGeminiAsync(prompt, maxTokens, ct);
            }
            catch (Exception ex) when (_options.EnableFallback)
            {
                _logger.LogWarning(ex, "Gemini failed, trying Ollama fallback.");
                return await GenerateWithOllamaAsync(prompt, maxTokens, ct);
            }
        }

        try
        {
            return await GenerateWithOllamaAsync(prompt, maxTokens, ct);
        }
        catch (Exception ex) when (_options.EnableFallback && _geminiAvailable)
        {
            _logger.LogWarning(ex, "Ollama failed, trying Gemini fallback.");
            return await GenerateWithGeminiAsync(prompt, maxTokens, ct);
        }
    }
 
    // ── FILE UPLOAD + VISION GENERATION ──────────────────────────────────────
 
    /// <summary>
    /// Uploads a PDF/image to the Gemini Files API, then generates content using vision.
    /// Used for scanned exam papers where Gemini must visually interpret the document.
    ///
    /// SDK handles the resumable upload protocol automatically — no manual HTTP needed.
    /// Uploaded files are cached for 48h on Google's servers; consider reusing URIs
    /// if the same scan needs to be processed multiple times (e.g. re-grading).
    /// </summary>
    public async Task<string> GenerateWithFileAsync(
        Stream fileStream,
        string mimeType,
        string prompt,
        int maxTokens = 1500,
        CancellationToken ct = default)
    {
        if (!_geminiAvailable || _client is null)
        {
            throw new InvalidOperationException(
                "Gemini API key is required for file-based generation.");
        }

        _logger.LogDebug("Uploading file to Gemini Files API: {MimeType}, {Bytes} bytes",
            mimeType, fileStream.Length);
 
        // Step 1: Upload file via SDK — handles chunked upload internally
        var fileBytes    = await ReadStreamAsync(fileStream, ct);
        var uploadResponse = await _client.Files.UploadAsync(
            bytes:    fileBytes,
            fileName: $"exam_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
 
        var fileUri = uploadResponse.Uri;
        _logger.LogInformation("File uploaded to Gemini Files API: {Uri}", fileUri);
 
        // Step 2: Generate content referencing the uploaded file
        var config = new GenerateContentConfig
        {
            ResponseMimeType = "application/json",
            Temperature      = 0.0f,
            MaxOutputTokens  = maxTokens,
            SystemInstruction = new Content 
            { 
                Parts = new List<Part> { new Part { Text = "" } } 
            }
        };
 
        // Build multimodal content: [file_data, text_prompt]
        var contents = new Content
        {
            Parts = new List<Part>
            {
                new Part
                {
                    FileData = new FileData
                    {
                        MimeType = mimeType,
                        FileUri  = fileUri
                    }
                },
                new Part { Text = prompt }
            }
        };
 
        var response = await _client.Models.GenerateContentAsync(
            model:    _geminiModel,
            contents: contents,
            config:   config);
 
        return ExtractText(response);
    }

    private async Task<string> GenerateWithGeminiAsync(
        string prompt,
        int maxTokens,
        CancellationToken ct)
    {
        if (!_geminiAvailable || _client is null)
        {
            throw new InvalidOperationException("Gemini API key is not configured.");
        }

        _logger.LogDebug(
            "Gemini text generation: {Chars} chars, model: {Model}, maxTokens: {MaxTokens}",
            prompt.Length,
            _geminiModel,
            maxTokens);

        var config = new GenerateContentConfig
        {
            ResponseMimeType = "application/json",
            Temperature = 0.0f,
            MaxOutputTokens = maxTokens,
            SystemInstruction = new Content
            {
                Parts = new List<Part>
                {
                    new Part { Text = GeminiPrompts.AnswerKeyExtractionSystem }
                }
            }
        };

        var response = await _client.Models.GenerateContentAsync(
            model: _geminiModel,
            contents: prompt,
            config: config);

        return ExtractText(response);
    }

    private async Task<string> GenerateWithOllamaAsync(
        string prompt,
        int maxTokens,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.OllamaBaseUrl))
        {
            throw new InvalidOperationException("OllamaBaseUrl is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.OllamaModel))
        {
            throw new InvalidOperationException("OllamaModel is not configured.");
        }

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(Math.Max(10, _options.OllamaTimeoutSeconds));

        if (!string.IsNullOrWhiteSpace(_options.OllamaApiKey))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.OllamaApiKey);
        }

        var generateUrl = BuildOllamaGenerateUrl();
        _logger.LogInformation("Calling Ollama generate endpoint: {GenerateUrl}", generateUrl);

        var requestPayload = new
        {
            model = _options.OllamaModel,
            prompt,
            system = OllamaPrompts.AnswerKeyExtractionSystem,
            format = "json",
            stream = false,
            options = new
            {
                temperature = 0,
                num_predict = maxTokens
            }
        };

        var requestJson = JsonSerializer.Serialize(requestPayload);
        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        using var response = await client.PostAsync(generateUrl, content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Ollama request failed ({generateUrl}): {(int)response.StatusCode} {response.ReasonPhrase}. Body: {responseBody}");
        }

        using var doc = JsonDocument.Parse(responseBody);
        if (!doc.RootElement.TryGetProperty("response", out var responseTextNode))
        {
            throw new InvalidOperationException("Ollama response did not include a 'response' field.");
        }

        var text = responseTextNode.GetString();
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("Ollama returned empty response text.");
        }

        return text;
    }

    private string BuildOllamaGenerateUrl()
    {
        var endpoint = string.IsNullOrWhiteSpace(_options.OllamaApiEndpoint)
            ? "/api"
            : _options.OllamaApiEndpoint.Trim();

        if (Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
        {
            var endpointBase = endpointUri.AbsoluteUri.EndsWith("/")
                ? endpointUri.AbsoluteUri
                : endpointUri.AbsoluteUri + "/";
            return new Uri(new Uri(endpointBase), "generate").ToString();
        }

        var baseUri = new Uri(_options.OllamaBaseUrl.TrimEnd('/') + "/");
        var normalizedEndpoint = endpoint.Trim('/');
        var apiUri = string.IsNullOrWhiteSpace(normalizedEndpoint)
            ? baseUri
            : new Uri(baseUri, normalizedEndpoint + "/");

        return new Uri(apiUri, "generate").ToString();
    }
 
    // ── HELPERS ───────────────────────────────────────────────────────────────
 
    private static string ExtractText(GenerateContentResponse response)
    {
        var candidate = response.Candidates?[0];
        var text = candidate?.Content?.Parts?[0]?.Text;
        var finishReason = candidate?.FinishReason;

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException(
                $"Gemini returned empty response. FinishReason: {finishReason}");
        }

        // Explicitly catch if the model truncated the output
        if (finishReason == FinishReason.MaxTokens)
        {
            throw new Exception("Generation truncated: The output reached the MaxOutputTokens limit.");
        }

        return text;
    }
 
    private static async Task<byte[]> ReadStreamAsync(Stream stream, CancellationToken ct)
    {
        if (stream is MemoryStream ms) return ms.ToArray();
 
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, ct);
        return buffer.ToArray();
    } 
}