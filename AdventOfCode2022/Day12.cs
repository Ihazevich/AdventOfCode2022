using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AdventOfCode2022
{
    internal class Day12
    {
        public static Tuple<string, string> Solve(string input)
        {
            var heightMap = File.ReadAllLines(input);
            var routeMap = new Dictionary<(int,int),Node>();
            var part1 = $"";
            var part2 = $"";
            var bestSignal = (0,0);
            var starts = new List<(int, int)>();
            var part2Treks = new List<int>();

            var h = heightMap.Length;
            var w = heightMap[0].Length;

           
            for(var s = 0; s < h; s++)
            {
                if(heightMap[s].Contains("S"))
                {
                    starts.Add((heightMap[s].IndexOf('S'), s));
                    heightMap[s] = heightMap[s].Replace('S', ('a'));
                }
                if (heightMap[s].Contains("E"))
                {
                    bestSignal = (heightMap[s].IndexOf('E'), s);
                    heightMap[s] = heightMap[s].Replace('E', ('z'));
                }
            }

            for (var s = 0; s < h; s++)
            {
                if (heightMap[s].Contains("a"))
                {
                    starts.Add((heightMap[s].IndexOf('a'), s));
                }
            }

            for (var i = 0; i < h; ++i)
            {
                for (var j = 0; j < w; j++)
                {
                    var current = heightMap[i][j];

                    var moves = new Node()
                    {
                        HeightCode = current,
                        Height = current,
                        X = j,
                        Y = i,
                        CanNorth = i > 0 && (heightMap[i - 1][j] - 1) <= current,
                        CanSouth = i < heightMap.Length - 1 && (heightMap[i + 1][j] - 1) <= current,
                        CanWest = j > 0 && (heightMap[i][j - 1] - 1) <= current,
                        CanEast = j < heightMap[i].Length - 1 && (heightMap[i][j + 1] - 1) <= current,
                    };

                    routeMap.Add((j,i), moves);
                }
            }

            Parallel.ForEach(starts, start => {
                var isPart1 = starts[0] == start;

                routeMap[start].SetDistance(bestSignal.Item1, bestSignal.Item2);
                var activeNodes = new List<Node>();
                activeNodes.Add(routeMap[start]);
                var visitedNodes = new List<Node>();

                var steps = 0;

                while (activeNodes.Any())
                {
                    var node = activeNodes.OrderBy(x => x.CostDistance).First();
                    if (node.X == routeMap[bestSignal].X && node.Y == routeMap[bestSignal].Y)
                    {
                        var n = node;
                        var map = new List<(int, int)>();
                        while (true)
                        {
                            map.Add((n.X, n.Y));
                            n = n.Parent;
                            if (n == null)
                            {
                                if(isPart1)
                                {
                                    part1 = $"{steps}";
                                }
                                else
                                {
                                    part2Treks.Add(steps);
                                }
                                break;
                            }
                            steps++;
                        }
                        break;
                    }

                    visitedNodes.Add(node);
                    activeNodes.Remove(node);

                    var nextNodes = node.TraversableNodes(routeMap, bestSignal.Item1, bestSignal.Item2);

                    foreach (var n in nextNodes)
                    {
                        if (visitedNodes.Any(v => v.X == n.X && v.Y == n.Y))
                        {
                            continue;
                        }
                        if (activeNodes.Any(v => v.X == n.X && v.Y == n.Y))
                        {
                            var repeatedNode = activeNodes.First(v => v.X == n.X && v.Y == n.Y);
                            if (repeatedNode.CostDistance > n.CostDistance)
                            {
                                activeNodes.Remove(repeatedNode);
                                activeNodes.Add(n);
                            }
                        }
                        else
                        {
                            activeNodes.Add(n);
                        }

                    }
                }
            });

            part2 = $"{part2Treks.Min(t => t)}";

            return new Tuple<string, string>(part1, part2);
        }

        private class Node
        {
            public char HeightCode { get; set; }
            public int Height { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public bool CanNorth { get; set; }
            public bool CanSouth { get; set; }
            public bool CanEast { get; set; }
            public bool CanWest { get; set; }

            public int Cost { get; set; }
            public int Distance { get; set; }
            public int CostDistance => Cost + Distance;

            public Node Parent { get; set; }

            public bool HasMoves()
            {
                return CanNorth || CanSouth || CanWest || CanEast;
            }

            public string Print()
            {
                return $"{(CanNorth ? "N" : "")}{(CanSouth ? "S" : "")}{(CanWest ? "W" : "")}{(CanEast ? "E" : "")}";
            }

            public void SetDistance(int x,int y)
            {
                Distance = Math.Abs(x - X) + Math.Abs(y - Y);
            }

            public List<Node> TraversableNodes(Dictionary<(int,int),Node> map, int x, int y)
            {
                var nodes = new List<Node>();

                if(CanNorth)
                {
                    nodes.Add(map[(X, Y - 1)].CopySelf());
                }
                if (CanSouth)
                {
                    nodes.Add(map[(X, Y + 1)].CopySelf());
                }
                if (CanEast)
                {
                    nodes.Add(map[(X + 1, Y)].CopySelf());
                }
                if (CanWest)
                {
                    nodes.Add(map[(X - 1, Y)].CopySelf());
                }

                foreach(var node in nodes)
                {
                    node.Parent = this;
                    node.Cost = Cost + 1;
                    node.SetDistance(x, y);
                }

                return nodes;
            }

            private Node CopySelf()
            {
                var node = new Node()
                {
                    X = X,
                    Y = Y,
                    CanEast = CanEast,
                    CanWest = CanWest,
                    CanNorth = CanNorth,
                    CanSouth = CanSouth,
                };
                return node;
            }
        }
    }
}
