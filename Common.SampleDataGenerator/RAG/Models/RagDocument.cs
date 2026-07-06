namespace Common.SampleDataGenerator.RAG.Models;

public sealed record RagDocument(
    string Id,
    string Content,
    float[]? Embedding = null);
