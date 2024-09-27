using Microsoft.CodeAnalysis.Diagnostics;

namespace Contrib.Avro.Codegen;

public sealed record AvroGenOptions(
    IReadOnlyDictionary<string, string> NamespaceMapping,
    IReadOnlyDictionary<string, string> LogicalTypes,
    bool GenerateRequiredFields)
{
    public static AvroGenOptions Create(AnalyzerConfigOptions options)
    {
        var namespaceMapping = options.GetMsBuildProperty("NamespaceMapping")?.AsDictionary() ?? [];

        var genRequired = !bool.TryParse(options.GetMsBuildProperty("GenerateRequiredFields"), out var value) || value;

        var logicalTypes = options.GetMsBuildProperty("LogicalTypes")?.AsDictionary() ?? [];

        return new AvroGenOptions(namespaceMapping, logicalTypes, genRequired);
    }
}
