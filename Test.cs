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
        p.SelectMany(f, (_, x) => x);

    public static Parser<V> SelectMany<T, U, V>(
        this Parser<T> p,
        Func<T, Parser<U>> f,
        Func<T, U, V> selector) =>
        input =>
            from result in p(input)
            let newParser = f(result.Item1)
            from newResult in newParser(result.Item2)
            select (selector(result.Item1, newResult.Item1), newResult.Item2);

    public static Parser<T> Where<T>(
        this Parser<T> parser,
        Func<T, bool> predicate)
    {
        return parser.SelectMany(result => predicate(result) ? Result(result) : Fail<T>());
    }

    public static Parser<char> Char(char c) => Item().Where(x => x == c);

    public static Parser<char> Digit() => Item().Where(char.IsDigit);

    public static Parser<char> Lower() => Item().Where(char.IsLower);

    public static Parser<char> Upper() => Item().Where(char.IsUpper);

    public static Parser<T> Or<T>(this Parser<T> one, Parser<T> other) =>
        input => one(input).Concat(other(input));

    public static Parser<char> Letter() => Upper().Or(Lower());
}
