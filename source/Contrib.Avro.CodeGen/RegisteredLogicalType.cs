using Avro;
using Avro.Util;

namespace Contrib.Avro.Codegen;

internal sealed class RegisteredLogicalType(string name, string dotnetType) : LogicalType(name)
{
    public string DotnetTypeHint { get; } = dotnetType;
    public override object ConvertToBaseValue(object logicalValue, LogicalSchema schema)
    {
        return logicalValue;
    }

    public override object ConvertToLogicalValue(object baseValue, LogicalSchema schema)
    {
        return baseValue;
    }

    public override Type GetCSharpType(bool nullible) =>
        throw new InvalidOperationException("This type should only be used for code generation.");

    public override bool IsInstanceOfLogicalType(object logicalValue) =>
        throw new InvalidOperationException("This type should only be used for code generation.");
}

internal sealed class UnknownLogicalType(string name) : LogicalType(name)
{
    public override object ConvertToBaseValue(object logicalValue, LogicalSchema schema) =>
        throw new InvalidOperationException("Unknown logical type should not be used.");

    public override object ConvertToLogicalValue(object baseValue, LogicalSchema schema) =>
        throw new InvalidOperationException("Unknown logical type should not be used.");

    public override Type GetCSharpType(bool nullible) =>
        throw new InvalidOperationException("Unknown logical type should not be used.");

    public override bool IsInstanceOfLogicalType(object logicalValue) =>
        throw new InvalidOperationException("Unknown logical type should not be used.");
}
