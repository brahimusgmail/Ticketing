namespace Common.SampleDataGenerator.Options;

using System.ComponentModel.DataAnnotations;

public sealed class SampleDataGeneratorOptions
{
    public const string SectionName = "SampleDataGenerator";

    /// <summary>
    /// Gets or sets the path to the model file used for generating sample data.
    /// </summary>
    [Required]
    public string ModelPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the context window for the model. This determines how many tokens the model can consider at once when generating sample data.
    /// </summary>
    public uint ContextSize { get; set; } = 2048;

    /// <summary>
    /// Gets or sets the number of GPU layers to use for generating sample data 0 veux dire aucun (utilise que le cpu).
    /// </summary>
    public int GpuLayerCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate for each sample data item. This limits the length of the generated output.
    /// </summary>
    public int MaxTokens { get; set; } = 512;

    /// <summary>
    /// Gets or sets the temperature for generating sample data. This controls the randomness of the generated output.
    /// pour json on utilise 0.1 ou 0.2 pour avoir un json valide, plus on augmente la temperature plus le json sera aléatoire et donc moins valide.
    /// </summary>
    public float Temperature { get; set; } = 0.2f;

    /// <summary>
    /// Gets or sets the number of sample data records to generate. This determines how many items will be included in the generated output.
    /// </summary>
    public int RecordCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the starting ID for the generated sample data records. This allows for sequential numbering of the generated items.
    /// </summary>
    public int StartingId { get; set; } = 1;

    /// <summary>
    /// Gets or sets the provider to use for generating sample data. This allows for selecting between different implementations of sample data generation, such as LlamaSharp or Ollama.
    /// </summary>
    public SampleDataGeneratorProvider Provider { get; set; }
    = SampleDataGeneratorProvider.LlamaSharp;

    /// <summary>
    /// Gets or sets the base URL for the Ollama API. This is used when the Ollama provider is selected for generating sample data.
    /// </summary>
    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Gets or sets the model to use for the Ollama API. This is used when the Ollama provider is selected for generating sample data.
    /// </summary>
    public string OllamaModel { get; set; } = "llama3";
}
