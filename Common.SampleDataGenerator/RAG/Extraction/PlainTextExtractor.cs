namespace Common.SampleDataGenerator.RAG.Extraction;

public sealed class PlainTextExtractor : ITextExtractor
{
    private static readonly string[] SupportedExtensions =
    [
        ".txt",
        ".md",
    ];

    public bool CanExtract(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        return SupportedExtensions.Contains(
            extension,
            StringComparer.OrdinalIgnoreCase);
    }

    public async Task<string> ExtractAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        return await File.ReadAllTextAsync(filePath, cancellationToken);
    }
}
