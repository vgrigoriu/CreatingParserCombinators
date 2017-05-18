﻿using System;
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
        }
    }
}
