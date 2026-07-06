namespace Common.SampleDataGenerator.RAG.Answering;

using Common.SampleDataGenerator.Abstractions;
using Common.SampleDataGenerator.RAG.Abstractions;

public sealed class RagAnswerService : IRagAnswerService
{
    private readonly IRagRetriever _ragRetriever;
    private readonly ILlmTextGenerator _llmTextGenerator;

    public RagAnswerService(
        IRagRetriever ragRetriever,
        ILlmTextGenerator llmTextGenerator)
    {
        _ragRetriever = ragRetriever;
        _llmTextGenerator = llmTextGenerator;
    }

    public async Task<string> AskAsync(
        string question,
        CancellationToken cancellationToken = default)
    {
        var documents = await _ragRetriever.SearchAsync(
            question,
            cancellationToken,
            maxResults: 5);

        var context = string.Join(
            Environment.NewLine + Environment.NewLine + "---" + Environment.NewLine + Environment.NewLine,
            documents.Select(document => document.Content));

        var prompt = $"""
        <|start_header_id|>system<|end_header_id|>

        Tu es un assistant technique.
        Tu dois répondre uniquement avec le contexte fourni.
        Si la réponse n'existe pas dans le contexte, réponds exactement : "Je ne sais pas."

        <|eot_id|><|start_header_id|>user<|end_header_id|>

        CONTEXTE :
        {context}

        QUESTION :
        {question}

        <|eot_id|><|start_header_id|>assistant<|end_header_id|>
        """;

        return await _llmTextGenerator.GenerateTextAsync(
            prompt,
            cancellationToken);
    }
}
