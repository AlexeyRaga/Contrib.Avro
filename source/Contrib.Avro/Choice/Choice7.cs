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
}
