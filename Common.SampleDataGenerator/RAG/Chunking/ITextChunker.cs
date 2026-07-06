namespace Common.SampleDataGenerator.RAG.Chunking;

using Common.SampleDataGenerator.RAG.Models;

public interface ITextChunker
{
    IReadOnlyList<DocumentChunk> Chunk(
        string documentName,
        string text);
}
