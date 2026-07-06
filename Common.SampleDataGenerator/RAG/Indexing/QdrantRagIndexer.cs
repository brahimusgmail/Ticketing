namespace Common.SampleDataGenerator.RAG.Indexing;

using Common.SampleDataGenerator.Options;
using Common.SampleDataGenerator.RAG.Abstractions;
using Common.SampleDataGenerator.RAG.Models;
using Microsoft.Extensions.Options;
using Qdrant.Client.Grpc;
using QdrantClient = Qdrant.Client.QdrantClient;

public sealed class QdrantRagIndexer : IDocumentChunkIndexer
{
    private readonly QdrantClient _qdrantClient;
    private readonly ITextEmbeddingGenerator _embeddingGenerator;
    private readonly QdrantOptions _options;

    public QdrantRagIndexer(
        QdrantClient qdrantClient,
        ITextEmbeddingGenerator embeddingGenerator,
        IOptions<QdrantOptions> options)
    {
        this._qdrantClient = qdrantClient;
        this._embeddingGenerator = embeddingGenerator;
        this._options = options.Value;
    }

    public async Task IndexAsync(
        DocumentChunk chunk,
        float[] embedding,
        CancellationToken cancellationToken = default)
    {
        var point = new PointStruct
        {
            Id = Guid.NewGuid(),
            Vectors = embedding,
            Payload =
            {
                ["id"] = chunk.Id,
                ["documentName"] = chunk.DocumentName,
                ["chunkIndex"] = chunk.ChunkIndex,
                ["content"] = chunk.Content,
            },
        };

        await _qdrantClient.UpsertAsync(
            collectionName: _options.CollectionName,
            points: new[] { point },
            cancellationToken: cancellationToken);
    }
}
