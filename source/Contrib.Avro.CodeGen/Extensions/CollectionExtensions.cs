namespace Contrib.Avro.Codegen;

public static class CollectionExtensions
{
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        this Dictionary<TKey, TValue> source,
        IEnumerable<KeyValuePair<TKey, TValue>> other) where TKey : notnull
    {
        var result = new Dictionary<TKey, TValue>(source);
        foreach (var (key, value) in other) result[key] = value;
        return result;
    }

    public static IReadOnlyDictionary<TKey, TValue> Merge<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> source,
        IEnumerable<KeyValuePair<TKey, TValue>> other) where TKey : notnull
    {
        if (other == null) throw new Exception("NILL");
        var result = new Dictionary<TKey, TValue>(source);
        foreach (var (key, value) in other) result[key] = value;
        return result.AsReadOnly();
    }
}
