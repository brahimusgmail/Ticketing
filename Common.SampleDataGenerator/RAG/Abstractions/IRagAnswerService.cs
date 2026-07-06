namespace Common.SampleDataGenerator.RAG.Abstractions;

public interface IRagAnswerService
{
    Task<string> AskAsync(string question, CancellationToken ct);
}
