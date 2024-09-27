using Avro;
using Avro.Util;

namespace Contrib.Avro.CodeGen.Tests.LogicalTypes;

public sealed class Duration() : LogicalType("duration")
{
    public override void ValidateSchema(LogicalSchema schema)
    {
        if (Schema.Type.Int != schema.BaseSchema.Tag)
            throw new AvroTypeException("'duration' can only be used with an underlying int type");
    }

    public override object ConvertToBaseValue(object logicalValue, LogicalSchema schema) =>
        (int)((TimeSpan)logicalValue).TotalMilliseconds;

    public override object ConvertToLogicalValue(object baseValue, LogicalSchema schema) =>
        TimeSpan.FromMilliseconds((int)baseValue);

    public override Type GetCSharpType(bool nullible) =>
        nullible ? typeof(TimeSpan?) : typeof(TimeSpan);

    public override bool IsInstanceOfLogicalType(object logicalValue) =>
        logicalValue is TimeSpan;
}
