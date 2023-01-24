using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using static AdventOfCode2022.Day17;
using static AdventOfCode2022.Day18;
using static AdventOfCode2022.Day19;

namespace AdventOfCode2022
{
    internal class Day20
    {
        public static Stopwatch Watch = new Stopwatch();


        public static Tuple<string, string> Solve(string input)
        {
            var list = File.ReadAllLines(input);
            var part1 = "";
            var part2 = "";

            var numbers = new Dictionary<int, Number>();

            for (var i = 0; i < list.Count(); i++)
            {
                var number = new Number { Index = i, Value = int.Parse(list[i]) };
                numbers.Add(i,number);
            }

            for (var i = 0; i < list.Count(); i++)
            {
                var target = numbers[i].Value + numbers[i].Index;
                var from = Math.Min(target, numbers[i].Index);
                var to = Math.Max(target, numbers[i].Index);

                var dir = target > numbers[i].Index ? -1 : 1; 

                for(var j = from; j <= to; j++)
                {
                    var tj = j;
                    if (tj < 0) tj = numbers.Count - (Math.Abs(tj)% numbers.Count);
                    else if (tj >= numbers.Count) tj = tj % numbers.Count;

                    var number = numbers.First( n => n.Value.Index == tj);
                    number.Value.Index += dir;
                    number.Value.Index = number.Value.Index % numbers.Count;
                    numbers[number.Key] = number.Value;
                }
                numbers[i].Index = target;
            }

            var result = numbers.Select(n => n.Value).OrderBy(n => n.Index);


            return new Tuple<string, string>(part1, part2);       
        }

        public class Number
        {
            public int Value;
            public int Index;
        }
    }
}
