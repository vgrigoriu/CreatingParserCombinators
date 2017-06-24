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

Where

```cs
public static Parser<T> Where<T>(
    this Parser<T> parser,
    Func<T, bool> predicate)
{
    return parser.SelectMany(result => predicate(result) ? Result(result) : Fail<T>());
}
```

A parser that matches one particular character

```cs
public static Parser<char> Char(char c) =>
    Item().Where(x => x == c);

var bParser = Char('b');
```

Parsers that match classes of characters

```cs
    public static Parser<char> Digit() => Item().Where(char.IsDigit);

    public static Parser<char> Lower() => Item().Where(char.IsLower);

    public static Parser<char> Upper() => Item().Where(char.IsUpper);
```

Alternative / Or

```cs
public static Parser<T> Or<T>(this Parser<T> one, Parser<T> other) =>
    input => one(input).Concat(other(input));
```

Any letter

```cs
var letter = Lower().Or(Upper());
```

A word of any length (returns multiple matches)

```cs
nonEmptyWord = from first in Upper().Or(Lower()).Or(Digit())
               from rest in word
               select string.Concat(first, rest);

word = nonEmptyWord.Or(Result(string.Empty));
```