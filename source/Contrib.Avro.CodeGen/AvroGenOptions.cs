using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Contrib.Avro.Codegen;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DebuggerDisplayFields
{
    None,
    All,
    Required
}

public sealed record AvroTypeOptions(
    IReadOnlyDictionary<string, string> TypeMappings,
    string TypeHintPropertyName,
    bool FailUnknownLogicalTypes);

public sealed record AvroTypeOptionsConfig(
    Optional<IReadOnlyDictionary<string, string>> TypeMappings,
    Optional<string> TypeHintPropertyName,
    Optional<bool> FailUnknownLogicalTypes)
{
    public static AvroTypeOptionsConfig Default =>
        new(
            TypeMappings: new Dictionary<string, string>(),
            TypeHintPropertyName: default,
            FailUnknownLogicalTypes: default);

    public AvroTypeOptionsConfig Combine(AvroTypeOptionsConfig other) =>
        new(
            TypeMappings.Merge(other.TypeMappings, (x, y) => x.Merge(y)),
            TypeHintPropertyName.Or(other.TypeHintPropertyName),
            FailUnknownLogicalTypes.Or(other.FailUnknownLogicalTypes));

    public AvroTypeOptions ToOptions() =>
        new(
            TypeMappings.DefaultIfEmpty(new Dictionary<string, string>().AsReadOnly()),
            TypeHintPropertyName.DefaultIfEmpty("typeHint"),
            FailUnknownLogicalTypes.DefaultIfEmpty(false));
}

public sealed record AvroGenOptionsConfig(
    Optional<IReadOnlyDictionary<string, string>> NamespaceMapping,
    Optional<AvroTypeOptionsConfig> TypeOptions,
    Optional<bool> GenerateRequiredFields,
    Optional<bool> GenerateRecords,
    Optional<DebuggerDisplayFields> DebuggerDisplayFields)
{
    public static AvroGenOptionsConfig Default =>
        new(
            NamespaceMapping: new Dictionary<string, string>(),
            TypeOptions: AvroTypeOptionsConfig.Default,
            GenerateRequiredFields: default,
            GenerateRecords: default,
            DebuggerDisplayFields: default);
    public AvroGenOptionsConfig Combine(AvroGenOptionsConfig other)
    {
        try
        {
            return new AvroGenOptionsConfig(
                NamespaceMapping.Merge(other.NamespaceMapping, (x, y) => x.Merge(y)),
                TypeOptions.Merge(other.TypeOptions, (x, y) => x.Combine(y)),
                GenerateRequiredFields.Or(other.GenerateRequiredFields),
                GenerateRecords.Or(other.GenerateRecords),
                DebuggerDisplayFields.Or(other.DebuggerDisplayFields));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.StackTrace?.Replace("\n", ";;"));
        }
    }

    public AvroGenOptions ToOptions() =>
        new(
            NamespaceMapping.DefaultIfEmpty(new Dictionary<string, string>().AsReadOnly()),
            GenerateRequiredFields.DefaultIfEmpty(true),
            GenerateRecords.DefaultIfEmpty(true),
            TypeOptions.DefaultIfEmpty(AvroTypeOptionsConfig.Default).ToOptions(),
            DebuggerDisplayFields.DefaultIfEmpty(Contrib.Avro.Codegen.DebuggerDisplayFields.None));

    public static AvroGenOptionsConfig FromAnalyzerConfig(AnalyzerConfigOptions options)
    {
        var genRequired = options.GetMsBuildBoolean("GenerateRequiredFields");
        var genRecords = options.GetMsBuildBoolean("GenerateRecords");
        var dbgDisplayFields = options.GetMsBuildEnum<DebuggerDisplayFields>("DebuggerDisplayFields");
        var namespaceMapping = options.GetMsBuildDictionary("NamespaceMapping");
        var failUnknownLogicalTypes = options.GetMsBuildBoolean("FailUnknownLogicalTypes");
        var typeMappings = options.GetMsBuildDictionary("TypeMappings");
        var typeHintPropertyName = options.GetMsBuildProperty("TypeHintName");
        return new AvroGenOptionsConfig(
            namespaceMapping,
            new AvroTypeOptionsConfig(typeMappings, typeHintPropertyName, failUnknownLogicalTypes),
            genRequired,
            genRecords,
            dbgDisplayFields);
    }

    public static AvroGenOptionsConfig FromJson(string json) =>
        JsonSerializer.Deserialize<AvroGenOptionsConfig>(json, AvroGenJsonOptions.Default) ?? Default;
}

public sealed record AvroGenOptions(
    IReadOnlyDictionary<string, string> NamespaceMapping,
    bool GenerateRequiredFields,
    bool GenerateRecords,
    AvroTypeOptions TypeOptions,
    DebuggerDisplayFields DebuggerDisplayFields);
