using System.Runtime.CompilerServices;

namespace Contrib.Avro;

public abstract record Choice<T1, T2, T3, T4, T5, T6, T7>
{
    private Choice() { }

    public sealed record Choice1Of7(T1 Value) : Choice<T1, T2, T3, T4, T5, T6, T7>;
    public sealed record Choice2Of7(T2 Value) : Choice<T1, T2, T3, T4, T5, T6, T7>;
    public sealed record Choice3Of7(T3 Value) : Choice<T1, T2, T3, T4, T5, T6, T7>;
    public sealed record Choice4Of7(T4 Value) : Choice<T1, T2, T3, T4, T5, T6, T7>;
    public sealed record Choice5Of7(T5 Value) : Choice<T1, T2, T3, T4, T5, T6, T7>;
    public sealed record Choice6Of7(T6 Value) : Choice<T1, T2, T3, T4, T5, T6, T7>;
    public sealed record Choice7Of7(T7 Value) : Choice<T1, T2, T3, T4, T5, T6, T7>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue(T1 value) => new Choice1Of7(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue(T2 value) => new Choice2Of7(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue(T3 value) => new Choice3Of7(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue(T4 value) => new Choice4Of7(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue(T5 value) => new Choice5Of7(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue(T6 value) => new Choice6Of7(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue(T7 value) => new Choice7Of7(value);


    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T1 value) => new Choice1Of7(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T2 value) => new Choice2Of7(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T3 value) => new Choice3Of7(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T4 value) => new Choice4Of7(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T5 value) => new Choice5Of7(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T6 value) => new Choice6Of7(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6, T7>(T7 value) => new Choice7Of7(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7> Wrap(object value) => value switch
    {
        T1 t1 => new Choice1Of7(t1),
        T2 t2 => new Choice2Of7(t2),
        T3 t3 => new Choice3Of7(t3),
        T4 t4 => new Choice4Of7(t4),
        T5 t5 => new Choice5Of7(t5),
        T6 t6 => new Choice6Of7(t6),
        T7 t7 => new Choice7Of7(t7),
        _ => throw new ArgumentException($"{value} is not a valid type for {nameof(Choice<T1, T2, T3, T4, T5, T6, T7>)}")
    };

    public object? Unwrap()
    {
        return this switch
        {
            Choice1Of7 c1 => c1.Value,
            Choice2Of7 c2 => c2.Value,
            Choice3Of7 c3 => c3.Value,
            Choice4Of7 c4 => c4.Value,
            Choice5Of7 c5 => c5.Value,
            Choice6Of7 c6 => c6.Value,
            Choice7Of7 c7 => c7.Value,
            _ => throw new ArgumentException("Impossible")
        };
    }
}
