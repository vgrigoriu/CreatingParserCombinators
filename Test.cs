using System;
using System.Collections.Generic;
using System.Linq;

public delegate IEnumerable<(TTree, IEnumerable<TToken>)> Parser<TTree, TToken>(IEnumerable<TToken> input);

public delegate IEnumerable<(TTree, IEnumerable<char>)> Parser<TTree>(IEnumerable<char> input);

public static class Parser
{
    public static Parser<TTree> Result<TTree>(TTree value) =>
        input => new[]{(value, input)};

    public static Parser<TTree> Fail<TTree>() =>
        input => Enumerable.Empty<(TTree, IEnumerable<char>)>();

    // maybe keep TToken type parameter instead of particularizing to char?
    public static Parser<char> Item() =>
        input => (input == null || !input.Any())
            ? new (char, IEnumerable<char>)[]{}
            : new[]{(input.First(), input.Skip(1))};

    public static Parser<U> SelectMany<T, U>(this Parser<T> p, Func<T, Parser<U>> f) =>
        input =>
            from result in p(input)
            let newParser = f(result.Item1)
            from newResult in newParser(result.Item2)
            select newResult;

    public static Parser<V> SelectMany<T, U, V>(
        this Parser<T> p,
        Func<T, Parser<U>> f,
        Func<T, U, V> selector) =>
        input =>
            from result in p(input)
            let newParser = f(result.Item1)
            from newResult in newParser(result.Item2)
            select (selector(result.Item1, newResult.Item1), newResult.Item2);

    public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> selector) =>
        input => from result in parser(input)
                 select (selector(result.Item1), result.Item2);

    public static Parser<char> Sat(Func<char, bool> predicate)
    {
        return Item().SelectMany(c => {
            if (predicate(c))
            {
                return Result(c);
            }
            else
            {
                return Fail<char>();
            }
        });
    }

    public static Parser<char> Char(char c)
    {
        return Sat(x => x == c);
    }

    public static Parser<char> Digit() => Sat(char.IsDigit);

    public static Parser<char> Lower() => Sat(char.IsLower);

    public static Parser<char> Upper() => Sat(char.IsUpper);

    public static Parser<T> Or<T>(this Parser<T> one, Parser<T> other) =>
        input => one(input).Concat(other(input));
}
