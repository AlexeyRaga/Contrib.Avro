using Microsoft.CodeAnalysis.Diagnostics;

namespace Contrib.Avro.Codegen;

public enum DebuggerDisplayFields
{
    None,
    All,
    Required
}

public sealed record LogicalTypeOptions(
    IReadOnlyDictionary<string, string> LogicalTypes,
    string LogicalTypeHintPropertyName,
    bool FailUnknownLogicalTypes);

public sealed record AvroGenOptions(
    IReadOnlyDictionary<string, string> NamespaceMapping,
    bool GenerateRequiredFields,
    bool GenerateRecords,
    LogicalTypeOptions LogicalTypes,
    DebuggerDisplayFields DebuggerDisplayFields)
{
    public static AvroGenOptions Create(AnalyzerConfigOptions options)
    {
        var genRequired = options.GetMsBuildBoolean("GenerateRequiredFields", true);
        var genRecords = options.GetMsBuildBoolean("GenerateRecords");
        var dbgDisplayFields = options.GetMsBuildEnum("DebuggerDisplayFields", DebuggerDisplayFields.None);

        var namespaceMapping = options.GetMsBuildDictionary("NamespaceMapping");

        var failUnknownLogicalTypes = options.GetMsBuildBoolean("FailUnknownLogicalTypes");
        var logicalTypes = options.GetMsBuildDictionary("LogicalTypes");
        var logicalTypeHintPropertyName = options.GetMsBuildProperty("LogicalTypeHintName") ?? "logicalTypeHint";

        var ltOptions = new LogicalTypeOptions(logicalTypes, logicalTypeHintPropertyName, failUnknownLogicalTypes);

        return new AvroGenOptions(
            namespaceMapping,
            genRequired,
            genRecords,
            ltOptions,
            dbgDisplayFields);
    }
}
