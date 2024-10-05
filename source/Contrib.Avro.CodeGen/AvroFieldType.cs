using System;
using Avro;

namespace Contrib.Avro.Codegen;

internal sealed record AvroFieldType(
    string Type,
    Schema Schema,
    bool Nullable = false,
    Func<string, string>? Unwrapper = null,
    Func<string, string>? Wrapper = null)
{
    public string FullType => Type + (Nullable ? "?" : "");
}
