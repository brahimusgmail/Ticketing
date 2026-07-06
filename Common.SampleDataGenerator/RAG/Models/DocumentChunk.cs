namespace Common.SampleDataGenerator.RAG.Models;

public sealed class DocumentChunk
{
    public required string Id { get; init; }

    public required string DocumentName { get; init; }

    public required string Content { get; init; }

    public int ChunkIndex { get; init; }
}
