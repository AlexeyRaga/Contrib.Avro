using System.Runtime.CompilerServices;

namespace Contrib.Avro;

public abstract record Choice<T1, T2, T3, T4, T5>
{
    private Choice() { }

    public sealed record Choice1Of5(T1 Value) : Choice<T1, T2, T3, T4, T5>;
    public sealed record Choice2Of5(T2 Value) : Choice<T1, T2, T3, T4, T5>;
    public sealed record Choice3Of5(T3 Value) : Choice<T1, T2, T3, T4, T5>;
    public sealed record Choice4Of5(T4 Value) : Choice<T1, T2, T3, T4, T5>;
    public sealed record Choice5Of5(T5 Value) : Choice<T1, T2, T3, T4, T5>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5> FromValue(T1 value) => new Choice1Of5(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5> FromValue(T2 value) => new Choice2Of5(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5> FromValue(T3 value) => new Choice3Of5(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5> FromValue(T4 value) => new Choice4Of5(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5> FromValue(T5 value) => new Choice5Of5(value);


    public static implicit operator Choice<T1, T2, T3, T4, T5>(T1 value) => new Choice1Of5(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5>(T2 value) => new Choice2Of5(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5>(T3 value) => new Choice3Of5(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5>(T4 value) => new Choice4Of5(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5>(T5 value) => new Choice5Of5(value);
}
