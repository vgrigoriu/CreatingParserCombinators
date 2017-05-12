abstract class Tree {}

delegate Tree Parser(string input);

delegate (Tree, string) Parser(string input);

delegate (Tree, string)? Parser(string input);

delegate IEnumerable<(Tree, string)> Parser(string input);

delegate IEnumerable<(TTree, string)> Parser<TTree>(string input);

delegate IEnumerable<(TTree, IEnumerable<char>)> Parser<TTree>(IEnumerable<char> input);

delegate IEnumerable<(TTree, IEnumerable<TToken>)> Parser<TTree, TToken>(IEnumerable<TToken> input);
