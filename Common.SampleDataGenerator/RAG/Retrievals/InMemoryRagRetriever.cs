namespace Common.SampleDataGenerator.RAG.Retrievals;

using Common.SampleDataGenerator.RAG.Abstractions;
using Common.SampleDataGenerator.RAG.Models;
using Microsoft.Extensions.AI;

public sealed class InMemoryRagRetriever : IRagRetriever
{
    private readonly List<RagDocument> _documents = [];
    private readonly ITextEmbeddingGenerator _textEmbeddingGenerator;

    public InMemoryRagRetriever(ITextEmbeddingGenerator textEmbeddingGenerator)
    {
        _textEmbeddingGenerator = textEmbeddingGenerator;

        AddAsync(
            new RagDocument(
            "1",
            "A ticket cannot be commented when it is closed."),
            CancellationToken.None
                ).GetAwaiter().GetResult();

        AddAsync(
            new RagDocument(
            "2",
            "Refresh tokens must be stored hashed in the database."),
            CancellationToken.None).GetAwaiter().GetResult();

        AddAsync(
            new RagDocument(
            "3",
            "Use ProblemDetails to return API errors."),
            CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task AddAsync(RagDocument document, CancellationToken cancellationToken)
    {
        var embedding = await _textEmbeddingGenerator.GenerateEmbeddingAsync(document.Content, cancellationToken);
        Console.WriteLine(
       $"ADDING {document.Id}: {string.Join(",", embedding)}");
        _documents.Add(document with
        {
            Embedding = embedding,
        });
    }

    public async Task<IReadOnlyList<RagDocument>> SearchAsync(string query, CancellationToken cancellationToken, int maxResults = 3)
    {
        var queryEmbedding = await _textEmbeddingGenerator.GenerateEmbeddingAsync(query, cancellationToken);
        foreach (var document in _documents)
        {
            Console.WriteLine($"DOC: {document.Content}");
            Console.WriteLine($"EMBEDDING: {string.Join(",", document.Embedding ?? [])}");
        }

        Console.WriteLine($"QUERY: {query}");
        Console.WriteLine($"QUERY EMBEDDING: {string.Join(",", queryEmbedding)}");
        var results = _documents
                    .Where(document => document.Embedding is not null)
                    .Select(document => new
                    {
                        Document = document,
                        Score = DotProduct(queryEmbedding, document.Embedding!),
                    })
                    .Where(result => result.Score > 0)
                    .OrderByDescending(result => result.Score)
                    .Take(maxResults)
                    .Select(result => result.Document)
                    .ToList();

        return results;
    }

    private static float DotProduct(float[] left, float[] right)
    {
        var sum = 0f;

        for (var i = 0; i < left.Length; i++)
        {
            sum += left[i] * right[i];
        }

        return sum;
    }
}
