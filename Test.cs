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
}