namespace Common.SampleDataGenerator.Exceptions;

public sealed class SampleDataJsonException : Exception
{
    public SampleDataJsonException(string message)
        : base(message)
    {
    }

    public SampleDataJsonException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
