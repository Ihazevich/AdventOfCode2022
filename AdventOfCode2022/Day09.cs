using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day09 
    {
        public static Tuple<string, string> Solve(string input)
        {
            var movements = File.ReadAllLines(input);

            var knots = 10;
            var rope = new List<(int, int)>(knots);

            for (var i = 0; i < knots; i++)
            {
                rope.Add((0, 0));
            }

            var visitedPart1 = new Dictionary<(int, int), int>();
            var visitedPart2 = new Dictionary<(int, int), int>();

            visitedPart1.Add(rope[knots - 1], 1);
            visitedPart2.Add(rope[knots - 1], 1);

            foreach (var movement in movements)
            {
                var distance = int.Parse(movement.Substring(movement.IndexOf(' ') + 1));

                for (var d = distance; d > 0; d--)
                {
                    var headPos = rope[0];

                    switch (movement[0])
                    {
                        case 'U':
                            headPos.Item2 += 1;
                            break;
                        case 'D':
                            headPos.Item2 -= 1;
                            break;
                        case 'L':
                            headPos.Item1 -= 1;
                            break;
                        case 'R':
                            headPos.Item1 += 1;
                            break;
                    }

                    rope[0] = headPos;

                    for (var k = 1; k < knots; k++)
                    {
                        var xDistance = rope[k - 1].Item1 - rope[k].Item1;
                        var yDistance = rope[k - 1].Item2 - rope[k].Item2;

                        while (Math.Abs(xDistance) > 1 || Math.Abs(yDistance) > 1)
                        {
                            var knotPos = rope[k];

                            // Horizontal correction
                            if (yDistance == 0)
                            {
                                knotPos.Item1 += xDistance / Math.Abs(xDistance);
                            }

                            // Vertical correction
                            if (xDistance == 0)
                            {
                                knotPos.Item2 += yDistance / Math.Abs(yDistance);
                            }

                            // Diagonal correction 1
                            if ((xDistance > 1 || xDistance < -1) && (yDistance != 0))
                            {
                                knotPos.Item1 += xDistance / Math.Abs(xDistance);
                                knotPos.Item2 += yDistance / Math.Abs(yDistance);
                            }
                            else if ((yDistance > 1 || yDistance < -1) && (xDistance != 0))
                            {
                                knotPos.Item1 += xDistance / Math.Abs(xDistance);
                                knotPos.Item2 += yDistance / Math.Abs(yDistance);
                            }

                            if (k == knots - 1)
                            {
                                if (visitedPart2.ContainsKey(knotPos))
                                {
                                    visitedPart2[knotPos]++;
                                }
                                else
                                {
                                    visitedPart2.Add(knotPos, 1);
                                }
                            }
                            else if (k == 1)
                            {
                                if (visitedPart1.ContainsKey(knotPos))
                                {
                                    visitedPart1[knotPos]++;
                                }
                                else
                                {
                                    visitedPart1.Add(knotPos, 1);
                                }
                            }

                            rope[k] = knotPos;

                            xDistance = rope[k - 1].Item1 - rope[k].Item1;
                            yDistance = rope[k - 1].Item2 - rope[k].Item2;
                        }
                    }
                }
            }

            var part1 = $"{visitedPart1.Count}";
            var part2 = $"{visitedPart2.Count}";

            return new Tuple<string, string>(part1, part2);
        }

    }
}
