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

    public static Parser<IEnumerable<T>> Many<T>(Parser<T> p)
    {
        var empty = Result(Enumerable.Empty<T>());
        var nonEmpty = from x in p
                       from rest in Many(p)
                       select x.Concat(rest);

        return nonEmpty.Or(empty);
    }

    public static Parser<int> Ints()
    {
        return from digits in Many(Digit())
               from digitsStr in Result(string.Join(string.Empty, digits))
               where digitsStr.Length > 0
               select int.Parse(digitsStr);
    }

    public static Parser<U> Select<T, U>(this Parser<T> p, Func<T, U> selector)
    {
        return p.SelectMany(x => Result(selector(x)));
    }

    private static IEnumerable<T> Concat<T>(this T first, IEnumerable<T> rest)
    {
        yield return first;
        foreach (var x in rest)
        {
            yield return x;
        }
    }
}
