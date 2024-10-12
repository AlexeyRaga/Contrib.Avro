using System.Runtime.CompilerServices;

namespace Contrib.Avro;

public abstract record Choice<T1, T2, T3, T4, T5, T6, T7, T8>
{
    private Choice() { }

    public sealed record Choice1Of8(T1 Value) : Choice<T1, T2, T3, T4, T5, T6, T7, T8>;
    public sealed record Choice2Of8(T2 Value) : Choice<T1, T2, T3, T4, T5, T6, T7, T8>;
    public sealed record Choice3Of8(T3 Value) : Choice<T1, T2, T3, T4, T5, T6, T7, T8>;
    public sealed record Choice4Of8(T4 Value) : Choice<T1, T2, T3, T4, T5, T6, T7, T8>;
    public sealed record Choice5Of8(T5 Value) : Choice<T1, T2, T3, T4, T5, T6, T7, T8>;
    public sealed record Choice6Of8(T6 Value) : Choice<T1, T2, T3, T4, T5, T6, T7, T8>;
    public sealed record Choice7Of8(T7 Value) : Choice<T1, T2, T3, T4, T5, T6, T7, T8>;
    public sealed record Choice8Of8(T8 Value) : Choice<T1, T2, T3, T4, T5, T6, T7, T8>;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue(T1 value) => new Choice1Of8(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue(T2 value) => new Choice2Of8(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue(T3 value) => new Choice3Of8(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue(T4 value) => new Choice4Of8(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue(T5 value) => new Choice5Of8(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue(T6 value) => new Choice6Of8(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue(T7 value) => new Choice7Of8(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue(T8 value) => new Choice8Of8(value);

    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value) => new Choice1Of8(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value) => new Choice2Of8(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value) => new Choice3Of8(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value) => new Choice4Of8(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value) => new Choice5Of8(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value) => new Choice6Of8(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value) => new Choice7Of8(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value) => new Choice8Of8(value);
}
