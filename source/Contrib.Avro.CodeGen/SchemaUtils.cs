using System;
using System.Collections.Generic;
using System.Linq;
using Avro;
using Newtonsoft.Json.Linq;

namespace Contrib.Avro.CodeGen;

public static class SchemaUtils
{
    public static IList<Schema> ExtractSchemas(Schema schema)
    {
        var result = new Dictionary<string, Schema>();
        ExtractSchemas(schema, result);
        return result.Values.ToList().AsReadOnly();
    }

    private static void ExtractSchemas(Schema schema, Dictionary<string, Schema> result)
    {
        switch (schema)
        {
            case FixedSchema:
            case EnumSchema:
                result.TryAdd(schema.Fullname, schema);
                break;
            case RecordSchema s:
                if (result.TryAdd(schema.Fullname, schema))
                    s.Fields.ForEach(f => ExtractSchemas(f.Schema, result));
                break;
            case ArraySchema s:
                ExtractSchemas(s.ItemSchema, result);
                break;
            case MapSchema s:
                ExtractSchemas(s.ValueSchema, result);
                break;
            case UnionSchema s:
                foreach (var item in s.Schemas) ExtractSchemas(item, result);
                break;
            // Primitive types are ignored
            default:
                break;
        }
    }

    public static JToken ReplaceNamespace(JToken schemaJson, IReadOnlyDictionary<string, string> namespaceMap)
    {
        if (namespaceMap.Count == 0) return schemaJson;
        var schemas = schemaJson switch
        {
            JArray x => x.Cast<JObject>(),
            JObject obj => [obj],
            _ => throw new InvalidOperationException("Invalid schema")
        };

        foreach (var s in schemas) ReplaceNamespaceImpl(s, namespaceMap);

        return schemaJson;
    }

    private static void ReplaceNamespaceImpl(JObject schemaJson, IReadOnlyDictionary<string, string> namespaceMap)
    {
        if (schemaJson.TryGetValue("namespace", out var ns))
        {
            var currentNamespace = ns.ToString();
            foreach (var (oldNamespace, newNamespace) in namespaceMap)
            {
                if (currentNamespace == oldNamespace)
                {
                    schemaJson["namespace"] = newNamespace;
                    break;
                }

                if (currentNamespace.StartsWith(oldNamespace + "."))
                {
                    schemaJson["namespace"] = currentNamespace.Replace(oldNamespace + ".", newNamespace + ".");
                    break;
                }
            }
        }

        // Handle fields that could contain records with namespaces
        if (!schemaJson.TryGetValue("fields", out var fields)) return;

        foreach (var field in fields)
        {
            if (field["type"] is JObject fieldType)
            {
                ReplaceNamespace(fieldType, namespaceMap);
            }
            else if (field["type"]?.Type == JTokenType.String)
            {
                // Handle references to fully qualified types
                var typeName = field["type"]?.ToString();
                if (typeName == null) continue;
                foreach (var (oldNamespace, newNamespace) in namespaceMap)
                {
                    if (!typeName.StartsWith(oldNamespace + ".")) continue;
                    field["type"] = typeName.Replace(oldNamespace + ".", newNamespace + ".");
                    break;
                }
            }
        }
    }
}
