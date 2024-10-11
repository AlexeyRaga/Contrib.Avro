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
            from either in ChoiceIntString
            from eitherInArray in ChoiceIntString.Array(Range.Constant(1, 5))
            from eitherInMap in ChoiceIntString.Dictionary()
            from eitherNullable in ChoiceIntString.WithNull()
            from eitherNullableInArray in ChoiceIntString.WithNull().Array(Range.Constant(1, 5))
            from eitherNullableInMap in ChoiceIntString.WithNull().Dictionary()
            from eitherInArrayInEither in G.ChoiceGen(Gen.Bool, ChoiceIntString.IList())
            from eitherNullableInArrayInEither in G.ChoiceGen(Gen.Bool, ChoiceIntString.WithNull().IList())
            from eitherInMapInEither in G.ChoiceGen(Gen.Bool, ChoiceIntString.Dictionary())
            from eitherNullableInMapInEither in G.ChoiceGen(Gen.Bool, ChoiceIntString.WithNull().Dictionary())
            select new MessageWithChoices
            {
                Either = either,
                EitherInArray = eitherInArray,
                EitherInMap = eitherInMap,
                EitherNullable = eitherNullable,
                EitherNullableInArray = eitherNullableInArray,
                EitherNullableInMap = eitherNullableInMap,
                EitherInArrayInEither = eitherInArrayInEither,
                EitherNullableInArrayInEither = eitherNullableInArrayInEither,
                EitherInMapInEither = eitherInMapInEither,
                EitherNullableInMapInEither = eitherNullableInMapInEither
            };

        public static AutoGenConfig Config =>
            GenX.defaults
                .WithGenerator(Message)
                .WithGenerator(ChoiceIntString);
    }

    [Property]
    public void Should_roundtrip_avro_message(MessageWithChoices msg)
    {
        var bytes = msg.SerializeToBinary();
        var deserialized = AvroUtils.DeserializeFromBinary<MessageWithChoices>(bytes);
        deserialized.Should().BeEquivalentTo(msg);
    }
}
