namespace Contrib.Avro;

public abstract record Choice<T1, T2, T3>
{
    private Choice() { }
    public sealed record Choice1Of3(T1 Value) : Choice<T1, T2, T3>;
    public sealed record Choice2Of3(T2 Value) : Choice<T1, T2, T3>;
    public sealed record Choice3Of3(T3 Value) : Choice<T1, T2, T3>;

    public static implicit operator Choice<T1, T2, T3>(T1 value) => new Choice1Of3(value);
    public static implicit operator Choice<T1, T2, T3>(T2 value) => new Choice2Of3(value);
    public static implicit operator Choice<T1, T2, T3>(T3 value) => new Choice3Of3(value);

    public static Choice<T1, T2, T3> Wrap(object value) => value switch
    {
        T1 t1 => new Choice1Of3(t1),
        T2 t2 => new Choice2Of3(t2),
        T3 t3 => new Choice3Of3(t3),
        _ => throw new ArgumentException($"{value} is not a valid type for {nameof(Choice<T1, T2, T3>)}")
    };

    public object? Unwrap() => this switch
    {
        Choice1Of3 c1 => c1.Value,
        Choice2Of3 c2 => c2.Value,
        Choice3Of3 c3 => c3.Value,
        _ => throw new ArgumentException("Impossible")
    };
}
