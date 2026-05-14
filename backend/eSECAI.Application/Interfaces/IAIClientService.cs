
public interface IAIClientService
{
    Task<string> GenerateAsync(
      string prompt,
      int maxTokens = 4096,
      CancellationToken ct = default)
    ;
    
    Task<string> GenerateWithFileAsync(
      Stream fileStream,
      string mimeType,
      string prompt,
      int maxTokens = 4096,
      CancellationToken ct = default
    );
}