namespace Common.SampleDataGenerator.RAG.Embeddings;

using System.Net.Http.Json;
using Common.SampleDataGenerator.RAG.Abstractions;

public sealed class OllamaTextEmbeddingGenerator : ITextEmbeddingGenerator
{
    private readonly HttpClient _httpClient;

    public OllamaTextEmbeddingGenerator(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        var request = new
        {
            model = "nomic-embed-text",
            prompt = text,
        };

        var response = await _httpClient.PostAsJsonAsync(
            "/api/embeddings",
            request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<OllamaEmbeddingResponse>();

        return result?.Embedding
            ?? throw new InvalidOperationException("Ollama did not return an embedding.");
    }

    private sealed record OllamaEmbeddingResponse(
    float[] Embedding);
}
