namespace Common.SampleDataGenerator.RAG.Retrievals;

using Common.SampleDataGenerator.Options;
using Common.SampleDataGenerator.RAG.Abstractions;
using Common.SampleDataGenerator.RAG.Models;
using Microsoft.Extensions.Options;
using Qdrant.Client.Grpc;
using QdrantClient = Qdrant.Client.QdrantClient;

public sealed class QdrantRagRetriever : IRagRetriever
{
    private readonly QdrantClient _qdrantClient;
    private readonly ITextEmbeddingGenerator _embeddingGenerator;
    private readonly QdrantOptions _options;

    public QdrantRagRetriever(
        QdrantClient qdrantClient,
        ITextEmbeddingGenerator embeddingGenerator,
        IOptions<QdrantOptions> options)
    {
        this._qdrantClient = qdrantClient;
        this._embeddingGenerator = embeddingGenerator;
        this._options = options.Value;
    }

    public async Task AddAsync(RagDocument document, CancellationToken cancellationToken)
    {
        var vector = await _embeddingGenerator.GenerateEmbeddingAsync(document.Content, cancellationToken);

        var point = new PointStruct
        {
            Id = Guid.Parse(document.Id),
            Vectors = vector,
            Payload =
        {
            ["content"] = document.Content,
        },
        };

        await _qdrantClient.UpsertAsync(
            collectionName: _options.CollectionName,
            points: new[] { point });
    }

    public async Task<IReadOnlyList<RagDocument>> SearchAsync(
    string query,
    CancellationToken cancellationToken,
    int maxResults = 3)
    {
        var vector = await _embeddingGenerator.GenerateEmbeddingAsync(query, cancellationToken);

        var results = await _qdrantClient.SearchAsync(
        collectionName: _options.CollectionName,
        vector: vector,
        limit: (ulong)maxResults);

        return results
        .Select(result => new RagDocument(
            result.Id.Uuid,
            result.Payload["content"].StringValue))
        .ToList();
    }
}
