using System;
using System.Linq;
using static Parser;

namespace ParserCombinators
{
    class Program
    {
        static void Main(string[] args)
        {
            var resultParser = Result(123);
            var results = resultParser("something");
            foreach (var result in results)
            {
                Console.WriteLine($"{result.Item1} {result.Item2}");
            }

            var failParser = Fail<TimeSpan>();
            var failResults = failParser("something else");
            foreach (var failResult in failResults)
            {
                Console.WriteLine($"{failResult.Item1} {failResult.Item2}");
            }

            var itemParser = Item();
            var oneItem = itemParser("a string").Single();
            Console.WriteLine($"{oneItem.Item1} {string.Concat(oneItem.Item2)}");
            var noItem = itemParser(string.Empty);
            Console.WriteLine($"{noItem.Count()} results");

            var parseX = Char('x');
            var trueX = parseX("x123").Single();
            Console.WriteLine($"{trueX.Item1} {string.Concat(trueX.Item2)}");

            var parseLowerUpperDigit = from lower in Lower()
                                       from upper in Upper()
                                       from digit in Digit()
                                       select string.Concat(lower, upper, digit);
            var ludResult = parseLowerUpperDigit("xC6 bau").Single();
            Console.WriteLine($"{ludResult.Item1} {string.Concat(ludResult.Item2)}");

            var letter = Letter();
            var lowerResult = letter("xC6 bau").Single();
            Console.WriteLine($"{lowerResult.Item1} {string.Concat(lowerResult.Item2)}");
            var upperResult = letter("Hey bau").Single();
            Console.WriteLine($"{upperResult.Item1} {string.Concat(upperResult.Item2)}");

            nonEmptyWord = from first in Upper().Or(Lower()).Or(Digit())
                           from rest in word
                           select string.Concat(first, rest);
            word = nonEmptyWord.Or(Result(string.Empty));

            var wordResults = word("Yes123!");
            foreach (var res in wordResults)
            {
                Console.WriteLine($"{res.Item1} {string.Concat(res.Item2)}");
            }

            var fourLetterWord = word.Where(x => x.Length == 4);
            var fourLetterWordResult = fourLetterWord("Abracadabra").Single();
            Console.WriteLine($"{fourLetterWordResult.Item1}");

            var manyDigits = Many(Digit());
            var manyDigitsResult = manyDigits("1234");
            foreach (var r in manyDigitsResult)
            {
                Console.WriteLine($"{string.Join("", r.Item1)}-{string.Join("", r.Item2)}");
            }

            var ints = Ints();
            var intsResult = ints("345");
            foreach (var r in intsResult)
            {
                Console.WriteLine($"{r.Item1}");
            }
        }

        static Parser<string> word;

        static Parser<string> nonEmptyWord;
    }
}
