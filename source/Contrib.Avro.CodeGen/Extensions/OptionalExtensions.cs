using System;
using Microsoft.CodeAnalysis;

namespace Contrib.Avro.Codegen;

public static class OptionalExtensions
{
    // LINQ-style Select (Map) operation
    public static Optional<TResult> Select<T, TResult>(this Optional<T> optional, Func<T, TResult> selector) =>
        optional.HasValue ? new Optional<TResult>(selector(optional.Value)) : default;

    // LINQ-style Where (Filter) operation
    public static Optional<T> Where<T>(this Optional<T> optional, Func<T, bool> predicate) =>
        optional.HasValue && predicate(optional.Value) ? optional : default;

    // LINQ-style DefaultIfEmpty operation
    public static T DefaultIfEmpty<T>(this Optional<T> optional, T defaultValue) =>
        optional.HasValue ? optional.Value : defaultValue;

    // LINQ-style DefaultIfEmpty with function for lazy evaluation
    public static T DefaultIfEmpty<T>(this Optional<T> optional, Func<T> defaultFunc) =>
        optional.HasValue ? optional.Value : defaultFunc();

    // LINQ-style Combine operation (equivalent to SelectMany)
    public static Optional<TResult> SelectMany<T1, T2, TResult>(
        this Optional<T1> opt1,
        Func<T1, Optional<T2>> opt2Selector,
        Func<T1, T2, TResult> resultSelector) =>
        opt1.HasValue && opt2Selector(opt1.Value).HasValue
            ? new Optional<TResult>(resultSelector(opt1.Value, opt2Selector(opt1.Value).Value))
            : default;
}
