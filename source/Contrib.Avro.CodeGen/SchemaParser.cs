using System.Diagnostics.CodeAnalysis;
using Avro;
using Avro.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Contrib.Avro.Codegen;

public sealed record GeneratorItem<T>(AdditionalText Item, AvroGenOptions Options, T Value);

public static class GeneratorItem
{
    public static GeneratorItem<TResult> Select<TSource, TResult>(this GeneratorItem<TSource> source,
        Func<TSource, TResult> selector) =>
        new(source.Item, source.Options, selector(source.Value));
}

public abstract record SchemaResult
{
    public sealed record Success(GeneratorItem<Schema> Source) : SchemaResult;

    public sealed record Failure(AdditionalText Item, Exception? Error, Location Location) : SchemaResult;

    public bool GetValue(SourceProductionContext ctx, [NotNullWhen(true)] out GeneratorItem<Schema>? value)
    {
        switch (this)
        {
            case Failure x:
                ctx.Error(x.Error!.Message, x.Location);
                value = null;
                return false;
            case Success s:
                value = s.Source;
                return true;
            default:
                value = default;
                return false;
        }
    }
}

public static class SchemaParser
{
    public static SchemaResult ParseSchema(
        AdditionalText item,
        AvroGenOptions options,
        CancellationToken cancellationToken = default)
    {
        var text = item.GetText(cancellationToken)!.ToString();
        try
        {
            var schemaJson = JToken.Parse(text);
            SchemaUtils.ReplaceNamespace(schemaJson, options.NamespaceMapping);

            FixUpLogicalTypes(schemaJson, options.LogicalTypes, options.FailUnknownLogicalTypes);

            var schemaText = schemaJson.ToString();

            var schema = Schema.Parse(schemaText);
            return new SchemaResult.Success(new GeneratorItem<Schema>(item, options, schema));
        }
        catch (JsonReaderException e)
        {
            var startPos = new LinePosition(e.LineNumber, e.LinePosition);
            var endPos = new LinePosition(e.LineNumber + 1, 0);
            var location = Location.Create(item.Path, default, new LinePositionSpan(startPos, endPos));
            return new SchemaResult.Failure(item, e, location);
        }
        catch (SchemaParseException e)
        {
            var location = Location.Create(item.Path, default, default);
            return new SchemaResult.Failure(item, e, location);
        } catch (AvroTypeException e)
        {
            var location = Location.Create(item.Path, default, default);
            return new SchemaResult.Failure(item, e, location);
        }
    }


    private static bool LogicalTypeExists(JToken logicalType)
    {
        try
        {
            return Schema.Parse(logicalType.ToString()) is not null;
        }
        catch (AvroTypeException)
        {
            return false;
        }
    }

    private static void FixUpLogicalTypes(
        JToken schema,
        IReadOnlyDictionary<string, string> logicalTypes,
        bool failUnknownLogicalTypes)
    {
        void Traverse(JToken token)
        {
            switch (token)
            {
                case JObject obj:
                    obj.TryGetValue("logicalType", out var logicalTypeToken);
                    var logicalType = logicalTypeToken?.ToString();
                    if (!string.IsNullOrEmpty(logicalType))
                    {
                        if (!LogicalTypeExists(obj))
                        {
                            LogicalType substitute = logicalTypes.TryGetValue(logicalType, out var dotnetType)
                                ? new RegisteredLogicalType(logicalType, dotnetType)
                                : new UnknownLogicalType(logicalType);

                            if (failUnknownLogicalTypes && substitute is UnknownLogicalType)
                                throw new AvroTypeException(
                                    $"Logical type {logicalType} is not supported and is not mapped to a .NET type in the generator options.");

                            LogicalTypeFactory.Instance.Register(substitute);
                        }

                        break;
                    }

                    // Traverse fields, items, values, or types
                    obj.TryGetValue("fields", out var fields);
                    obj.TryGetValue("items", out var items);
                    obj.TryGetValue("values", out var values);
                    obj.TryGetValue("type", out var type);

                    if (fields is JArray array) array.ForEach(field => Traverse(field["type"]!));
                    else if (items != null) Traverse(items);
                    else if (values != null) Traverse(values);
                    else if (type is JArray cases) cases.ForEach(Traverse);
                    else if (type is JObject typeObj) Traverse(typeObj);

                    break;

                case JArray arr:
                    arr.ForEach(Traverse);
                    break;
            }
        }

        Traverse(schema);
    }
}
