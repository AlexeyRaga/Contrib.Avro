using System.Runtime.CompilerServices;

namespace Contrib.Avro;

public abstract record Choice<T1, T2, T3>
{
    private Choice() { }
    public sealed record Choice1Of3(T1 Value) : Choice<T1, T2, T3>;
    public sealed record Choice2Of3(T2 Value) : Choice<T1, T2, T3>;
    public sealed record Choice3Of3(T3 Value) : Choice<T1, T2, T3>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3> FromValue(T1 value) => new Choice1Of3(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3> FromValue(T2 value) => new Choice2Of3(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3> FromValue(T3 value) => new Choice3Of3(value);


    public static implicit operator Choice<T1, T2, T3>(T1 value) => new Choice1Of3(value);
    public static implicit operator Choice<T1, T2, T3>(T2 value) => new Choice2Of3(value);
    public static implicit operator Choice<T1, T2, T3>(T3 value) => new Choice3Of3(value);
}
