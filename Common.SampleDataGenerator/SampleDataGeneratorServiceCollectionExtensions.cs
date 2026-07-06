namespace Common.SampleDataGenerator;

using Common.SampleDataGenerator.Abstractions;
using Common.SampleDataGenerator.Options;
using Common.SampleDataGenerator.Providers;
using Common.SampleDataGenerator.RAG.Abstractions;
using Common.SampleDataGenerator.RAG.Answering;
using Common.SampleDataGenerator.RAG.Chunking;
using Common.SampleDataGenerator.RAG.Embeddings;
using Common.SampleDataGenerator.RAG.Extraction;
using Common.SampleDataGenerator.RAG.Indexing;
using Common.SampleDataGenerator.RAG.Ingestion;
using Common.SampleDataGenerator.RAG.Retrievals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Qdrant.Client;

public static class SampleDataGeneratorServiceCollectionExtensions
{
    public static IServiceCollection AddSampleDataGenerator(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<SampleDataGeneratorOptions>()
                .Bind(configuration.GetSection(SampleDataGeneratorOptions.SectionName))
                .ValidateDataAnnotations()
                .Validate(
                   options => File.Exists(options.ModelPath),
                   "The model file does not exist.")
                .ValidateOnStart();

        services.Configure<QdrantOptions>(
        configuration.GetSection("Qdrant"));

        services.AddSingleton<QdrantClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<QdrantOptions>>().Value;

            return new QdrantClient(options.Host, options.Port);
        });

        services.AddSingleton<QdrantCollectionInitializer>();
        services.AddScoped<IDocumentChunkIndexer, QdrantRagIndexer>();
        services.AddScoped<QdrantRagRetriever>();

        services.TryAddSingleton<ISampleDataGenerator>(serviceProvider =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<SampleDataGeneratorOptions>>()
                .Value;

            return options.Provider switch
            {
                SampleDataGeneratorProvider.LlamaSharp =>
                    ActivatorUtilities.CreateInstance<LlamaSharpSampleDataGenerator>(
                        serviceProvider),

                SampleDataGeneratorProvider.Ollama =>
                    ActivatorUtilities.CreateInstance<OllamaSampleDataGenerator>(
                        serviceProvider),

                _ => throw new InvalidOperationException(
                    $"Unsupported sample data generator provider: {options.Provider}"),
            };
        });

        services.TryAddSingleton<IRagRetriever, QdrantRagRetriever>();

        services.AddHttpClient<ITextEmbeddingGenerator, OllamaTextEmbeddingGenerator>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434");
        });

        services.Configure<TextChunkingOptions>(
                     configuration.GetSection("Rag:Chunking"));

        services.AddSingleton<ITextExtractor, PlainTextExtractor>();

        services.AddSingleton<ITextChunker, SimpleTextChunker>();

        services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();
        services.AddSingleton<ISampleDataGenerator, LlamaSharpSampleDataGenerator>();
        services.AddSingleton<ILlmTextGenerator>(sp =>
            (ILlmTextGenerator)sp.GetRequiredService<ISampleDataGenerator>());

        services.AddScoped<IRagAnswerService, RagAnswerService>();
        return services;
    }

    public static IServiceCollection AddSampleDataGenerator(
    this IServiceCollection services,
    Action<SampleDataGeneratorOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.TryAddSingleton<ISampleDataGenerator>(serviceProvider =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<SampleDataGeneratorOptions>>()
                .Value;

            return options.Provider switch
            {
                SampleDataGeneratorProvider.LlamaSharp =>
                    ActivatorUtilities.CreateInstance<LlamaSharpSampleDataGenerator>(
                        serviceProvider),

                SampleDataGeneratorProvider.Ollama =>
                    ActivatorUtilities.CreateInstance<OllamaSampleDataGenerator>(
                        serviceProvider),

                _ => throw new InvalidOperationException(
                    $"Unsupported sample data generator provider: {options.Provider}"),
            };
        });

        services.TryAddSingleton<IRagRetriever, InMemoryRagRetriever>();
        services.TryAddSingleton<ITextEmbeddingGenerator, SimpleTextEmbeddingGenerator>();

        return services;
    }
}
