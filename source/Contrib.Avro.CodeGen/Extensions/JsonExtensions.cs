using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace Contrib.Avro.Codegen;

internal static class JsonExtensions
{
    public static Optional<JToken> TraverseJson(this JToken root, IEnumerable<object> pathComponents) =>
        pathComponents.Aggregate(new Optional<JToken>(root), (currentOpt, component) =>
            currentOpt.Select(current =>
                component switch
                {
                    int index when current is JArray array && array.Count > index => array[index],
                    string key when current is JObject obj && obj.TryGetValue(key, out var value) => value,
                    _ => throw new Exception("Invalid path component")
                }
            ));

    public static void ForEach(this JArray array, Action<JToken> action)
    {
        foreach (var item in array) action(item);
    }
}
