namespace Common.SampleDataGenerator.RAG.Extraction;

public interface ITextExtractor
{
    bool CanExtract(string filePath);

    Task<string> ExtractAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}
