namespace Common.SampleDataGenerator.RAG.Indexing;

using Common.SampleDataGenerator.Options;
using Microsoft.Extensions.Options;
using Qdrant.Client.Grpc;
using QdrantClient = Qdrant.Client.QdrantClient;

public sealed class QdrantCollectionInitializer
{
    private readonly QdrantClient _qdrantClient;
    private readonly QdrantOptions _options;

    public QdrantCollectionInitializer(
        QdrantClient qdrantClient,
        IOptions<QdrantOptions> options)
    {
        this._qdrantClient = qdrantClient;
        this._options = options.Value;
    }

    public async Task InitializeAsync()
    {
        var collections = await _qdrantClient.ListCollectionsAsync();

        var exists = collections.Any(collection =>
            collection == _options.CollectionName);

        if (exists)
        {
            return;
        }

        await _qdrantClient.CreateCollectionAsync(
            collectionName: _options.CollectionName,
            vectorsConfig: new VectorParams
            {
                Size = 768,
                Distance = Distance.Cosine,
            });
    }
}
