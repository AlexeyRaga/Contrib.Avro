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
        var defaultOpts = opts.GetValueDefault($"build_metadata.AdditionalFiles.{name}")?
            .AsDictionary(keyValueDelimiter, entryDelimiter) ?? [];

        var itemOpts = opts.GetValueDefault($"build_property.Avro_{name}")?
            .AsDictionary(keyValueDelimiter, entryDelimiter) ?? [];

        foreach (var (k, v) in itemOpts) defaultOpts[k] = v;

        return defaultOpts;
    }

    public static bool GetMsBuildBoolean(this AnalyzerConfigOptions opts, string name, bool defaultValue = false) => bool.TryParse(opts.GetMsBuildProperty(name), out var value) ? value : defaultValue;

    public static T GetMsBuildEnum<T>(this AnalyzerConfigOptions opts, string name, T defaultValue = default) where T : struct =>
        Enum.TryParse<T>(opts.GetMsBuildProperty(name), true, out var value) ? value : defaultValue;

    public static string? GetMsBuildProperty(this AnalyzerConfigOptions opts, string name)
    {
        if ((opts.TryGetValue($"build_metadata.AdditionalFiles.{name}", out var v) && !string.IsNullOrWhiteSpace(v))
            || opts.TryGetValue($"build_property.Avro_{name}", out v) && !string.IsNullOrWhiteSpace(v))
        {
            return v;
        }

        return null;
    }

    private static string? GetValueDefault(this AnalyzerConfigOptions opts, string fullName) =>
        opts.TryGetValue(fullName, out var v) && !string.IsNullOrWhiteSpace(v) ? v : null;

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
