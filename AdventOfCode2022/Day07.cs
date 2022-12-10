using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day07 
    {
        public static Tuple<string, string> Solve(string input)
        {
            var terminalOutput = File.ReadAllLines(input);
            var listOfDirectories = new Dictionary<string, int>();
            var currentDirectory = "";

            foreach (var line in terminalOutput)
            {
                var fileSize = 0;

                if (line.StartsWith("$ cd .."))
                {
                    currentDirectory = currentDirectory.Remove(currentDirectory.LastIndexOf("/"));
                    var c = currentDirectory.Last();
                    while (c != '/')
                    {
                        currentDirectory = currentDirectory.Remove(currentDirectory.Length - 1);
                        c = currentDirectory.Last();
                    }
                }
                else if (line.StartsWith("$ cd"))
                {
                    currentDirectory += line.Substring(5) + "/";
                    listOfDirectories.Add(currentDirectory, 0);
                }
                else if (int.TryParse(line.Substring(0, line.IndexOf(" ")), out fileSize))
                {
                    listOfDirectories[currentDirectory] += fileSize;
                }
            }

            var updatedSizes = listOfDirectories;
            foreach (var (dir1, size1) in listOfDirectories)
            {
                foreach (var (dir2, size2) in listOfDirectories)
                {
                    if (dir1 != dir2)
                    {
                        if (dir2.StartsWith(dir1))
                        {
                            updatedSizes[dir1] += updatedSizes[dir2];
                        }
                    }
                }
            }

            var totalSize = 0;
            foreach (var (dir, size) in updatedSizes)
            {
                if (dir == "//") continue;
                if (size <= 100000)
                {
                    totalSize += size;
                }
            }

            var part1 = $"{totalSize}";

            var targetSize = 30000000;
            var remainingSize = 70000000 - listOfDirectories["//"];
            var deleteTarget = targetSize - remainingSize;
            var deleteTargets = new Dictionary<string, int>();

            foreach (var (dir, size) in updatedSizes)
            {
                if (size >= deleteTarget)
                {
                    deleteTargets.Add(dir, size);
                }
            }

            var min = deleteTargets.ElementAt(0).Value;
            foreach (var (dir, size) in deleteTargets)
            {
                if (size < min)
                {
                    min = size;
                }
            }

            var part2 = $"{min}";

            return new Tuple<string, string>(part1, part2);
        }

    }
}
