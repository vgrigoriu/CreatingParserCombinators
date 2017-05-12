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

Result:

```cs
public static Parser<TTree> Result<TTree>(TTree value) =>
    input => new[]{(value, input)};
```
