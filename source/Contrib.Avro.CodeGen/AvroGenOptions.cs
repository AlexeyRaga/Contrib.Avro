using System.Collections.Immutable;
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

public sealed record AvroGenOptionsConfig(
    Optional<Dictionary<string, string>> NamespaceMapping,
    Optional<bool> GenerateRequiredFields,
    Optional<bool> GenerateRecords,
    Optional<DebuggerDisplayFields> DebuggerDisplayFields,
    Optional<Dictionary<string, string>> TypeMappings,
    Optional<string> TypeHintPropertyName,
    Optional<bool> FailUnknownLogicalTypes)
{
    public static AvroGenOptionsConfig Default =>
        new(
            NamespaceMapping: new Dictionary<string, string>(),
            GenerateRequiredFields: default,
            GenerateRecords: default,
            DebuggerDisplayFields: default,
            TypeMappings: default,
            TypeHintPropertyName: default,
            FailUnknownLogicalTypes: default);

    public AvroGenOptionsConfig Combine(AvroGenOptionsConfig other) =>
        new(
            NamespaceMapping.Merge(other.NamespaceMapping, (x, y) => x.Merge(y)),
            GenerateRequiredFields.Or(other.GenerateRequiredFields),
            GenerateRecords.Or(other.GenerateRecords),
            DebuggerDisplayFields.Or(other.DebuggerDisplayFields),
            TypeMappings.Merge(other.TypeMappings, (x, y) => x.Merge(y)),
            TypeHintPropertyName.Or(other.TypeHintPropertyName),
            FailUnknownLogicalTypes.Or(other.FailUnknownLogicalTypes));

    public AvroGenOptions ToOptions() =>
        new(
            NamespaceMapping.DefaultIfEmpty(new Dictionary<string, string>()).ToImmutableDictionary(),
            GenerateRequiredFields.DefaultIfEmpty(true),
            GenerateRecords.DefaultIfEmpty(true),
            new AvroTypeOptions(
                TypeMappings.DefaultIfEmpty(new Dictionary<string, string>()).ToImmutableDictionary(),
                TypeHintPropertyName.DefaultIfEmpty("typeHint"),
                FailUnknownLogicalTypes.DefaultIfEmpty(false)),
            DebuggerDisplayFields.DefaultIfEmpty(Contrib.Avro.Codegen.DebuggerDisplayFields.None));

    public static AvroGenOptionsConfig FromAnalyzerConfig(AnalyzerConfigOptions options) =>
        new(
            NamespaceMapping: options.GetMsBuildDictionary("NamespaceMapping"),
            GenerateRecords: options.GetMsBuildBoolean("GenerateRecords"),
            GenerateRequiredFields:  options.GetMsBuildBoolean("GenerateRequiredFields"),
            DebuggerDisplayFields: options.GetMsBuildEnum<DebuggerDisplayFields>("DebuggerDisplayFields"),
            TypeMappings: options.GetMsBuildDictionary("TypeMappings"),
            TypeHintPropertyName: options.GetMsBuildProperty("TypeHintName"),
            FailUnknownLogicalTypes: options.GetMsBuildBoolean("FailUnknownLogicalTypes"));

    public static AvroGenOptionsConfig FromJson(string json) =>
        JsonSerializer.Deserialize<AvroGenOptionsConfig>(json, AvroGenJsonOptions.Default) ?? Default;
}

public sealed record AvroTypeOptions(
    ImmutableDictionary<string, string> TypeMappings,
    string TypeHintPropertyName,
    bool FailUnknownLogicalTypes);

public sealed record AvroGenOptions(
    ImmutableDictionary<string, string> NamespaceMapping,
    bool GenerateRequiredFields,
    bool GenerateRecords,
    AvroTypeOptions TypeOptions,
    DebuggerDisplayFields DebuggerDisplayFields);
