namespace esecai.Application.DTOs;

public class AIOptions
{
    public const string SectionName = "AI";

    public string GeminiApiKey { get; set; } = string.Empty;
    public string GeminiModel { get; set; } = string.Empty;

    // Backward compatibility for older config keys.
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int MaxOutputTokens { get; set; } = 8192;

    // Hybrid provider controls
    public string PrimaryProvider { get; set; } = "Ollama"; // Ollama | Gemini
    public bool EnableFallback { get; set; } = true;

    // Ollama settings
    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";
    public string OllamaApiEndpoint { get; set; } = "/api";
    public string OllamaModel { get; set; } = "llama3.1:8b";
    public string OllamaApiKey { get; set; } = string.Empty;
    public int OllamaTimeoutSeconds { get; set; } = 120;
}