namespace Contrib.Avro;

public static class Choice
{
    public static Choice<T1, T2> FromValue<T1, T2>(T1 value) => new Choice<T1, T2>.Choice1Of2(value);
    public static Choice<T1, T2> FromValue<T1, T2>(T2 value) => new Choice<T1, T2>.Choice2Of2(value);
    public static Choice<T1, T2, T3> FromValue<T1, T2, T3>(T1 value) => new Choice<T1, T2, T3>.Choice1Of3(value);
    public static Choice<T1, T2, T3> FromValue<T1, T2, T3>(T2 value) => new Choice<T1, T2, T3>.Choice2Of3(value);
    public static Choice<T1, T2, T3> FromValue<T1, T2, T3>(T3 value) => new Choice<T1, T2, T3>.Choice3Of3(value);

    public static Choice<T1, T2, T3, T4> FromValue<T1, T2, T3, T4>(T1 value) =>
        new Choice<T1, T2, T3, T4>.Choice1Of4(value);

    public static Choice<T1, T2, T3, T4> FromValue<T1, T2, T3, T4>(T2 value) =>
        new Choice<T1, T2, T3, T4>.Choice2Of4(value);

    public static Choice<T1, T2, T3, T4> FromValue<T1, T2, T3, T4>(T3 value) =>
        new Choice<T1, T2, T3, T4>.Choice3Of4(value);

    public static Choice<T1, T2, T3, T4> FromValue<T1, T2, T3, T4>(T4 value) =>
        new Choice<T1, T2, T3, T4>.Choice4Of4(value);

    public static Choice<T1, T2, T3, T4, T5> FromValue<T1, T2, T3, T4, T5>(T1 value) =>
        new Choice<T1, T2, T3, T4, T5>.Choice1Of5(value);

    public static Choice<T1, T2, T3, T4, T5> FromValue<T1, T2, T3, T4, T5>(T2 value) =>
        new Choice<T1, T2, T3, T4, T5>.Choice2Of5(value);

    public static Choice<T1, T2, T3, T4, T5> FromValue<T1, T2, T3, T4, T5>(T3 value) =>
        new Choice<T1, T2, T3, T4, T5>.Choice3Of5(value);

    public static Choice<T1, T2, T3, T4, T5> FromValue<T1, T2, T3, T4, T5>(T4 value) =>
        new Choice<T1, T2, T3, T4, T5>.Choice4Of5(value);

    public static Choice<T1, T2, T3, T4, T5> FromValue<T1, T2, T3, T4, T5>(T5 value) =>
        new Choice<T1, T2, T3, T4, T5>.Choice5Of5(value);

    public static Choice<T1, T2, T3, T4, T5, T6> FromValue<T1, T2, T3, T4, T5, T6>(T1 value) =>
        new Choice<T1, T2, T3, T4, T5, T6>.Choice1Of6(value);

    public static Choice<T1, T2, T3, T4, T5, T6> FromValue<T1, T2, T3, T4, T5, T6>(T2 value) =>
        new Choice<T1, T2, T3, T4, T5, T6>.Choice2Of6(value);

    public static Choice<T1, T2, T3, T4, T5, T6> FromValue<T1, T2, T3, T4, T5, T6>(T3 value) =>
        new Choice<T1, T2, T3, T4, T5, T6>.Choice3Of6(value);

    public static Choice<T1, T2, T3, T4, T5, T6> FromValue<T1, T2, T3, T4, T5, T6>(T4 value) =>
        new Choice<T1, T2, T3, T4, T5, T6>.Choice4Of6(value);

    public static Choice<T1, T2, T3, T4, T5, T6> FromValue<T1, T2, T3, T4, T5, T6>(T5 value) =>
        new Choice<T1, T2, T3, T4, T5, T6>.Choice5Of6(value);

    public static Choice<T1, T2, T3, T4, T5, T6> FromValue<T1, T2, T3, T4, T5, T6>(T6 value) =>
        new Choice<T1, T2, T3, T4, T5, T6>.Choice6Of6(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue<T1, T2, T3, T4, T5, T6, T7>(T1 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7>.Choice1Of7(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue<T1, T2, T3, T4, T5, T6, T7>(T2 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7>.Choice2Of7(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue<T1, T2, T3, T4, T5, T6, T7>(T3 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7>.Choice3Of7(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue<T1, T2, T3, T4, T5, T6, T7>(T4 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7>.Choice4Of7(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue<T1, T2, T3, T4, T5, T6, T7>(T5 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7>.Choice5Of7(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue<T1, T2, T3, T4, T5, T6, T7>(T6 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7>.Choice6Of7(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7> FromValue<T1, T2, T3, T4, T5, T6, T7>(T7 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7>.Choice7Of7(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7, T8>.Choice1Of8(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7, T8>.Choice2Of8(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7, T8>.Choice3Of8(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7, T8>.Choice4Of8(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7, T8>.Choice5Of8(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7, T8>.Choice6Of8(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7, T8>.Choice7Of8(value);

    public static Choice<T1, T2, T3, T4, T5, T6, T7, T8> FromValue<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value) =>
        new Choice<T1, T2, T3, T4, T5, T6, T7, T8>.Choice8Of8(value);
}
