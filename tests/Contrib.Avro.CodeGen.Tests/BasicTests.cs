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
    public static class Generators
    {
        public static Gen<MD5> MD5 =>
            Gen.Byte(Range.LinearBoundedByte())
                .Array(Range.Constant(16, 16))
                .Select(x => new MD5(x));

        public static AutoGenConfig Config =>
            GenX.defaults
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
}
