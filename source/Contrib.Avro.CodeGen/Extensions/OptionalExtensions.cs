using System;
using Microsoft.CodeAnalysis;

namespace Contrib.Avro.CodeGen;

public static class OptionalExtensions
{
    // LINQ-style Select (Map) operation
    public static Optional<TResult> Select<T, TResult>(this Optional<T> self, Func<T, TResult> selector) =>
        self.HasValue ? new Optional<TResult>(selector(self.Value)) : default;

    // LINQ-style Where (Filter) operation
    public static Optional<T> Where<T>(this Optional<T> self, Func<T, bool> predicate) =>
        self.HasValue && predicate(self.Value) ? self : default;

    public static Optional<T> Merge<T>(this Optional<T> self, Optional<T> other, Func<T, T, T> combine) =>
        (self, other) switch
        {
            ({ HasValue: true } v1, { HasValue: true } v2) => new Optional<T>(combine(v1.Value, v2.Value)),
            (_, { HasValue: true } o) => o,
            _ => self
        };

    public static Optional<T> Or<T>(this Optional<T> self, Optional<T> other) =>
        self.HasValue ? self : other;

    public static Optional<T> Or<T>(this Optional<T> self, Func<Optional<T>> other) =>
        self.HasValue ? self : other();

    // LINQ-style DefaultIfEmpty operation
    public static T DefaultIfEmpty<T>(this Optional<T> self, T defaultValue) =>
        self.HasValue ? self.Value : defaultValue;

    // LINQ-style DefaultIfEmpty with function for lazy evaluation
    public static T DefaultIfEmpty<T>(this Optional<T> self, Func<T> defaultFunc) =>
        self.HasValue ? self.Value : defaultFunc();

    // LINQ-style Combine operation (equivalent to SelectMany)
    public static Optional<TResult> SelectMany<T1, T2, TResult>(
        this Optional<T1> self,
        Func<T1, Optional<T2>> opt2Selector,
        Func<T1, T2, TResult> resultSelector) =>
        self.HasValue && opt2Selector(self.Value).HasValue
            ? new Optional<TResult>(resultSelector(self.Value, opt2Selector(self.Value).Value))
            : default;
}
