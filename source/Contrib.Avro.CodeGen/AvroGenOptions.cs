using Microsoft.CodeAnalysis.Diagnostics;

namespace Contrib.Avro.Codegen;

public sealed record AvroGenOptions(
    IReadOnlyDictionary<string, string> NamespaceMapping,
    IReadOnlyDictionary<string, string> LogicalTypes,
    bool GenerateRequiredFields)
{
    public static AvroGenOptions Create(AnalyzerConfigOptions options)
    {
        var genRequired = !bool.TryParse(options.GetMsBuildProperty("GenerateRequiredFields"), out var value) || value;

        var namespaceMapping = options.GetMsBuildDictionary("NamespaceMapping");
        var logicalTypes = options.GetMsBuildDictionary("LogicalTypes");

        return new AvroGenOptions(namespaceMapping, logicalTypes, genRequired);
    }
}
