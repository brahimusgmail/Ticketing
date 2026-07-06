namespace Common.SampleDataGenerator.RAG.Abstractions;

using Common.SampleDataGenerator.RAG.Models;

public interface IRagRetriever
{
    Task AddAsync(RagDocument document, CancellationToken cancellationToken);

    Task<IReadOnlyList<RagDocument>> SearchAsync(
        string query,
        CancellationToken cancellationToken,
        int maxResults = 3);
}
