# Parser Combinators

## What is a parser?

Turns a string into an abstract syntax tree:

```cs
delegate Tree Parser(string input);
```

But, it might not consume all the input:

```cs
delegate (Tree, string) Parser(string input);
```

But, it might fail:

```cs
delegate (Tree, string)? Parser(string input);
```

But, the grammar might be ambiguous:

```cs
delegate IEnumerable<(Tree, string)> Parser(string input);
```

But, it needs to be generic:

```cs
delegate IEnumerable<(TTree, string)> Parser<TTree>(string input);
```

And, we can also generalize the input. A `string` is an `IEnumerable<char>`:

```cs
delegate IEnumerable<(TTree, IEnumerable<char>)> Parser<TTree>(IEnumerable<char> input);
```

And we might need to parse something else (e.g. bytes):

```cs
delegate IEnumerable<(TTree, IEnumerable<TToken>)> Parser<TTree, TToken>(IEnumerable<TToken> input);
```

## Basic parsers

Result, a parser that always returns the same value:

```cs
public static Parser<TTree> Result<TTree>(TTree value) =>
    input => new[]{(value, input)};
```

Fail, a parser that always fails:

```cs
public static Parser<TTree> Fail<TTree>() =>
    input => Enumerable.Empty<(TTree, IEnumerable<char>)>();
```

Item, a parser that consumes a token and returns it:

```cs
public static Parser<char> Item() =>
    input => (input == null || !input.Any())
        ? new (char, IEnumerable<char>)[]{}
        : new[]{(input.First(), input.Skip(1))};
```

## Parser combinators

Bind / SelectMany

```cs
public static Parser<V> SelectMany<T, U, V>(
    this Parser<T> p,
    Func<T, Parser<U>> f,
    Func<T, U, V> selector) =>
    input =>
        from result in p(input)
        let newParser = f(result.Item1)
        from newResult in newParser(result.Item2)
        select (selector(result.Item1, newResult.Item1), newResult.Item2);
```

Alternative / Or

```cs
public static Parser<T> Or<T>(this Parser<T> one, Parser<T> other) =>
    input => one(input).Concat(other(input));
```