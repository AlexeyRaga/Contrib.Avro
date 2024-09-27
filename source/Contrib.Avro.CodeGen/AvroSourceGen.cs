using Avro;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace Contrib.Avro.Codegen;

[Generator]
public sealed class AvroSourceGen : IIncrementalGenerator
{
    private sealed record AvroField(Field Field, string Name, AvroFieldType Type);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var schemaResults = context.AdditionalTextsProvider
            .Where(static x => Path.GetExtension(x.Path).Equals(".avsc", StringComparison.OrdinalIgnoreCase))
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select((x, _) => (Item: x.Left, Options: AvroGenOptions.Create(x.Right.GetOptions(x.Left))))
            .Select((x, t) => SchemaParser.ParseSchema(x.Item, x.Options, t))
            .SelectMany((x, _) => x switch
            {
                SchemaResult.Failure f => [f],
                SchemaResult.Success s =>
                    SchemaUtils.ExtractSchemas(s.Source.Value)
                        .Select(SchemaResult (z) => s with { Source = s.Source with { Value = z } }),
                _ => throw new InvalidOperationException("Impossible happened")
            });

        context.RegisterSourceOutput(schemaResults, GenerateSchemaCode);
    }

    private void GenerateSchemaCode(SourceProductionContext ctx, SchemaResult value)
    {
        if (!value.GetValue(ctx, out var item)) return;
        switch (item.Value)
        {
            case EnumSchema s:
                GenerateEnumCode(ctx, s);
                break;

            case FixedSchema s:
                GenerateFixedCode(ctx, s);
                break;

            case RecordSchema s:
                GenerateRecordCode(ctx, item.Select(_ => s));
                break;
        }
    }

    private void GenerateRecordCode(SourceProductionContext ctx, GeneratorItem<RecordSchema> item)
    {
        var schema = item.Value;
        var name = CodeGenUtil.Instance.Mangle(schema.Name);

        var fields = schema.Fields
            .Select(x => new AvroField(x, CodeGenUtil.Instance.Mangle(x.Name), GetAvroType(x.Schema, false)))
            .ToList();

        ctx.AddSource($"{schema.Fullname}.g.cs", $@"
{BuildFileHeader(schema)}

{BuildTypeDocumentation(schema)}
public partial class {name}() : global::Avro.Specific.ISpecificRecord, global::Contrib.Avro.IHaveAvroSchema 
{{
    public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse({JsonConvert.ToString(schema.ToString())});
    public virtual global::Avro.Schema Schema => {name}._SCHEMA;

    static global::Avro.Schema global::Contrib.Avro.IHaveAvroSchema.Schema => {name}._SCHEMA;

    {IndentedStringBuilder
        .New(initialIndentationLevel: 1)
        .AppendMany(fields, x => BuildRecordField(x, item.Options))
        .ToString()
        .Trim()}

    {BuildRecordGet(fields)}

    {BuildRecordPut(fields)}
}}
");
    }

    private void GenerateFixedCode(SourceProductionContext ctx, FixedSchema schema)
    {
        var name = CodeGenUtil.Instance.Mangle(schema.Name);

        ctx.AddSource($"{schema.Fullname}.g.cs", $@"
{BuildFileHeader(schema)}

{BuildTypeDocumentation(schema)}
public partial class {name} : global::Avro.Specific.SpecificFixed, global::Contrib.Avro.IHaveAvroSchema
{{
    public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse({JsonConvert.ToString(schema.ToString())});
    public override global::Avro.Schema Schema {{ get {{ return {name}._SCHEMA; }} }}

    static global::Avro.Schema global::Contrib.Avro.IHaveAvroSchema.Schema => {name}._SCHEMA;

    public {name}() : base({schema.Size}) {{ }}

    public static uint FixedSize => {schema.Size};
}}
");
    }

    private void GenerateEnumCode(SourceProductionContext ctx, EnumSchema schema)
    {
        var name = CodeGenUtil.Instance.Mangle(schema.Name);

        var members = schema.Symbols.Select((x, i) => $"{x} = {i}");

        ctx.AddSource($"{schema.Fullname}.g.cs", $@"
using System;

namespace {schema.Namespace};

{BuildTypeDocumentation(schema)}
public enum {name} 
{{
    {string.Join($", \n    ", members)}
}}");
    }

    private static AvroFieldType GetAvroType(Schema schema, bool nullable)
    {
        switch (schema.Tag)
        {
            case Schema.Type.Null:
                return new AvroFieldType(typeof(object).ToString(), Schema: schema, Nullable: nullable);

            case Schema.Type.Boolean:
                return new AvroFieldType(typeof(bool).ToString(), Schema: schema, Nullable: nullable);

            case Schema.Type.Int:
                return new AvroFieldType(typeof(int).ToString(), Schema: schema, Nullable: nullable);

            case Schema.Type.Long:
                return new AvroFieldType(typeof(long).ToString(), Schema: schema, Nullable: nullable);

            case Schema.Type.Float:
                return new AvroFieldType(typeof(float).ToString(), Schema: schema, Nullable: nullable);

            case Schema.Type.Double:
                return new AvroFieldType(typeof(double).ToString(), Schema: schema, Nullable: nullable);

            case Schema.Type.Bytes:
                return new AvroFieldType(typeof(byte[]).ToString(), Schema: schema, Nullable: nullable);

            case Schema.Type.String:
                return new AvroFieldType(typeof(string).ToString(), Schema: schema, Nullable: nullable);

            case Schema.Type.Enumeration:
                return new AvroFieldType(CodeGenUtil.Instance.Mangle(schema.Fullname), Schema: schema,
                    Nullable: nullable);

            case Schema.Type.Fixed:
            case Schema.Type.Record:
            case Schema.Type.Error:
                return new AvroFieldType(CodeGenUtil.Instance.Mangle(schema.Fullname), Schema: schema,
                    Nullable: nullable);

            case Schema.Type.Array:
                var arraySchema = (ArraySchema)schema;

                var itemType = GetAvroType(arraySchema.ItemSchema, false);
                var arrayType = $"IList<{itemType.Type}>";
                var unwrapper = itemType.Unwrapper is not null ? $".Select(x => x{itemType.Unwrapper}).ToList()" : "";
                Func<string, string>? wrapper =
                    itemType.Wrapper is not null
                        ? v => $"((IList<System.Object>){v}).Select(x => {itemType.Wrapper("x")}).ToList()"
                        : null;

                return new AvroFieldType(
                    arrayType,
                    Schema: schema,
                    Unwrapper: unwrapper,
                    Wrapper: wrapper,
                    Nullable: nullable);

            case Schema.Type.Map:
                var mapSchema = (MapSchema)schema;
                return new AvroFieldType("IDictionary<string," + GetAvroType(mapSchema.ValueSchema, false).Type + ">",
                    Schema: schema);

            case Schema.Type.Union:
                var unionSchema = (UnionSchema)schema;

                var isNullable = unionSchema.Schemas[0].Tag == Schema.Type.Null;
                var schemas = isNullable ? unionSchema.Schemas.Skip(1).ToList() : unionSchema.Schemas.ToList();

                if (isNullable && schemas.Count == 1)
                {
                    return GetAvroType(schemas[0], true);
                }

                var choiceTypes = schemas.Select(x => GetAvroType(x, false).Type);
                var choiceType = $"Contrib.Avro.Choice<{string.Join(", ", choiceTypes)}>";

                return new AvroFieldType(choiceType,
                    Schema: schema,
                    Nullable: isNullable,
                    Unwrapper: ".Unwrap()",
                    Wrapper: x => $"{choiceType}.Wrap({x})");

            case Schema.Type.Logical:
                var logicalSchema = (LogicalSchema)schema;

                switch (logicalSchema.LogicalType)
                {
                    case RegisteredLogicalType { DotnetTypeHint: var dotnetType }:
                        return new AvroFieldType(dotnetType, Schema: schema, nullable);
                    case UnknownLogicalType _:
                        return GetAvroType(logicalSchema.BaseSchema, nullable);
                    default:
                        var csharpType = logicalSchema.LogicalType.GetCSharpType(nullable);
                        return csharpType.IsGenericType && csharpType.GetGenericTypeDefinition() == typeof(Nullable<>)
                            ? new AvroFieldType(csharpType.GetGenericArguments()[0].ToString(), Schema: schema,
                                Nullable: true)
                            : new AvroFieldType(csharpType.ToString(), Schema: schema);
                }

        }

        throw new CodeGenException("Unable to generate CodeTypeReference for " + schema.Name + " type " + schema.Tag);
    }


    private static string BuildRecordField(AvroField field, AvroGenOptions options)
    {
        return $@"{BuildFieldDocumentation(field.Field)}
public {(field.Type.Nullable || !options.GenerateRequiredFields ? "" : "required ")}{field.Type.FullType} {field.Name} {{ get; set; }}";
    }

    private static string BuildRecordGet(List<AvroField> fields)
    {
        var unwrapper = (AvroFieldType t) => t.Unwrapper is null
            ? string.Empty
            : t.Nullable
                ? "?" + t.Unwrapper
                : t.Unwrapper;

        return IndentedStringBuilder
            .New(initialIndentationLevel: 1)
            .AppendLine($"public object Get(int fieldPos) => fieldPos switch")
            .AppendLine("{").IncreaseIndentation()
            .AppendMany(fields, x => $"{x.Field.Pos} => this.{x.Name}{unwrapper(x.Type)},")
            .AppendLine("""_ => throw new AvroRuntimeException("Bad index " + fieldPos.ToString() + " in Get()")""")
            .DecreaseIndentation()
            .AppendLine("};")
            .ToString().Trim();
    }

    private static string BuildRecordPut(List<AvroField> fields) =>
        IndentedStringBuilder
            .New(initialIndentationLevel: 1)
            .AppendLine("public virtual void Put(int fieldPos, object fieldValue)")
            .StartBlock("{", "}")
            .AppendLine("switch (fieldPos)")
            .StartBlock("{", "}")
            .AppendMany(fields, x =>
            {
                // var prefix = x.Type.Nullable ? "fieldValue is null ? null : " : "";
                // var w = x.Type.Wrapper?.Invoke("fieldValue") ?? $"({x.Type.Type})fieldValue";
                // return $"case {x.Field.Pos}: this.{x.Name} = {prefix}{w}; break;";
                switch (x.Type.Schema.Tag)
                {
                    case Schema.Type.Enumeration when x.Type.Nullable:
                    case Schema.Type.Union when x.Type.Nullable:
                        var wrapped = x.Type.Wrapper?.Invoke("fieldValue") ?? $"({x.Type.Type})fieldValue";
                        return $"case {x.Field.Pos}: this.{x.Name} = fieldValue is null ? null : {wrapped}; break;";
                    default:
                        var wrappedValue = x.Type.Wrapper?.Invoke("fieldValue") ?? $"({x.Type.FullType})fieldValue";
                        return $"case {x.Field.Pos}: this.{x.Name} = {wrappedValue}; break;";
                }
            })
            .AppendLine(
                """default: throw new AvroRuntimeException("Bad index " + fieldPos.ToString() + " in Put()");""")
            .EndAllBlocks()
            .ToString()
            .Trim();

    public static string BuildFileHeader(NamedSchema schema) =>
        $@"
using System;
using System.Collections.Generic;
using System.Text;
using global::Avro;
using global::Avro.Specific;

namespace {schema.Namespace};
".Trim();

    private static string BuildTypeDocumentation(NamedSchema schema)
    {
        return $@"
/// <summary>
/// {schema.Documentation}
/// </summary>
/// <remarks>
/// This class is generated for {schema.Fullname}
/// </remarks>";
    }

    private static string BuildFieldDocumentation(Field field)
    {
        return $@"
/// <summary>
/// {field.Documentation}
/// </summary>";
    }
}
