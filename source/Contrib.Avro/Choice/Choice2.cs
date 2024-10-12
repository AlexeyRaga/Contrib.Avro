using System.Runtime.CompilerServices;

namespace Contrib.Avro;

public abstract record Choice<T1, T2>
{
    private Choice() { }

    public sealed record Choice1Of2(T1 Value) : Choice<T1, T2>;
    public sealed record Choice2Of2(T2 Value) : Choice<T1, T2>;

    public static implicit operator Choice<T1, T2>(T1 value) => new Choice1Of2(value);
    public static implicit operator Choice<T1, T2>(T2 value) => new Choice2Of2(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2> FromValue(T1 value) => new Choice1Of2(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2> FromValue(T2 value) => new Choice2Of2(value);
}
