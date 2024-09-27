using Microsoft.CodeAnalysis.Diagnostics;

namespace Contrib.Avro.Codegen;

public sealed record AvroGenOptions(
    IReadOnlyDictionary<string, string> NamespaceMapping,
    IReadOnlyDictionary<string, string> LogicalTypes,
    bool GenerateRequiredFields,
    bool FailUnknownLogicalTypes)
{
    public static AvroGenOptions Create(AnalyzerConfigOptions options)
    {
        var genRequired = options.GetMsBuildBoolean("GenerateRequiredFields");
        var failUnknownLogicalTypes = options.GetMsBuildBoolean("FailUnknownLogicalTypes");

        var namespaceMapping = options.GetMsBuildDictionary("NamespaceMapping");
        var logicalTypes = options.GetMsBuildDictionary("LogicalTypes");


        return new AvroGenOptions(namespaceMapping, logicalTypes, genRequired, failUnknownLogicalTypes);
    }
}
