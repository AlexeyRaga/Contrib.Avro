using Contrib.Avro.CodeGen.Tests.LogicalTypes;
using FluentAssertions;
using Hedgehog;
using Hedgehog.Linq;
using Hedgehog.Xunit;
using Testing.Messages;
using Gen = Hedgehog.Linq.Gen;
using Range = Hedgehog.Linq.Range;

namespace Contrib.Avro.CodeGen.Tests;

public static class Generators
{
    public static Gen<Simple> Simple()
    {
        return from id in Gen.Guid.NoShrink().Select(x => new UserId(x))
               from name in Gen.AlphaNumeric.String(Range.LinearInt32(0, 100))
               from duration in Gen.Int32(Range.LinearInt32(0, 10000)).Select(x => TimeSpan.FromMilliseconds(x)).NullValue()
               select new Simple
               {
                   id = id,
                   name = name,
                   result = duration
               };
    }

    public static AutoGenConfig Config =>
        GenX.defaults
            .WithGenerator(Simple());
}

[Properties(AutoGenConfig = typeof(Generators))]
public class RoundtripTests
{
    static RoundtripTests()
    {
        global::Avro.Util.LogicalTypeFactory.Instance.Register(new UserIdLogicalType());
        global::Avro.Util.LogicalTypeFactory.Instance.Register(new Duration());
    }

    [Property]
    public void Should_roundtrip_avro_message(Simple msg)
    {
        var bytes = msg.SerializeToBinary();
        var deserialized = AvroUtils.DeserializeFromBinary<Simple>(bytes);
        msg.name.Should().NotContain("f");
        deserialized.Should().BeEquivalentTo(msg);
    }
}


public record B(bool b);
public record A(int X, string Y, B b, float c);
