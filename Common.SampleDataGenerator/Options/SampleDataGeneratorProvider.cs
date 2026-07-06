namespace Common.SampleDataGenerator.Options;

public enum SampleDataGeneratorProvider
{
    /// <summary>
    /// LlamaSharp is a C# wrapper for the LLaMA (Large Language Model Meta AI) model, which allows developers to integrate and utilize the capabilities of the LLaMA model within their C# applications. It provides an interface to interact with the model, enabling tasks such as text generation, completion, and other natural language processing functionalities.
    /// </summary>
    LlamaSharp = 0,

    /// <summary>
    /// Ollama is a C# library that provides an interface to interact with the Ollama API, which is a service that allows developers to access and utilize large language models for various natural language processing tasks. It enables functionalities such as text generation, completion, and other AI-driven text manipulations within C# applications.
    /// </summary>
    Ollama = 1,
}
