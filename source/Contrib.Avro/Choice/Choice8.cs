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

    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> Wrap(object value) => value switch
    {
        T1 t1 => new Choice1Of8(t1),
        T2 t2 => new Choice2Of8(t2),
        T3 t3 => new Choice3Of8(t3),
        T4 t4 => new Choice4Of8(t4),
        T5 t5 => new Choice5Of8(t5),
        T6 t6 => new Choice6Of8(t6),
        T7 t7 => new Choice7Of8(t7),
        T8 t8 => new Choice8Of8(t8),
        _ => throw new ArgumentException($"{value} is not a valid type for {nameof(Choice<T1, T2, T3, T4, T5, T6, T7, T8>)}")
    };

    public object? Unwrap()
    {
        return this switch
        {
            Choice1Of8 c1 => c1.Value,
            Choice2Of8 c2 => c2.Value,
            Choice3Of8 c3 => c3.Value,
            Choice4Of8 c4 => c4.Value,
            Choice5Of8 c5 => c5.Value,
            Choice6Of8 c6 => c6.Value,
            Choice7Of8 c7 => c7.Value,
            Choice8Of8 c8 => c8.Value,
            _ => throw new ArgumentException("Impossible")
        };
    }
}
