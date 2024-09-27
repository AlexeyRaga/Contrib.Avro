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

    public static implicit operator Choice<T1, T2, T3, T4>(T1 value) => new Choice1Of4(value);
    public static implicit operator Choice<T1, T2, T3, T4>(T2 value) => new Choice2Of4(value);
    public static implicit operator Choice<T1, T2, T3, T4>(T3 value) => new Choice3Of4(value);
    public static implicit operator Choice<T1, T2, T3, T4>(T4 value) => new Choice4Of4(value);

    public static Choice<T1, T2, T3, T4> Wrap(object value) => value switch
    {
        T1 t1 => new Choice1Of4(t1),
        T2 t2 => new Choice2Of4(t2),
        T3 t3 => new Choice3Of4(t3),
        T4 t4 => new Choice4Of4(t4),
        _ => throw new ArgumentException($"{value} is not a valid type for {nameof(Choice<T1, T2, T3, T4>)}")
    };

    public object? Unwrap() => this switch
    {
        Choice1Of4 c1 => c1.Value,
        Choice2Of4 c2 => c2.Value,
        Choice3Of4 c3 => c3.Value,
        Choice4Of4 c4 => c4.Value,
        _ => throw new ArgumentException("Impossible")
    };
}
