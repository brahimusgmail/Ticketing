namespace Common.SampleDataGenerator.Options;

public sealed class QdrantOptions
{
    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 6334;

    public string CollectionName { get; set; } = "ticketing-rag";
}
