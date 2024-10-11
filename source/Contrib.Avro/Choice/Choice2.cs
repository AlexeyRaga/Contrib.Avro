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

    public static Choice<T1, T2> Wrap(object value) => value switch
    {
        T1 t1 => new Choice1Of2(t1),
        T2 t2 => new Choice2Of2(t2),
        _ => throw new ArgumentException($"{value} is not a valid type for {nameof(Choice<T1, T2>)}")
    };

    public object? Unwrap()
    {
        return this switch
        {
            Choice1Of2 c1 => c1.Value,
            Choice2Of2 c2 => c2.Value,
            _ => throw new ArgumentException("Impossible")
        };
    }
}
