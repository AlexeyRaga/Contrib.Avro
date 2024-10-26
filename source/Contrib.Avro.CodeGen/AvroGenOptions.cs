using Microsoft.CodeAnalysis.Diagnostics;

namespace Contrib.Avro.Codegen;

public enum DebuggerDisplayFields
{
    None,
    All,
    Required
}

public sealed record TypeMappingsOptions(
    IReadOnlyDictionary<string, string> TypeMappings,
    string TypeHintPropertyName,
    bool FailUnknownLogicalTypes);

public sealed record AvroGenOptions(
    IReadOnlyDictionary<string, string> NamespaceMapping,
    bool GenerateRequiredFields,
    bool GenerateRecords,
    TypeMappingsOptions TypeMappingsMappings,
    DebuggerDisplayFields DebuggerDisplayFields)
{
    public static AvroGenOptions Create(AnalyzerConfigOptions options)
    {
        var genRequired = options.GetMsBuildBoolean("GenerateRequiredFields", true);
        var genRecords = options.GetMsBuildBoolean("GenerateRecords", true);
        var dbgDisplayFields = options.GetMsBuildEnum("DebuggerDisplayFields", DebuggerDisplayFields.None);

        var namespaceMapping = options.GetMsBuildDictionary("NamespaceMapping");

        var failUnknownLogicalTypes = options.GetMsBuildBoolean("FailUnknownLogicalTypes");
        var typeMappings = options.GetMsBuildDictionary("TypeMappings");
        var typeHintPropertyName = options.GetMsBuildProperty("TypeHintName") ?? "typeHint";

        var ltOptions = new TypeMappingsOptions(typeMappings, typeHintPropertyName, failUnknownLogicalTypes);

        return new AvroGenOptions(
            namespaceMapping,
            genRequired,
            genRecords,
            ltOptions,
            dbgDisplayFields);
    }
}
