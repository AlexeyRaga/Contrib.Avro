using System.Runtime.CompilerServices;

namespace Contrib.Avro;

public abstract record Choice<T1, T2, T3, T4>
{
    private Choice()
    {
    }

    public sealed record Choice1Of4(T1 Value) : Choice<T1, T2, T3, T4>;
    public sealed record Choice2Of4(T2 Value) : Choice<T1, T2, T3, T4>;
    public sealed record Choice3Of4(T3 Value) : Choice<T1, T2, T3, T4>;
    public sealed record Choice4Of4(T4 Value) : Choice<T1, T2, T3, T4>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4> FromValue(T1 value) => new Choice1Of4(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4> FromValue(T2 value) => new Choice2Of4(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4> FromValue(T3 value) => new Choice3Of4(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4> FromValue(T4 value) => new Choice4Of4(value);


    public static implicit operator Choice<T1, T2, T3, T4>(T1 value) => new Choice1Of4(value);
    public static implicit operator Choice<T1, T2, T3, T4>(T2 value) => new Choice2Of4(value);
    public static implicit operator Choice<T1, T2, T3, T4>(T3 value) => new Choice3Of4(value);
    public static implicit operator Choice<T1, T2, T3, T4>(T4 value) => new Choice4Of4(value);
}
