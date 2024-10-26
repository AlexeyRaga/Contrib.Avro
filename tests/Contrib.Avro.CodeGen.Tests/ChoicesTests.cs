using FluentAssertions;
using Hedgehog;
using Hedgehog.Linq;
using Hedgehog.Xunit;
using Gen = Hedgehog.Linq.Gen;
using Range = Hedgehog.Linq.Range;

namespace Contrib.Avro.CodeGen.Tests;

[Properties(AutoGenConfig = typeof(Generators))]
public sealed class ChoicesTests
{
    private static class Generators
    {
        private static Gen<Choice<int, string>> ChoiceIntString =>
            G.ChoiceGen(G.Int32, G.String);

        private static Gen<MessageWithChoices> Message =>
            from union in ChoiceIntString
            from unionInArray in ChoiceIntString.Array(Range.Constant(1, 5))
            from unionInMap in ChoiceIntString.Dictionary()
            from unionNullable in ChoiceIntString.WithNull()
            from unionNullableInArray in ChoiceIntString.WithNull().Array(Range.Constant(1, 5))
            from unionNullableInMap in ChoiceIntString.WithNull().Dictionary()
            from unionInArrayInUnion in G.ChoiceGen(Gen.Bool, ChoiceIntString.IList())
            from unionNullableInArrayInUnion in G.ChoiceGen(Gen.Bool, ChoiceIntString.WithNull().IList())
            from unionInMapInUnion in G.ChoiceGen(Gen.Bool, ChoiceIntString.Dictionary())
            from unionNullableInMapInUnion in G.ChoiceGen(Gen.Bool, ChoiceIntString.WithNull().Dictionary())
            from nullableArray in G.Int32.Array(Range.Constant(1, 5)).WithNull()
            from nullableMap in G.Int32.Dictionary().WithNull()
            select new MessageWithChoices
            {
                Union = union,
                UnionInArray = unionInArray,
                UnionInMap = unionInMap,
                UnionNullable = unionNullable,
                UnionNullableInArray = unionNullableInArray,
                UnionNullableInMap = unionNullableInMap,
                UnionInArrayInUnion = unionInArrayInUnion,
                UnionNullableInArrayInUnion = unionNullableInArrayInUnion,
                UnionInMapInUnion = unionInMapInUnion,
                UnionNullableInMapInUnion = unionNullableInMapInUnion,
                NullableArray = nullableArray,
                NullableMap = nullableMap
            };

        public static AutoGenConfig Config =>
            GenX.defaults
                .WithGenerator(Message)
                .WithGenerator(ChoiceIntString);
    }

    [Property(tests: 500)]
    public void Should_roundtrip_avro_message(MessageWithChoices msg)
    {
        var bytes = msg.SerializeToBinary();
        var deserialized = AvroUtils.DeserializeFromBinary<MessageWithChoices>(bytes);
        deserialized.Should().BeEquivalentTo(msg);
    }
}
