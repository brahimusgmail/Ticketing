namespace Common.SampleDataGenerator.RAG.Indexing;

using Common.SampleDataGenerator.RAG.Models;

public interface IDocumentChunkIndexer
{
    Task IndexAsync(
        DocumentChunk chunk,
        float[] embedding,
        CancellationToken cancellationToken = default);
}
