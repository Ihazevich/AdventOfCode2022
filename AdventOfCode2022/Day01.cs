using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day01  
    {
        public static Tuple<string, string> Solve(string input)
        {
            var file = File.ReadAllLines(input);
            var currentElf = 0;
            int[] top3 = { 0, 0, 0 };

            foreach (var line in file)
            {
                if (int.TryParse(line, out int i))
                {
                    currentElf += i;
                }
                else
                {
                    if (currentElf > top3[2])
                    {
                        top3[0] = top3[1];
                        top3[1] = top3[2];
                        top3[2] = currentElf;
                    }
                    currentElf = 0;
                }
            }

            var part1 = $"{top3[2]}";
            var part2 = $"{top3[0] + top3[1] + top3[2]}";

            return new Tuple<string, string>(part1, part2);
        }

    }
}
