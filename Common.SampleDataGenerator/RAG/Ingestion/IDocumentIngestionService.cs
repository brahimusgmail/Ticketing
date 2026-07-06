namespace Common.SampleDataGenerator.RAG.Ingestion;

public interface IDocumentIngestionService
{
    Task IngestAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}
