# Parser Combinators

## What is a parser?

Turns a string into an abstract syntax tree:

```
delegate Tree Parser(string input);
```

But, it might not consume all the input:

```
delegate (Tree, string) Parser(string input);
```

But, it might fail:

```
delegate (Tree, string)? Parser(string input);
```

But, the grammar might be ambiguous:

```
delegate IEnumerable<(Tree, string)> Parser(string input);
```

But, it needs to be generic:

```
delegate IEnumerable<(TTree, string)> Parser<TTree>(string input);
```

And, we can also generalize the input. A `string` is an `IEnumerable<char>`:

```
delegate IEnumerable<(TTree, IEnumerable<char>)> Parser<TTree>(IEnumerable<char> input);
```

And we might need to parse something else (e.g. bytes):

```
delegate IEnumerable<(TTree, IEnumerable<TToken>)> Parser<TTree, TToken>(IEnumerable<TToken> input);
```
