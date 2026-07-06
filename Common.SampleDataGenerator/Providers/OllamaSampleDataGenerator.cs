namespace Common.SampleDataGenerator.Providers;

using Common.SampleDataGenerator.Abstractions;

public sealed class OllamaSampleDataGenerator : ISampleDataGenerator
{
    public Task<IReadOnlyList<T>> GenerateAsync<T>(
        string instruction,
        int recordCount,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<T>> GenerateAsync<T>(
        object schema,
        string instruction,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<T>> GenerateAsync<T>(
        object schema,
        string instruction,
        int recordCount,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
