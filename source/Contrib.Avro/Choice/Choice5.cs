namespace Contrib.Avro;

public abstract record Choice<T1, T2, T3, T4, T5>
{
    private Choice() { }

    public sealed record Choice1Of5(T1 Value) : Choice<T1, T2, T3, T4, T5>;
    public sealed record Choice2Of5(T2 Value) : Choice<T1, T2, T3, T4, T5>;
    public sealed record Choice3Of5(T3 Value) : Choice<T1, T2, T3, T4, T5>;
    public sealed record Choice4Of5(T4 Value) : Choice<T1, T2, T3, T4, T5>;
    public sealed record Choice5Of5(T5 Value) : Choice<T1, T2, T3, T4, T5>;

    public static implicit operator Choice<T1, T2, T3, T4, T5>(T1 value) => new Choice1Of5(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5>(T2 value) => new Choice2Of5(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5>(T3 value) => new Choice3Of5(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5>(T4 value) => new Choice4Of5(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5>(T5 value) => new Choice5Of5(value);

    public static Choice<T1, T2, T3, T4, T5> Wrap(object value) => value switch
    {
        T1 t1 => new Choice1Of5(t1),
        T2 t2 => new Choice2Of5(t2),
        T3 t3 => new Choice3Of5(t3),
        T4 t4 => new Choice4Of5(t4),
        T5 t5 => new Choice5Of5(t5),
        _ => throw new ArgumentException($"{value} is not a valid type for {nameof(Choice<T1, T2, T3, T4, T5>)}")
    };

    public object? Unwrap()
    {
        return this switch
        {
            Choice1Of5 c1 => c1.Value,
            Choice2Of5 c2 => c2.Value,
            Choice3Of5 c3 => c3.Value,
            Choice4Of5 c4 => c4.Value,
            Choice5Of5 c5 => c5.Value,
            _ => throw new ArgumentException("Impossible")
        };
    }
}
