namespace Common.SampleDataGenerator.RAG.Chunking;

using Common.SampleDataGenerator.RAG.Models;
using Microsoft.Extensions.Options;

public sealed class SimpleTextChunker : ITextChunker
{
    private readonly TextChunkingOptions _options;

    public SimpleTextChunker(IOptions<TextChunkingOptions> options)
    {
        _options = options.Value;
        _options.Validate();
    }

    public IReadOnlyList<DocumentChunk> Chunk(
        string documentName,
        string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var chunks = new List<DocumentChunk>();

        var start = 0;
        var index = 0;

        while (start < text.Length)
        {
            var length = Math.Min(
                _options.MaxChunkSize,
                text.Length - start);

            var content = text.Substring(start, length).Trim();

            if (!string.IsNullOrWhiteSpace(content))
            {
                chunks.Add(new DocumentChunk
                {
                    Id = $"{Path.GetFileNameWithoutExtension(documentName)}-{index}",
                    DocumentName = documentName,
                    Content = content,
                    ChunkIndex = index,
                });
            }

            start += _options.MaxChunkSize - _options.OverlapSize;
            index++;
        }

        return chunks;
    }
}
