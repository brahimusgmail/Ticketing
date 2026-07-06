namespace Common.SampleDataGenerator.Helpers;

public static class SchemaGenerator
{
    public static object Generate<T>()
    {
        var result = new Dictionary<string, object?>();

        foreach (var property in typeof(T).GetProperties())
        {
            result[property.Name] = GetDefaultValue(property.PropertyType);
        }

        return result;
    }

    private static object? GetDefaultValue(Type type)
    {
        if (type == typeof(string))
        {
            return "string";
        }

        if (type == typeof(int))
        {
            return 0;
        }

        if (type == typeof(Guid))
        {
            return Guid.Empty;
        }

        if (type == typeof(DateTime))
        {
            return DateTime.UtcNow;
        }

        if (type == typeof(bool))
        {
            return false;
        }

        return null;
    }
}
