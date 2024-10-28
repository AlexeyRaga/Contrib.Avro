using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;

namespace Contrib.Avro.CodeGen;

public class OptionalConverter<T> : JsonConverter<Optional<T>>
{
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;

        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return value is null ? new Optional<T>() : new Optional<T>(value);
    }

    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        if (!value.HasValue) writer.WriteNullValue();
        else JsonSerializer.Serialize(writer, value.Value, options);
    }
}

public class OptionalConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Check if the type is Optional<T>
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter)Activator
            .CreateInstance(typeof(OptionalConverter<>)
                .MakeGenericType(typeToConvert.GetGenericArguments()[0]))!;
}


public static class AvroGenJsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        Converters = { new OptionalConverterFactory() }
    };
}
