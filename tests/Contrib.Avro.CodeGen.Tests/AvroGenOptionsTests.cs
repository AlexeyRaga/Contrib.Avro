using System.Text.Json;
using Contrib.Avro.Codegen;
using FluentAssertions;
using Hedgehog;
using Hedgehog.Linq;
using Hedgehog.Xunit;
using Gen = Hedgehog.Linq.Gen;

namespace Contrib.Avro.CodeGen.Tests;

[Properties(AutoGenConfig = typeof(Generators))]
public sealed class AvroGenOptionsTests
{
    [Property]
    public void Should_deserialise_from_json(AvroGenOptionsConfig config)
    {
        var serialised = JsonSerializer.Serialize(config, AvroGenJsonOptions.Default);

        serialised.Should().NotContain("HasValue");

        var deserialised = AvroGenOptionsConfig.FromJson(serialised);
        deserialised.Should().BeEquivalentTo(config);
    }

    [Property]
    public void Combine_should_work(AvroGenOptionsConfig a, AvroGenOptionsConfig b, AvroGenOptionsConfig c)
    {
        a.Combine(b).Combine(c).Should().BeEquivalentTo(a.Combine(b.Combine(c)));
    }

    [Property]
    public void Combine_with_default_should_not_change_result(AvroGenOptionsConfig a)
    {
        a.Combine(AvroGenOptionsConfig.Default).Should().BeEquivalentTo(a);
        AvroGenOptionsConfig.Default.Combine(a).Should().BeEquivalentTo(a);
    }
}

file static class Generators
{
    private static Gen<AvroTypeOptionsConfig> AvroTypeOptionsConfigGen =>
        from typeMappings in G.String.Dictionary()
        from typeHintPropertyName in GenX.auto<string>().Optional()
        from failUnknownLogicalTypes in Gen.Bool.Optional()
        select new AvroTypeOptionsConfig(
            typeMappings,
            typeHintPropertyName,
            failUnknownLogicalTypes);

    private static Gen<AvroGenOptionsConfig> AvroGenOptionsConfigGen =>
        from namespaceMapping in G.String.Dictionary()
        from typeOptions in AvroTypeOptionsConfigGen
        from generateRequiredFields in Gen.Bool.Optional()
        from generateRecords in Gen.Bool.Optional()
        from debuggerDisplayFields in GenX.auto<DebuggerDisplayFields>()
        select new AvroGenOptionsConfig(
            namespaceMapping,
            typeOptions,
            generateRequiredFields,
            generateRecords,
            debuggerDisplayFields);

    public static AutoGenConfig Config =>
        GenX.defaults
            .WithGenerator(AvroTypeOptionsConfigGen)
            .WithGenerator(AvroGenOptionsConfigGen);
}
