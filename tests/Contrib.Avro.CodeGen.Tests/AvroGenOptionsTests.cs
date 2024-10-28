using System.Text.Json;
using Contrib.Avro.CodeGen;
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

        deserialised.ToOptions().Should().BeEquivalentTo(config.ToOptions());
    }

    [Property]
    public void Combine_should_work(AvroGenOptionsConfig a, AvroGenOptionsConfig b, AvroGenOptionsConfig c)
    {
        var xs = a.Combine(b).Combine(c);
        var ys = a.Combine(b.Combine(c));

        xs.ToOptions().Should().BeEquivalentTo(ys.ToOptions());
    }

    [Property]
    public void Combine_with_default_should_not_change_result(AvroGenOptionsConfig a)
    {
        var expected = a.ToOptions();
        a.Combine(AvroGenOptionsConfig.Default).ToOptions().Should().BeEquivalentTo(expected);
        AvroGenOptionsConfig.Default.Combine(a).ToOptions().Should().BeEquivalentTo(expected);
    }
}

file static class Generators
{
    private static Gen<AvroGenOptionsConfig> AvroGenOptionsConfigGen =>
        from namespaceMapping in G.String.Dictionary()
        from generateRequiredFields in Gen.Bool.Optional()
        from generateRecords in Gen.Bool.Optional()
        from debuggerDisplayFields in GenX.auto<DebuggerDisplayFields>()
        from typeMappings in G.String.Dictionary().Optional()
        from typeHintPropertyName in GenX.auto<string>().Optional()
        from failUnknownLogicalTypes in Gen.Bool.Optional()
        select new AvroGenOptionsConfig(
            namespaceMapping,
            generateRequiredFields,
            generateRecords,
            debuggerDisplayFields,
            typeMappings,
            typeHintPropertyName,
            failUnknownLogicalTypes);

    public static AutoGenConfig Config =>
        GenX.defaults
            .WithGenerator(AvroGenOptionsConfigGen);
}
