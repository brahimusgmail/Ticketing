namespace Common.SampleDataGenerator.Providers;

using System.Text;
using System.Text.Json;
using Common.SampleDataGenerator.Abstractions;
using Common.SampleDataGenerator.Exceptions;
using Common.SampleDataGenerator.Options;
using LLama;
using LLama.Common;
using LLama.Sampling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class LlamaSharpSampleDataGenerator : ISampleDataGenerator, ILlmTextGenerator
{
    private static readonly JsonSerializerOptions _deserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly LLamaWeights _model;
    private readonly LLamaContext _context;
    private readonly InteractiveExecutor _executor;
    private readonly SampleDataGeneratorOptions _options;
    private readonly ILogger<LlamaSharpSampleDataGenerator> _logger;

    public LlamaSharpSampleDataGenerator(
        IOptions<SampleDataGeneratorOptions> options,
        ILogger<LlamaSharpSampleDataGenerator> logger)
    {
        _options = options.Value;

        var parameters = new ModelParams(_options.ModelPath)
        {
            ContextSize = _options.ContextSize,
            GpuLayerCount = _options.GpuLayerCount,
        };

        _model = LLamaWeights.LoadFromFile(parameters);
        _context = _model.CreateContext(parameters);
        _executor = new InteractiveExecutor(_context);
        _logger = logger;
    }

    public Task<IReadOnlyList<T>> GenerateAsync<T>(
        object schema,
        string instruction,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync<T>(
            schema,
            instruction,
            _options.RecordCount,
            cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GenerateAsync<T>(
        object schema,
        string instruction,
        int recordCount,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schema);

        if (string.IsNullOrWhiteSpace(instruction))
        {
            throw new ArgumentException("Instruction is required.", nameof(instruction));
        }

        if (recordCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(recordCount));
        }

        var schemaJson = JsonSerializer.Serialize(schema, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        var prompt = BuildJsonPrompt(
            schemaJson,
            instruction,
            recordCount,
            _options.StartingId);

        try
        {
            var rawResponse = await GenerateTextAsync(prompt, cancellationToken);

            _logger.LogDebug("Raw AI response: {RawResponse}", rawResponse);

            return ParseGeneratedItems<T>(recordCount, rawResponse);
        }
        catch (SampleDataJsonException ex)
        {
            _logger.LogWarning(ex, "First generation attempt failed. Retrying once.");

            var retryRawResponse = await GenerateTextAsync(prompt, cancellationToken);

            _logger.LogDebug("Retry raw AI response: {RawResponse}", retryRawResponse);

            return ParseGeneratedItems<T>(recordCount, retryRawResponse);
        }
    }

    public async Task<string> GenerateTextAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var inferenceParams = new InferenceParams
        {
            MaxTokens = _options.MaxTokens,
            AntiPrompts = new List<string>
            {
                "<|eot_id|>",
            },
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = _options.Temperature,
            },
        };

        return await GenerateRawResponseAsync(
            prompt,
            inferenceParams,
            cancellationToken);
    }

    private async Task<string> GenerateRawResponseAsync(
        string prompt,
        InferenceParams inferenceParams,
        CancellationToken cancellationToken)
    {
        var builder = new StringBuilder();

        await foreach (var token in _executor.InferAsync(
            prompt,
            inferenceParams,
            cancellationToken))
        {
            builder.Append(token);
        }

        return builder.ToString();
    }

    private IReadOnlyList<T> ParseGeneratedItems<T>(
        int recordCount,
        string rawResponse)
    {
        var json = ExtractJsonArray(rawResponse);

        try
        {
            using var document = JsonDocument.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new SampleDataJsonException(
                $"Generated content is not valid JSON. JSON: {json}", ex);
        }

        try
        {
            var result = JsonSerializer.Deserialize<List<T>>(
                json,
                _deserializeOptions);

            if (result is null || result.Count == 0)
            {
                throw new SampleDataJsonException(
                    $"Generated JSON was valid but returned no {typeof(T).Name} items.");
            }

            if (result.Count != recordCount)
            {
                _logger.LogWarning(
                    "Generated {ActualCount} items instead of requested {RequestedCount} items.",
                    result.Count,
                    recordCount);
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to deserialize generated JSON to {Type}",
                typeof(T).Name);

            throw new SampleDataJsonException(
                $"Generated JSON cannot be deserialized to {typeof(T).Name}. JSON: {json}", ex);
        }
    }

    private static string BuildJsonPrompt(
        string sampleAsString,
        string instruction,
        int recordCount,
        int startingId = 1)
    {
        return $@"<|start_header_id|>system<|end_header_id|>

You are a precise JSON data generator. You must follow instructions exactly.

<|eot_id|><|start_header_id|>user<|end_header_id|>

Generate exactly {recordCount} records of realistic sample data following this structure:

{sampleAsString}

CRITICAL REQUIREMENTS:
1. Output ONLY a valid JSON array starting with [ and ending with ]
2. Generate exactly {recordCount} complete records
3. Each record MUST match the exact property names and types shown above
4. ALL fields must contain realistic, non-empty values
5. Generate varied information and avoid repeating patterns
6. DO NOT include any text, explanation, or markdown before or after the JSON array
7. Ensure valid JSON syntax with proper commas between objects
8. Make each record unique with different values
9. If there is a property name that requires a numeric ID, start with the ID of {startingId} and increment by one each time
10. Do not copy placeholder values from the structure
11. The user instruction is: {instruction}
12. The output must be ONE SINGLE JSON array containing all records.
13. Do NOT create one array per record.
14. The first character must be [ and the last character must be ].

<|eot_id|><|start_header_id|>assistant<|end_header_id|>";
    }

    private static string ExtractJsonArray(string response)
    {
        var start = response.IndexOf('[');
        var end = response.LastIndexOf(']');

        if (start < 0)
        {
            throw new SampleDataJsonException(
                $"No JSON array start found. Response: {response}");
        }

        if (end < 0)
        {
            throw new SampleDataJsonException(
                $"JSON array appears incomplete. Missing closing ']'. Try increasing MaxTokens. Response: {response}");
        }

        return response[start..(end + 1)];
    }
}
