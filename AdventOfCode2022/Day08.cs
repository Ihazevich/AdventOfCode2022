using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day08 
    {
        public static Tuple<string, string> Solve(string input)
        {
            var map = File.ReadAllLines(input);

            var maxColHeights = new List<int>();
            var maxRowHeights = new List<int>();

            var visibleTrees = 0;

            foreach (var tree in map[0])
            {
                var height = int.Parse(tree.ToString());
                maxColHeights.Add(height);
                visibleTrees++;
            }

            foreach (var tree in map[map.Length - 1])
            {
                var height = int.Parse(tree.ToString());
                visibleTrees++;
            }

            for (int i = 1; i < map.Length - 1; i++)
            {
                visibleTrees++;
                maxRowHeights.Add(int.Parse(map[i][0].ToString()));

                visibleTrees++;
            }

            maxRowHeights.Add(int.Parse(map[0][0].ToString()));

            for (int i = 1; i < map.Length - 1; i++)
            {
                for (int j = 1; j < map[i].Length - 1; j++)
                {
                    var height = int.Parse(map[i][j].ToString());

                    var thisTreeVisible = false;

                    if (height > maxColHeights[j])
                    {
                        thisTreeVisible = true;
                        maxColHeights[j] = height;
                    }
                    else
                    {
                        var visible = true;
                        for (int k = i + 1; k < map.Length; k++)
                        {
                            if (height <= int.Parse(map[k][j].ToString()))
                            {
                                visible = false; break;
                            }
                        }
                        if (visible)
                        {
                            thisTreeVisible = true;
                        }
                    }

                    if (height > maxRowHeights[i])
                    {
                        thisTreeVisible = true;
                        maxRowHeights[i] = height;
                    }
                    else
                    {
                        var visible = true;
                        for (int k = j + 1; k < map[i].Length; k++)
                        {
                            if (height <= int.Parse(map[i][k].ToString()))
                            {
                                visible = false; break;
                            }
                        }
                        if (visible)
                        {
                            thisTreeVisible = true;
                        }
                    }

                    if (thisTreeVisible)
                    {
                        visibleTrees++;
                    }
                }
            }

            var part1 = $"{visibleTrees}";

            var maxScenicScore = 0;
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    var height = int.Parse(map[i][j].ToString());

                    var scoreU = 0;
                    var scoreD = 0;
                    var scoreL = 0;
                    var scoreR = 0;
                    for (int l = j + 1; l < map[i].Length; l++)
                    {
                        var height2 = int.Parse(map[i][l].ToString());
                        if (height > height2)
                        {
                            scoreL++;
                        }
                        else
                        {
                            scoreL++;
                            break;
                        }
                    }

                    for (int r = j - 1; r >= 0; r--)
                    {
                        var height2 = int.Parse(map[i][r].ToString());
                        if (height > height2)
                        {
                            scoreR++;
                        }
                        else
                        {
                            scoreR++;
                            break;
                        }
                    }

                    for (int u = i - 1; u >= 0; u--)
                    {
                        var height2 = int.Parse(map[u][j].ToString());
                        if (height > height2)
                        {
                            scoreU++;
                        }
                        else
                        {
                            scoreU++;
                            break;
                        }
                    }

                    for (int d = i + 1; d < map.Length; d++)
                    {
                        var height2 = int.Parse(map[d][j].ToString());
                        if (height > height2)
                        {
                            scoreD++;
                        }
                        else
                        {
                            scoreD++;
                            break;
                        }
                    }

                    var scenicScore = scoreD * scoreU * scoreL * scoreR;

                    if (scenicScore > maxScenicScore)
                    {
                        maxScenicScore = scenicScore;
                    }
                }
            }

            var part2 = $"{maxScenicScore}";

            return new Tuple<string, string>(part1, part2);
        }

    }
}
