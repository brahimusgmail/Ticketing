namespace Common.SampleDataGenerator.RAG.Chunking;

public sealed class TextChunkingOptions
{
    public int MaxChunkSize { get; set; } = 800;

    public int OverlapSize { get; set; } = 120;

    public void Validate()
    {
        if (MaxChunkSize <= 0)
        {
            throw new InvalidOperationException(
                "MaxChunkSize must be greater than 0.");
        }

        if (OverlapSize < 0)
        {
            throw new InvalidOperationException(
                "OverlapSize cannot be negative.");
        }

        if (OverlapSize >= MaxChunkSize)
        {
            throw new InvalidOperationException(
                "OverlapSize must be smaller than MaxChunkSize.");
        }
    }
}
