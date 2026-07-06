namespace Common.SampleDataGenerator.RAG.Ingestion;

using Common.SampleDataGenerator.RAG.Abstractions;
using Common.SampleDataGenerator.RAG.Chunking;
using Common.SampleDataGenerator.RAG.Extraction;
using Common.SampleDataGenerator.RAG.Indexing;

public sealed class DocumentIngestionService : IDocumentIngestionService
{
    private readonly IEnumerable<ITextExtractor> _extractors;
    private readonly ITextChunker _chunker;
    private readonly ITextEmbeddingGenerator _embeddingGenerator;
    private readonly IDocumentChunkIndexer _indexer;

    public DocumentIngestionService(
        IEnumerable<ITextExtractor> extractors,
        ITextChunker chunker,
        ITextEmbeddingGenerator embeddingGenerator,
        IDocumentChunkIndexer indexer)
    {
        _extractors = extractors;
        _chunker = chunker;
        _embeddingGenerator = embeddingGenerator;
        _indexer = indexer;
    }

    public async Task IngestAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var extractor = _extractors.FirstOrDefault(x => x.CanExtract(filePath));

        if (extractor is null)
        {
            throw new NotSupportedException(
                $"No text extractor found for file '{filePath}'.");
        }

        var text = await extractor.ExtractAsync(filePath, cancellationToken);

        var documentName = Path.GetFileName(filePath);

        var chunks = _chunker.Chunk(documentName, text);

        foreach (var chunk in chunks)
        {
            var embedding = await _embeddingGenerator.GenerateEmbeddingAsync(
                chunk.Content,
                cancellationToken);

            await _indexer.IndexAsync(
                chunk,
                embedding,
                cancellationToken);
        }
    }
}
