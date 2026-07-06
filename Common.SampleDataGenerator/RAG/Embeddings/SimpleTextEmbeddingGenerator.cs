namespace Common.SampleDataGenerator.RAG.Embeddings;

using Common.SampleDataGenerator.RAG.Abstractions;

public sealed class SimpleTextEmbeddingGenerator : ITextEmbeddingGenerator
{
    public Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        return Task.FromResult(new[]
        {
            Contains(text, "refresh") ? 1f : 0f,
            Contains(text, "token") ? 1f : 0f,
            Contains(text, "ticket") ? 1f : 0f,
            Contains(text, "error") ? 1f : 0f,
            Contains(text, "closed") ? 1f : 0f,
        });
    }

    private static bool Contains(string text, string word)
    {
        return text.Contains(
            word,
            StringComparison.OrdinalIgnoreCase);
    }
}
