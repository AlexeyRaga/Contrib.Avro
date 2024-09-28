using Microsoft.CodeAnalysis.Diagnostics;

namespace Contrib.Avro.Codegen;

public enum DebuggerDisplayFields
{
    None,
    All,
    Required
}

public sealed record AvroGenOptions(
    IReadOnlyDictionary<string, string> NamespaceMapping,
    IReadOnlyDictionary<string, string> LogicalTypes,
    bool GenerateRequiredFields,
    bool GenerateRecords,
    bool FailUnknownLogicalTypes,
    DebuggerDisplayFields DebuggerDisplayFields)
{
    public static AvroGenOptions Create(AnalyzerConfigOptions options)
    {
        var genRequired = options.GetMsBuildBoolean("GenerateRequiredFields");
        var genRecords = options.GetMsBuildBoolean("GenerateRecords");
        var failUnknownLogicalTypes = options.GetMsBuildBoolean("FailUnknownLogicalTypes");
        var dbgDisplayFields = options.GetMsBuildEnum("DebuggerDisplayFields", DebuggerDisplayFields.None);

        var namespaceMapping = options.GetMsBuildDictionary("NamespaceMapping");
        var logicalTypes = options.GetMsBuildDictionary("LogicalTypes");


        return new AvroGenOptions(
            namespaceMapping,
            logicalTypes,
            genRequired,
            genRecords,
            failUnknownLogicalTypes,
            dbgDisplayFields);
    }
}
