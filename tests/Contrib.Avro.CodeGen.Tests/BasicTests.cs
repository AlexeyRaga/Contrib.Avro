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
    public static class Generators
    {
        public static Gen<UserId> UserId =>
            Gen.Guid.NoShrink().Select(x => new UserId(x));

        public static Gen<MD5> MD5 =>
            Gen.Byte(Range.LinearBoundedByte())
                .Array(Range.Constant(16, 16))
                .Select(x => new MD5(x));

        public static Gen<MessageWithLogicalDecorators> InProject =>
            from id in UserId
            from cid in UserId.NullValue()
            from choice in Gen.Choice(new List<Gen<Choice<int, UserId>>>
            {
                Gen.Int32(Range.LinearBoundedInt32()).Select(Choice<int, UserId>.FromValue),
                UserId.Select(Choice<int, UserId>.FromValue)
            })
            from md5 in MD5
            select new MessageWithLogicalDecorators
            {
                Id = id,
                createdBy = cid,
                decoratedInChoice = choice
            };

        public static AutoGenConfig Config =>
            GenX.defaults
                .WithGenerator(InProject)
                .WithGenerator(MD5);
    }

    [Property]
    public void Should_have_equality_for_fixed_types(MD5 md5)
    {
        var other = new MD5((byte[])md5.Value.Clone());

        (other.Value == md5.Value).Should().BeFalse();
        (other == md5).Should().BeTrue();
        other.Equals(md5).Should().BeTrue();
    }

    [Property, Recheck("0_10321898184904394126_6839591380452281685_000000000000")]
    public void Should_roundtrip_avro_message(MessageWithLogicalDecorators msg)
    {
        var bytes = msg.SerializeToBinary();
        var deserialized = AvroUtils.DeserializeFromBinary<MessageWithLogicalDecorators>(bytes);
        deserialized.Should().BeEquivalentTo(msg);
    }
}
