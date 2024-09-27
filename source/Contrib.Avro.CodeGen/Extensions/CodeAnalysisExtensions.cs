using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Contrib.Avro.Codegen;

public static class CodeAnalysisExtensions
{
    public static string GetMsBuildProperty(
        this GeneratorExecutionContext context,
        string name,
        string defaultValue = "")
    {
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.Avro_{name}", out var value);
        return value ?? defaultValue;
    }

    public static string? GetMsBuildProperty(
        this AnalyzerConfigOptionsProvider provider,
        AdditionalText text,
        string name)
    {
        var opts = provider.GetOptions(text);
        if ((opts.TryGetValue($"build_metadata.AdditionalFiles.{name}", out var v) && !string.IsNullOrWhiteSpace(v))
            || opts.TryGetValue($"build_property.Avro_{name}", out v) && !string.IsNullOrWhiteSpace(v))
        {
            return v;
        }

        return null;
    }

    public static string? GetMsBuildProperty(this AnalyzerConfigOptions opts, string name)
    {
        if ((opts.TryGetValue($"build_metadata.AdditionalFiles.{name}", out var v) && !string.IsNullOrWhiteSpace(v))
            || opts.TryGetValue($"build_property.Avro_{name}", out v) && !string.IsNullOrWhiteSpace(v))
        {
            return v;
        }

        return null;
    }

    public static Dictionary<string, string> AsDictionary(this string value)
    {
        return value
            .Split(',', StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Split(':', 2, StringSplitOptions.TrimEntries))
            .Where(x => x.Length == 2)
            .ToDictionary(x => x[0], x => x[1]);
    }
}
