using Avro.Util;
using Contrib.Avro.LogicalTypes;
using FluentAssertions;
using Hedgehog.Xunit;

namespace Contrib.Avro.CodeGen.Tests;

public sealed class LogicalTypesTests
{
    static LogicalTypesTests()
    {
        LogicalTypeFactory.Instance.Register(new IdentityLogicalType<string>("department-id"));
    }

    [Property]
    public void Should_roundtrip_message(MessageWithLogicalTypes message)
    {
        var bytes = message.SerializeToBinary();
        var deserialized = AvroUtils.DeserializeFromBinary<MessageWithLogicalTypes>(bytes);
        deserialized.Should().BeEquivalentTo(message);
    }
}
