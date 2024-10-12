using System.Runtime.CompilerServices;

namespace Contrib.Avro;

public abstract record Choice<T1, T2, T3, T4, T5, T6>
{
    private Choice() { }

    public sealed record Choice1Of6(T1 Value) : Choice<T1, T2, T3, T4, T5, T6>;
    public sealed record Choice2Of6(T2 Value) : Choice<T1, T2, T3, T4, T5, T6>;
    public sealed record Choice3Of6(T3 Value) : Choice<T1, T2, T3, T4, T5, T6>;
    public sealed record Choice4Of6(T4 Value) : Choice<T1, T2, T3, T4, T5, T6>;
    public sealed record Choice5Of6(T5 Value) : Choice<T1, T2, T3, T4, T5, T6>;
    public sealed record Choice6Of6(T6 Value) : Choice<T1, T2, T3, T4, T5, T6>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6> FromValue(T1 value) => new Choice1Of6(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6> FromValue(T2 value) => new Choice2Of6(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6> FromValue(T3 value) => new Choice3Of6(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6> FromValue(T4 value) => new Choice4Of6(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6> FromValue(T5 value) => new Choice5Of6(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Choice<T1, T2, T3, T4, T5, T6> FromValue(T6 value) => new Choice6Of6(value);



    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T1 value) => new Choice1Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T2 value) => new Choice2Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T3 value) => new Choice3Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T4 value) => new Choice4Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T5 value) => new Choice5Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T6 value) => new Choice6Of6(value);
}
