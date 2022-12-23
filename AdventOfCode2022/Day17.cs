using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using static AdventOfCode2022.Day17;

namespace AdventOfCode2022
{
    internal class Day17
    {
        public static bool Verbose = false;

        public static Stopwatch Watch = new();

        public static List<Rock> Rocks = new List<Rock>();
        public static int MaxHeightP1 = 0;

        public static int[] Jets = new int[0];
        public static int JetsP1 = 0;

        public const int SIZE = 500_000;

        public static Tuple<string, string> Solve(string input)
        {
            Rocks.Add(new Rock
            {
                Shape = new bool[][]
                {
                    new bool[] {true,true,true,true }
                },
                Width = 4,
                Height = 1,
            });

            Rocks.Add(new Rock
            {
                Shape = new bool[][]
                {
                    new bool[] {false,true,false},
                    new bool[] {true,false,true},
                    new bool[] {false,true,false},
                },
                Width = 3,
                Height = 3,
            });

            Rocks.Add(new Rock
            {
                Shape = new bool[][]
                {
                    new bool[] {true,true,true},
                    new bool[] {false,false,true},
                    new bool[] {false,false,true},
                },
                Width = 3,
                Height = 3,
            });

            Rocks.Add(new Rock
            {
                Shape = new bool[][]
                {
                    new bool[] {true},
                    new bool[] {true},
                    new bool[] {true},
                    new bool[] {true},
                },
                Width = 1,
                Height = 4,
            });

            Rocks.Add(new Rock
            {
                Shape = new bool[][]
                {
                    new bool[] {true,true},
                    new bool[] {true,true},
                },
                Width = 2,
                Height = 2,
            });

            var t = new Stopwatch();
            t.Start();
            var literalJets = File.ReadAllBytes(input);
            Jets = new int[literalJets.Length];
            var part1 = "";
            var part2 = "";

            for (var j = 0; j < literalJets.Length; j++)
            {
                Jets[j] = literalJets[j] == '<' ? -1 : 1;
            }

            var chamber = new bool[7][];

            for (var x = 0; x < 7; x++)
            {
                chamber[x] = new bool[SIZE];
            }

            for (var r = 0; r < 2022; r++)
            {
                chamber = DropRock(Rocks[r % 5], chamber);
            }

            part1 = $"{MaxHeightP1:N0}";

            chamber = new bool[7][];

            for (var x = 0; x < 7; x++)
            {
                chamber[x] = new bool[SIZE];
            }

            MaxHeightP1 = 0;
            JetsP1 = 0;

            BigInteger upperLimit = 1_000_000_000_000;
            BigInteger result = 0;
            BigInteger remainingRocks = 0;

            var startingCombinations = new Dictionary<RockDropData, (int, int)>();
            var cycleFound = false;
            var prevHeight = 0;

            for (var r = 0; r < upperLimit; r++)
            {
                var prevJetIndex = JetsP1;
                var jet = JetsP1 % Jets.Length;
                var rock = r % 5;
                prevHeight = MaxHeightP1;

                var prevChamber = chamber;

                chamber = DropRock(Rocks[r % 5], chamber);

                var dropData = new RockDropData
                {
                    RockType = rock,
                    JetIndex = jet,
                    JetsFired = JetsP1 - prevJetIndex,
                    HeightChange = MaxHeightP1 - prevHeight,
                };

                if (!cycleFound)
                {
                    if (startingCombinations.ContainsKey(dropData))
                    {
                        var combination = startingCombinations[dropData];

                        var cycleHeight = MaxHeightP1 - combination.Item1;
                        var cycleLength = r - combination.Item2;

                        BigInteger cyclesLeft = (upperLimit - r) / cycleLength;
                        remainingRocks = (upperLimit - r) % cycleLength;
                        var remainingRocks2 = (upperLimit - r) - (cyclesLeft * cycleLength);
                        if ((cyclesLeft * cycleLength) + remainingRocks + r != upperLimit) throw new Exception();

                        result = cyclesLeft * cycleHeight + prevHeight;
                        chamber = prevChamber;
                                                
                        if (remainingRocks != 0)
                        {
                            var finalRock = startingCombinations.First((c) => c.Value.Item2 == (combination.Item2 + remainingRocks));
                            result += finalRock.Value.Item1 - combination.Item1;
                        }
                        cycleFound = true;
                        break;
                    }
                    else
                        startingCombinations.Add(dropData, (MaxHeightP1, r));
                }
            }
            part2 = $"{result:N0}";

            return new Tuple<string, string>(part1, part2);
        }

        public class Rock
        {
            public bool[][] Shape;
            public bool Falling = true;
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        public class RockDropData
        {
            public int RockType;
            public int JetIndex;
            public int JetsFired;
            public int HeightChange;

            public override bool Equals(object? obj)
            {
                if(obj == null) return false;

                var data = obj as RockDropData;
                if (data == null) return false;
                if (RockType != data.RockType) return false;
                if (JetIndex != data.JetIndex) return false;
                if (JetsFired != data.JetsFired) return false;
                if (HeightChange != data.HeightChange) return false;

                return true;
            }

            public override int GetHashCode()
            {
                return RockType.GetHashCode() ^ JetIndex.GetHashCode() ^ JetsFired.GetHashCode() ^ HeightChange.GetHashCode();
            }
        }

        public static bool[][] DropRock(Rock rock, bool[][] chamber)
        {
            rock.X = 2;
            rock.Y = MaxHeightP1 + 3;
            rock.Falling = true;
            while (rock.Falling)
            {
                if(Verbose)
                {
                    for (var y = Math.Max(MaxHeightP1, rock.Y + rock.Height); y >= 0; y--)
                    {
                        var s = "|";
                        for (var x = 0; x < 7; x++)
                        {
                            if (y >= rock.Y && y <= rock.Y + rock.Height - 1)
                            {
                                if (x >= rock.X && x <= rock.X + rock.Width - 1)
                                {
                                    if (rock.Shape[y - rock.Y][x - rock.X]) s += "@";
                                    else if (chamber[x][y]) s += "#";
                                    else s += ".";
                                }
                                else if (chamber[x][y]) s += "#";
                                else s += ".";
                            }
                            else if (chamber[x][y]) s += "#";
                            else s += ".";
                        }
                        s += "|";
                        Console.WriteLine(s);
                    }
                    Console.WriteLine("+-------+");
                }
                
                var minX = 0;
                var maxX = 6 - rock.Width + 1;

                for(var x = rock.X; x < rock.X + rock.Width; x++)
                {
                    for (var y = rock.Y; y < rock.Y + rock.Height; y++)
                    {
                        var (w, h) = (x - rock.X, y - rock.Y);
                        if (rock.Shape[h][w])
                        {
                            if (x > 0)
                                if (chamber[x - 1][y])
                                    minX = Math.Max(minX, rock.X);

                            if (x < 6)
                                if (chamber[x + 1][y])
                                    maxX = Math.Min(maxX, rock.X);
                        }

                    }
                }

                rock.X += Jets[JetsP1 % Jets.Length];
                if(rock.X < minX) rock.X = minX;
                if(rock.X > maxX) rock.X = maxX;

                JetsP1++;

                for (var x = rock.X; x < rock.X + rock.Width; x++)
                {
                    for (var y = rock.Y; y < rock.Y + rock.Height; y++)
                    {
                        var (w, h) = (x - rock.X, y - rock.Y);
                        if (rock.Shape[h][w] && rock.Falling)
                        {
                            if (y > 0)
                            {
                                if (chamber[x][y - 1])
                                    rock.Falling = false;
                            }
                            else
                                rock.Falling = false;
                        }
                    }
                }

                if (rock.Falling)
                    rock.Y--;
            }

            for (var x = rock.X; x < rock.X + rock.Width; x++)
            {
                for (var y = rock.Y; y < rock.Y + rock.Height; y++)
                {
                    var (w, h) = (x - rock.X, y - rock.Y);
                    if (rock.Shape[h][w])
                    {
                        chamber[x][y] = true;
                        MaxHeightP1 = Math.Max(y + 1, MaxHeightP1);
                    }
                }
            }
            return chamber;
        }
    }
}
