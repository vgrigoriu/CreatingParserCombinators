using System.Collections.Generic;

public delegate IEnumerable<(TTree, IEnumerable<TToken>)> Parser<TTree, TToken>(IEnumerable<TToken> input);

public delegate IEnumerable<(TTree, IEnumerable<char>)> Parser<TTree>(IEnumerable<char> input);

public static class Parser
{
    public static Parser<TTree> Result<TTree>(TTree value) =>
        input => new[]{(value, input)};
}