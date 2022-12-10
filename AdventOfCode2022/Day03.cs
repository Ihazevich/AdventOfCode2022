using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day03 
    {
        public static Tuple<string, string> Solve(string input)
        {
            var rucksacks = File.ReadAllLines(input);
            var totalPriority = 0;

            // Part 1
            foreach (var rucksack in rucksacks)
            {
                var items = rucksack.Length / 2;

                var firstCompartment = rucksack.Substring(0, items);
                var secondCompartment = rucksack.Substring(items);

                var found = false;

                foreach (var item in firstCompartment)
                {
                    for (var i = 0; i < items; i++)
                    {
                        if (item == secondCompartment[i])
                        {
                            var priority = 0;
                            if ((int)secondCompartment[i] > 90)
                            {
                                priority = (int)secondCompartment[i] - 96;
                            }
                            else
                            {
                                priority = (int)secondCompartment[i] - 38;
                            }

                            totalPriority += priority;
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                }
            }

            // Part 2

            var totalPriority2 = 0;

            Parallel.For(0, rucksacks.Length / 3, index =>
            {
                var found = false;
                for (int i = 0; i < rucksacks[index * 3].Length; i++)
                {
                    for (int j = 0; j < rucksacks[index * 3 + 1].Length; j++)
                    {
                        if (rucksacks[index * 3][i] == rucksacks[index * 3 + 1][j])
                        {
                            for (int k = 0; k < rucksacks[index * 3 + 2].Length; k++)
                            {
                                if (rucksacks[index * 3][i] == rucksacks[index * 3 + 2][k])
                                {
                                    found = true;

                                    if ((int)rucksacks[index * 3][i] > 90)
                                    {
                                        Interlocked.Add(ref totalPriority2, (int)rucksacks[index * 3][i] - 96);
                                    }
                                    else
                                    {
                                        Interlocked.Add(ref totalPriority2, (int)rucksacks[index * 3][i] - 38);
                                    }
                                    break;
                                }
                            }
                        }
                        if (found) { break; }
                    }
                    if (found) { break; }
                }
            });

            var part1 = $"{totalPriority}";
            var part2 = $"{totalPriority2}";

            return new Tuple<string, string>(part1, part2);
        }

    }
}
