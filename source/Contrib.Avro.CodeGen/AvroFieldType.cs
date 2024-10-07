using System;
using Avro;

namespace Contrib.Avro.Codegen;

internal sealed record AvroFieldType(
    string Type,
    string BaseType,
    Schema Schema,
    bool Nullable = false,
    Func<string, string>? Unwrapper = null,
    Func<string, string>? Wrapper = null)
{
    public AvroFieldType(string Type,
        Schema Schema,
        bool Nullable = false,
        Func<string, string>? Unwrapper = null,
        Func<string, string>? Wrapper = null) : this(Type, Type, Schema, Nullable, Unwrapper, Wrapper)
    {
    }

    public string FullType => Type + (Nullable ? "?" : "");
    public string FullBaseType => BaseType + (Nullable ? "?" : "");
}
