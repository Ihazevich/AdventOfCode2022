using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day05  
    {
        public static Tuple<string, string> Solve(string input)
        {
            var instructions = File.ReadAllLines(input);

            var stacksNumberLine = 0;
            var numberOfStacks = 0;
            var stacksPart1 = new List<List<char>>();
            var stacksPart2 = new List<List<char>>();

            foreach (var instruction in instructions)
            {
                stacksNumberLine++;
                if (instruction.StartsWith(" 1"))
                {
                    numberOfStacks = int.Parse(instruction[instruction.Length - 2].ToString());
                    break;
                }
            }

            for (int i = 0; i < numberOfStacks; i++)
            {
                stacksPart1.Add(new List<char>());
                stacksPart2.Add(new List<char>());
            }

            for (int i = stacksNumberLine - 2; i >= 0; i--)
            {
                for (int j = 0; j < numberOfStacks; j++)
                {
                    var instruction = instructions[i];

                    if ((j * 4) + 1 < instruction.Length)
                    {
                        var box = instruction[(j * 4) + 1];
                        if (box.ToString() != " ")
                        {
                            stacksPart1[j].Add(instructions[i][(j * 4) + 1]);
                            stacksPart2[j].Add(instructions[i][(j * 4) + 1]);
                        }
                    }
                }
            }

            for (int i = stacksNumberLine; i < instructions.Length; i++)
            {
                var instruction = instructions[i];

                if (instruction.StartsWith("move"))
                {
                    var qty = int.Parse(instruction.Substring(5, instruction.IndexOf('f') - 5));
                    var from = int.Parse(instruction.Substring(instruction.LastIndexOf('m') + 1, instruction.IndexOf('t') - 1 - (instruction.LastIndexOf('m') + 1)));
                    var to = int.Parse(instruction.Substring(instruction.LastIndexOf('o') + 1));

                    // Part 1
                    var tempqty = qty;
                    while (tempqty > 0)
                    {
                        var movingbox = stacksPart1[from - 1].Last();
                        stacksPart1[to - 1].Add(movingbox);
                        stacksPart1[from - 1].RemoveAt(stacksPart1[from - 1].Count - 1);
                        tempqty--;
                    }

                    // Part 2
                    tempqty = qty;
                    while (tempqty > 0)
                    {
                        var movingIndex = stacksPart2[from - 1].Count - tempqty;
                        var movingbox = stacksPart2[from - 1][movingIndex];
                        stacksPart2[to - 1].Add(movingbox);
                        stacksPart2[from - 1].RemoveAt(movingIndex);
                        tempqty--;
                    }
                }
            }

            var part1 = "";
            var part2 = "";

            foreach (var stack in stacksPart1)
            {
                part1 += stack.Last();
            }

            foreach (var stack in stacksPart2)
            {
                part2 += stack.Last();
            }

            return new Tuple<string, string>(part1, part2);
        }

    }
}
