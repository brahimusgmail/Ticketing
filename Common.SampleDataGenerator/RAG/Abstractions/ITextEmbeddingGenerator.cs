namespace Common.SampleDataGenerator.RAG.Abstractions;

public interface ITextEmbeddingGenerator
{
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken);
}
