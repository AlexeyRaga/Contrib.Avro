using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Contrib.Avro.Codegen;

public static class CodeAnalysisExtensions
{
    public static Dictionary<string, string> GetMsBuildDictionary(
        this AnalyzerConfigOptions opts,
        string name,
        string keyValueDelimiter = ":",
        string entryDelimiter = ",")
    {
        var itemOpts = opts.GetValueOrDefault($"build_metadata.AdditionalFiles.{name}")?
            .AsDictionary(keyValueDelimiter, entryDelimiter) ?? [];

        var defaultOpts = opts.GetValueOrDefault($"build_property.Avro_{name}")?
            .AsDictionary(keyValueDelimiter, entryDelimiter) ?? [];

        foreach (var (k, v) in itemOpts) defaultOpts[k] = v;

        return defaultOpts;
    }

    public static Optional<bool> GetMsBuildBoolean(this AnalyzerConfigOptions opts, string name) =>
        bool.TryParse(opts.GetMsBuildPropertyOrNull(name), out var value) ? value : new Optional<bool>();

    public static Optional<T> GetMsBuildEnum<T>(this AnalyzerConfigOptions opts, string name) where T : struct =>
        Enum.TryParse<T>(opts.GetMsBuildPropertyOrNull(name), true, out var value) ? value : new Optional<T>();

    public static string? GetMsBuildPropertyOrNull(this AnalyzerConfigOptions opts, string name)
    {
        if ((opts.TryGetValue($"build_metadata.AdditionalFiles.{name}", out var v) && !string.IsNullOrWhiteSpace(v))
            || opts.TryGetValue($"build_property.Avro_{name}", out v) && !string.IsNullOrWhiteSpace(v))
        {
            return v;
        }

        return null;
    }

    public static Optional<string> GetMsBuildProperty(this AnalyzerConfigOptions opts, string name)
    {
        if ((opts.TryGetValue($"build_metadata.AdditionalFiles.{name}", out var v) && !string.IsNullOrWhiteSpace(v))
            || opts.TryGetValue($"build_property.Avro_{name}", out v) && !string.IsNullOrWhiteSpace(v))
        {
            return v;
        }

        return new Optional<string>();
    }

    private static string? GetValueOrDefault(this AnalyzerConfigOptions opts, string fullName) =>
        opts.TryGetValue(fullName, out var v) && !string.IsNullOrWhiteSpace(v) ? v : null;

    private static Optional<string> GetValue(this AnalyzerConfigOptions opts, string fullName) =>
        opts.TryGetValue(fullName, out var v) && !string.IsNullOrWhiteSpace(v) ? v : new Optional<string>();

    private static Dictionary<string, string> AsDictionary(
        this string value,
        string keyValueDelimiter = ":",
        string entryDelimiter = ",")
    {
        return value
            .Split(entryDelimiter, StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Split(keyValueDelimiter, 2, StringSplitOptions.TrimEntries))
            .Where(x => x.Length == 2)
            .ToDictionary(x => x[0], x => x[1]);
    }
}
