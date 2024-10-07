using FluentAssertions;
using Hedgehog;
using Hedgehog.Linq;
using Hedgehog.Xunit;
using Gen = Hedgehog.Linq.Gen;
using Range = Hedgehog.Linq.Range;

namespace Contrib.Avro.CodeGen.Tests;


[Properties(AutoGenConfig = typeof(Generators))]
public sealed class BasicTests
{
    static BasicTests()
    {
        global::Avro.Util.LogicalTypeFactory.Instance.Register(new UserIdLogicalType());
    }

    private static class Generators
    {
        private static Gen<UserId> UserId =>
            Gen.Guid.NoShrink().Select(x => new UserId(x));

        private static Gen<Dictionary<string, T>> Map<T>(Gen<T> valueGen, Range<int> range) =>
            valueGen
                .Array(range)
                .Select(xs => xs.Select((x, i) => (x, i)).ToDictionary(x => x.i.ToString(), x => x.x));

        private static Gen<MessageWithLogicalDecorators> MessageWithLogicalDecorators =>
            from id in UserId
            from cid in UserId.NullValue()
            from arr in UserId.Array(Range.Constant(0, 5))
            from arrNull in UserId.NullValue().Array(Range.Constant(0, 5))
            from map in Map(UserId, Range.Constant(0, 10))
            from mapNull in Map(UserId.NullValue(), Range.Constant(0, 10))
            from choice in Gen.Choice(new List<Gen<Choice<int, UserId>>>
            {
                Gen.Int32(Range.LinearBoundedInt32()).Select(Choice<int, UserId>.FromValue),
                UserId.Select(Choice<int, UserId>.FromValue)
            })
            select new MessageWithLogicalDecorators
            {
                Id = id,
                createdBy = cid,
                decoratedInChoice = choice,
                decoratedInArray = arr,
                decoratedNullableInArray = arrNull,
                decoratedInMap = map,
                decoratedNullableInMap = mapNull
            };

        public static AutoGenConfig Config =>
            GenX.defaults
                .WithGenerator(UserId)
                .WithGenerator(MessageWithLogicalDecorators);
    }

    [Property]
    public void Should_roundtrip_avro_message(MessageWithLogicalDecorators msg)
    {
        var bytes = msg.SerializeToBinary();
        var deserialized = AvroUtils.DeserializeFromBinary<MessageWithLogicalDecorators>(bytes);
        deserialized.Should().BeEquivalentTo(msg);
    }
}
