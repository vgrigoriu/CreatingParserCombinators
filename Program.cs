﻿using System;
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
        }
    }
}
