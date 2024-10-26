using FluentAssertions;
using Hedgehog;
using Hedgehog.Linq;
using Hedgehog.Xunit;
using Gen = Hedgehog.Linq.Gen;
using Range = Hedgehog.Linq.Range;

namespace Contrib.Avro.CodeGen.Tests;

[Properties(AutoGenConfig = typeof(Generators))]
public sealed class FixedTypesTests
{
    [Property]
    public void Should_have_equality_for_fixed_types(MD5 md5)
    {
        var other = new MD5((byte[])md5.Value.Clone());

        (other.Value == md5.Value).Should().BeFalse();
        (other == md5).Should().BeTrue();
        other.Equals(md5).Should().BeTrue();
    }
}

file static class Generators
{
    private static Gen<MD5> Md5 =>
        Gen.Byte(Range.LinearBoundedByte())
            .Array(Range.Constant(16, 16))
            .Select(x => new MD5(x));

    private static Gen<MessageWithFixedTypes> FixedTypes =>
        from md5 in Md5
        from md5Array in Md5.Array(Range.Constant(1, 10))
        from md5Map in Md5
            .Array(Range.Constant(0, 10))
            .Select(xs => xs
                .Select((x, i) => (Item: x, Index: i))
                .ToDictionary(x => x.Index.ToString(), x => x.Item))
        from md5Nullable in Md5.WithNull()
        select new MessageWithFixedTypes
        {
            Md5 = md5,
            Md5Array = md5Array,
            Md5Map = md5Map,
            Md5Nullable = md5Nullable
        };

    public static AutoGenConfig Config =>
        GenX.defaults
            .WithGenerator(Md5)
            .WithGenerator(FixedTypes);
}
