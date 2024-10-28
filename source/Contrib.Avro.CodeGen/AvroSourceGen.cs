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
        var jsonConfig = context.AdditionalTextsProvider
            .Where(x => Path.GetFileName(x.Path).Equals("avrogen.config.json", StringComparison.OrdinalIgnoreCase))
            .Select((x, t) => x.GetText(t))
            .Collect()
            .Select((items, _) => items
                .Where(t => t is not null)
                .Select(t => AvroGenOptionsConfig.FromJson(t!.ToString()))
                .FirstOrDefault(AvroGenOptionsConfig.Default));

        var schemaResults = context.AdditionalTextsProvider
            .Where(static x => Path.GetExtension(x.Path).Equals(".avsc", StringComparison.OrdinalIgnoreCase))
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Combine(jsonConfig)
            .Select((x, _) => (Item: x.Left.Left, Config: x.Left.Right, JsonConfigs: x.Right))
            .Select((x, _) => (
                Item: x.Item,
                Options: AvroGenOptionsConfig.FromAnalyzerConfig(x.Config.GetOptions(x.Item))
                    .Combine(x.JsonConfigs)
                    .ToOptions()))
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
        var classOrRecord = item.Options.GenerateRecords ? "record" : "class";

        var fields = schema.Fields
            .Select(x =>
                new AvroField(x, CodeGenUtil.Instance.Mangle(x.Name),
                    AvroFields.GetAvroType(item.Options, x.Schema, false)))
            .ToList();

        ctx.AddSource($"{schema.Fullname}.g.cs", $@"
{BuildFileHeader(schema)}

{BuildTypeDocumentation(schema)}
{BuildDebuggerDisplay(name, fields, item.Options.DebuggerDisplayFields)}
public partial {classOrRecord} {name}() : global::Avro.Specific.ISpecificRecord, global::Contrib.Avro.IHaveAvroSchema 
{{
    public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse({JsonConvert.ToString(schema.ToString())});
    global::Avro.Schema global::Avro.Specific.ISpecificRecord.Schema => {name}._SCHEMA;

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
[System.Diagnostics.DebuggerDisplay(""{name}({{GetDebuggerDisplay(),nq}})"")]
public partial class {name} : global::Avro.Specific.SpecificFixed, global::Contrib.Avro.IHaveAvroSchema
{{
    public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse({JsonConvert.ToString(schema.ToString())});
    public override global::Avro.Schema Schema {{ get {{ return {name}._SCHEMA; }} }}
    static global::Avro.Schema global::Contrib.Avro.IHaveAvroSchema.Schema => {name}._SCHEMA;

    public static uint FixedSize => {schema.Size};

    public {name}() : base({schema.Size}) {{ }}

    public {name}(IEnumerable<byte> value) : this() 
    {{
        Value = value.ToArray(); 
    }}

    public static bool operator ==({name} left, {name} right) =>
        ReferenceEquals(left, null) || left.Equals(right);

    public static bool operator !=({name} left, {name} right) => 
        !(left == right);

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object obj) => base.Equals(obj);

    private string GetDebuggerDisplay() =>
        $""[{{string.Join("", "", Value.Select(b => b.ToString()))}}]"";
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


    private static string BuildRecordField(AvroField field, AvroGenOptions options)
    {
        return $@"{BuildFieldDocumentation(field.Field)}
public {(field.Type.Nullable || !options.GenerateRequiredFields ? "" : "required ")}{field.Type.FullType} {field.Name} {{ get; set; }}";
    }

    private static string BuildRecordGet(List<AvroField> fields)
    {
        var unwrapper = (AvroField t) => t.Type.Unwrapper is null
            ? $"this.{t.Name}"
            : t.Type.Nullable
                ? $"this.{t.Name} is null ? null : " + t.Type.Unwrapper($"this.{t.Name}")
                : t.Type.Unwrapper($"this.{t.Name}");

        return IndentedStringBuilder
            .New(initialIndentationLevel: 1)
            .AppendLine($"public object Get(int fieldPos) => fieldPos switch")
            .AppendLine("{").IncreaseIndentation()
            .AppendMany(fields, x => $"{x.Field.Pos} => {unwrapper(x)},")
            .AppendLine("""_ => throw new AvroRuntimeException("Bad index " + fieldPos.ToString() + " in Get()")""")
            .DecreaseIndentation()
            .AppendLine("};")
            .ToString().Trim();
    }

    private static string BuildRecordPut(List<AvroField> fields)
    {
        var wrapper = (AvroField t) => t.Type.Wrapper is null
            ? $"({t.Type.FullType})fieldValue"
            : t.Type.Nullable
                ? $"fieldValue is null ? null : {t.Type.Wrapper("fieldValue")}"
                : t.Type.Wrapper("fieldValue");

        return IndentedStringBuilder
            .New(initialIndentationLevel: 1)
            .AppendLine("public virtual void Put(int fieldPos, object fieldValue)")
            .StartBlock("{", "}")
            .AppendLine("switch (fieldPos)")
            .StartBlock("{", "}")
            .AppendMany(fields, x =>
            {
                switch (x.Type.Schema.Tag)
                {
                    case Schema.Type.Enumeration when x.Type.Nullable:
                    case Schema.Type.Union when x.Type.Nullable:
                        var wrapped = x.Type.Wrapper?.Invoke("fieldValue") ?? $"({x.Type.Type})fieldValue";
                        return $"case {x.Field.Pos}: this.{x.Name} = fieldValue is null ? null : {wrapped}; break;";
                    default:
                        return $"case {x.Field.Pos}: this.{x.Name} = {wrapper(x)}; break;";
                }
            })
            .AppendLine(
                """default: throw new AvroRuntimeException("Bad index " + fieldPos.ToString() + " in Put()");""")
            .EndAllBlocks()
            .ToString()
            .Trim();
    }

    private static string BuildDebuggerDisplay(string typeName, List<AvroField> fields,
        DebuggerDisplayFields displayFields)
    {
        if (displayFields == DebuggerDisplayFields.None) return string.Empty;

        var fieldsToDisplay = displayFields switch
        {
            DebuggerDisplayFields.All => fields,
            DebuggerDisplayFields.Required => fields.Where(x => !x.Type.Nullable).ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(displayFields), displayFields, null)
        };

        var fieldsString = string.Join(", ", fieldsToDisplay.Select(x => $"{x.Name} = {{{x.Name}}}"));
        return $@"[System.Diagnostics.DebuggerDisplay(""{typeName}({fieldsString})"")]";
    }

    public static string BuildFileHeader(NamedSchema schema) =>
        $@"
#pragma warning disable CS8669

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
