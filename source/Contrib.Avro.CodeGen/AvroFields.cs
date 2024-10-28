using Avro;

namespace Contrib.Avro.Codegen;

public static class AvroFields
{
    public static AvroFieldType GetAvroType(AvroGenOptions options, Schema schema, bool nullable)
    {
        var fieldType = schema.Tag switch
        {
            Schema.Type.Null => new AvroFieldType(typeof(object).ToString(), Schema: schema, nullable),
            Schema.Type.Boolean => new AvroFieldType(typeof(bool).ToString(), Schema: schema, nullable),
            Schema.Type.Int => new AvroFieldType(typeof(int).ToString(), Schema: schema, nullable),
            Schema.Type.Long => new AvroFieldType(typeof(long).ToString(), Schema: schema, nullable),
            Schema.Type.Float => new AvroFieldType(typeof(float).ToString(), Schema: schema, nullable),
            Schema.Type.Double => new AvroFieldType(typeof(double).ToString(), Schema: schema, nullable),
            Schema.Type.Bytes => new AvroFieldType(typeof(byte[]).ToString(), Schema: schema, nullable),
            Schema.Type.String => new AvroFieldType(typeof(string).ToString(), Schema: schema, nullable),
            Schema.Type.Enumeration =>
                new AvroFieldType(CodeGenUtil.Instance.Mangle(schema.Fullname), Schema: schema, nullable),
            Schema.Type.Fixed or Schema.Type.Record or Schema.Type.Error =>
                new AvroFieldType(CodeGenUtil.Instance.Mangle(schema.Fullname), Schema: schema, nullable),
            Schema.Type.Array => GetArrayFieldType(options, (ArraySchema)schema, nullable),
            Schema.Type.Map => GetAvroMapFieldType(options, (MapSchema)schema),
            Schema.Type.Union => GetAvroUnionFieldType(options, (UnionSchema)schema),
            Schema.Type.Logical => GetLogicalFieldType(options, (LogicalSchema)schema, nullable),
            _ => throw new CodeGenException($"Unable to generate CodeTypeReference for {schema.Name} type {schema.Tag}")
        };

        var hint = schema.GetProperty(options.TypeOptions.TypeHintPropertyName)?.Trim('"');
        if (hint is not null && options.TypeOptions.TypeMappings.TryGetValue(hint, out var hintedType))
        {
            return fieldType with
            {
                Type = hintedType,
                Wrapper = x => GetConvertFromClause(hintedType, x, nullable),
                Unwrapper = x => GetConvertToClause(fieldType.Type, hintedType, x, nullable)
            };
        }

        return fieldType;
    }

    private static AvroFieldType GetAvroUnionFieldType(AvroGenOptions options, UnionSchema schema)
    {
        var isNullable = schema.Schemas[0].Tag == Schema.Type.Null;
        var schemas = isNullable ? schema.Schemas.Skip(1).ToList() : schema.Schemas.ToList();

        if (isNullable && schemas.Count == 1)
        {
            return GetAvroType(options, schemas[0], true);
        }

        var choiceTypes = schemas.Select(x => GetAvroType(options, x, false)).ToList();
        var choiceType = $"Contrib.Avro.Choice<{string.Join(", ", choiceTypes.Select(x => x.Type))}>";

        var choiceUnwrapper = (string what) => new IndentedStringBuilder(initialIndentationLevel: 1)
            .AppendLine($"{what} switch")
            .StartBlock("{", "}")
            .AppendLine("null => null,")
            .AppendMany(choiceTypes,
                (x, i) =>
                {
                    var returnValue = x.Unwrapper?.Invoke("x") ?? "x";
                    return
                        $"{choiceType}.Choice{i + 1}Of{choiceTypes.Count}({x.Type} x) => (object){returnValue},";
                })
            .AppendLine($"_ => throw new AvroRuntimeException(\"Bad choice For {choiceType}\")")
            .EndAllBlocks()
            .ToString()
            .Trim();

        var choiceWrapper = (string what) => new IndentedStringBuilder(initialIndentationLevel: 1)
            .AppendLine($"{what} switch")
            .StartBlock("{", "}")
            .AppendLine("null => null,")
            .AppendMany(choiceTypes, (x, i) =>
            {
                var returnValue = x.Wrapper?.Invoke("x") ?? $"x";
                return $"{x.BaseType} x => {choiceType}.FromValue({returnValue}),";
            })
            .AppendLine($"_ => throw new AvroRuntimeException(\"Bad choice For {choiceType}\")")
            .EndAllBlocks()
            .ToString()
            .Trim();

        return new AvroFieldType(choiceType,
            BaseType: "object",
            Schema: schema,
            Nullable: isNullable,
            Unwrapper: x => choiceUnwrapper(x),
            Wrapper: x => choiceWrapper(x));
    }

    private static AvroFieldType GetAvroMapFieldType(AvroGenOptions options, MapSchema schema)
    {
        var valueType = GetAvroType(options, schema.ValueSchema, false);

        Func<string, string>? valueUnwrapper =
            valueType.Unwrapper is not null
                ? x => $"{x}?.ToDictionary(k => k.Key, v => {valueType.Unwrapper("v.Value")})"
                : null;

        Func<string, string>? valueWrapper =
            valueType.Wrapper is not null
                ? v =>
                    $"((IDictionary<string, {valueType.FullBaseType}>){v}).ToDictionary(k => k.Key, v => {valueType.Wrapper("v.Value")})"
                : null;

        return new AvroFieldType(
            "IDictionary<string," + valueType.FullType + ">",
            BaseType: $"IDictionary<string, object>",
            Schema: schema,
            Wrapper: valueWrapper,
            Unwrapper: valueUnwrapper,
            Nullable: true);
    }

    private static AvroFieldType GetArrayFieldType(AvroGenOptions options, ArraySchema schema, bool nullable)
    {
        var itemType = GetAvroType(options, schema.ItemSchema, false);
        var arrayType = $"IList<{itemType.FullType}>";

        Func<string, string>? itemUnwrapper =
            itemType.Unwrapper is not null
                ? x => $"{x}?.Select(item => {itemType.Unwrapper("item")}).ToList()"
                : null;

        Func<string, string>? itemWrapper =
            itemType.Wrapper is not null
                ? v =>
                    $"((IEnumerable<{itemType.FullBaseType}>){v}).Select(item => {itemType.Wrapper("item")}).ToList()"
                : null;

        return new AvroFieldType(
            arrayType,
            BaseType: "IList<object>",
            Schema: schema,
            Unwrapper: itemUnwrapper,
            Wrapper: itemWrapper,
            Nullable: nullable);
    }

    private static AvroFieldType GetLogicalFieldType(AvroGenOptions options, LogicalSchema schema, bool nullable) =>
        schema.LogicalType switch
        {
            RegisteredLogicalType { DotnetTypeHint: var dotnetType } => new AvroFieldType(dotnetType, schema, nullable),
            UnknownLogicalType _ => GetAvroType(options, schema.BaseSchema, nullable),
            _ => GetDefaultLogicalType(schema, nullable)
        };

    private static string GetConvertFromClause(string type, string term, bool nullable)
    {
        var castType = nullable ? type + "?" : type;
        var convert =
            $"({castType})System.ComponentModel.TypeDescriptor.GetConverter(typeof({type})).ConvertFrom({term})";
        return nullable ? $"{term} is null ? null : {convert}" : convert;
    }

    private static string GetConvertToClause(string fromType, string toType, string term, bool nullable)
    {
        var convert =
            $"System.ComponentModel.TypeDescriptor.GetConverter(typeof({toType})).ConvertTo({term}, typeof({fromType}))";
        return nullable ? $"{term} is null ? null : {convert}" : convert;
    }

    private static AvroFieldType GetDefaultLogicalType(LogicalSchema logicalSchema, bool nullable)
    {
        var csharpType = logicalSchema.LogicalType.GetCSharpType(nullable);
        return csharpType.IsGenericType && csharpType.GetGenericTypeDefinition() == typeof(Nullable<>)
            ? new AvroFieldType(csharpType.GetGenericArguments()[0].ToString(), Schema: logicalSchema,
                Nullable: true)
            : new AvroFieldType(csharpType.ToString(), Schema: logicalSchema);
    }
}
