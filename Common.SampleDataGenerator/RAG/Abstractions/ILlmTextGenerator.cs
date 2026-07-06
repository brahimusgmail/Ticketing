public interface ILlmTextGenerator
{
    Task<string> GenerateTextAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}
