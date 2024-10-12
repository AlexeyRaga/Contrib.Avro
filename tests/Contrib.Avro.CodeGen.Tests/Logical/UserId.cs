using Avro;
using Avro.Util;
using Strongly;

namespace Contrib.Avro.CodeGen.Tests;

[Strongly]
public partial struct UserId;

public sealed class UserIdLogicalType() : LogicalType("user-id")
{
    public override object? ConvertToBaseValue(object logicalValue, LogicalSchema schema) =>
        logicalValue.ToString();

    public override object ConvertToLogicalValue(object baseValue, LogicalSchema schema) =>
        UserId.Parse((string)baseValue);

    public override Type GetCSharpType(bool nullible) => !nullible ? typeof(UserId) : typeof(UserId?);

    public override bool IsInstanceOfLogicalType(object logicalValue) => logicalValue is UserId;

    public override void ValidateSchema(LogicalSchema schema)
    {
        if (Schema.Type.String != schema.BaseSchema.Tag)
            throw new AvroTypeException("'user-id' can only be used with an underlying string type");
    }
}
