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

    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T1 value) => new Choice1Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T2 value) => new Choice2Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T3 value) => new Choice3Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T4 value) => new Choice4Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T5 value) => new Choice5Of6(value);
    public static implicit operator Choice<T1, T2, T3, T4, T5, T6>(T6 value) => new Choice6Of6(value);

    public static Choice<T1, T2, T3, T4, T5, T6> Wrap(object value) => value switch
    {
        T1 t1 => new Choice1Of6(t1),
        T2 t2 => new Choice2Of6(t2),
        T3 t3 => new Choice3Of6(t3),
        T4 t4 => new Choice4Of6(t4),
        T5 t5 => new Choice5Of6(t5),
        T6 t6 => new Choice6Of6(t6),
        _ => throw new ArgumentException($"{value} is not a valid type for {nameof(Choice<T1, T2, T3, T4, T5, T6>)}")
    };

    public object? Unwrap()
    {
        return this switch
        {
            Choice1Of6 c1 => c1.Value,
            Choice2Of6 c2 => c2.Value,
            Choice3Of6 c3 => c3.Value,
            Choice4Of6 c4 => c4.Value,
            Choice5Of6 c5 => c5.Value,
            Choice6Of6 c6 => c6.Value,
            _ => throw new ArgumentException("Impossible")
        };
    }
}
