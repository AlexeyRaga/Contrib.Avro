using Avro;
using Avro.Util;

namespace Contrib.Avro.LogicalTypes;

public sealed class IdentityLogicalType<TCsharpType>(string name) : LogicalType(name)
{
    public override object ConvertToBaseValue(object logicalValue, LogicalSchema schema) =>
        logicalValue;

    public override object ConvertToLogicalValue(object baseValue, LogicalSchema schema) =>
        baseValue;

    public override Type GetCSharpType(bool nullible) =>
        nullible ? typeof(TCsharpType?) : typeof(TCsharpType);

    public override bool IsInstanceOfLogicalType(object logicalValue) =>
        logicalValue is TCsharpType;

    public override void ValidateSchema(LogicalSchema schema)
    {
        if (schema.LogicalTypeName != Name)
            throw new AvroTypeException("Wrong logical type name");
    }
}
