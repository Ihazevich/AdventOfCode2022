using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day04  
    {
        public static Tuple<string, string> Solve(string input)
        {
            var assignments = File.ReadAllLines(input);

            var totalOverlaps = 0;
            var partialOverlaps = 0;

            foreach (var assignment in assignments)
            {
                var elf1Min = int.Parse(assignment.Substring(0, assignment.IndexOf('-')));
                var elf1Max = int.Parse(assignment.Substring(assignment.IndexOf('-') + 1, assignment.IndexOf(',') - assignment.IndexOf('-') - 1));

                var elf2Min = int.Parse(assignment.Substring(assignment.IndexOf(',') + 1, assignment.LastIndexOf('-') - assignment.IndexOf(',') - 1));
                var elf2Max = int.Parse(assignment.Substring(assignment.LastIndexOf('-') + 1, assignment.Length - 1 - assignment.LastIndexOf('-')));

                if (elf1Min <= elf2Min && elf1Max >= elf2Max)
                {
                    totalOverlaps++;
                    partialOverlaps++;
                }
                else if ((elf2Min <= elf1Min) && (elf2Max >= elf1Max))
                {
                    totalOverlaps++;
                    partialOverlaps++;
                }
                else if (elf1Min >= elf2Min && elf1Min <= elf2Max)
                {
                    partialOverlaps++;
                }
                else if (elf1Max <= elf2Max && elf1Max >= elf2Min)
                {
                    partialOverlaps++;
                }
                else if (elf2Min >= elf1Min && elf2Min <= elf1Max)
                {
                    partialOverlaps++;
                }
                else if (elf2Max <= elf1Max && elf2Max >= elf1Min)
                {
                    partialOverlaps++;
                }
            }

            var part1 = $"{totalOverlaps}";
            var part2 = $"{partialOverlaps}";

            return new Tuple<string, string>(part1, part2);
        }

    }
}
