using System.Diagnostics;
using System.Net.Http.Headers;
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


        public static Tuple<string, string> Solve(string input)
        {
            Rocks.Add(new Rock
            {
                Shape = new int[,]
                {
                    {1,1,1,1 }
                },
                Width = 4,
                Height = 1,
            });

            Rocks.Add(new Rock
            {
                Shape = new int[,]
                {
                    {0,1,0},
                    {1,1,1},
                    {0,1,0},
                },
                Width = 3,
                Height = 3,
            });

            Rocks.Add(new Rock
            {
                Shape = new int[,]
                {
                    {1,1,1},
                    {0,0,1},
                    {0,0,1},
                },
                Width = 3,
                Height = 3,
            });

            Rocks.Add(new Rock
            {
                Shape = new int[,]
                {
                    {1},
                    {1},
                    {1},
                    {1},
                },
                Width = 1,
                Height = 4,
            });

            Rocks.Add(new Rock
            {
                Shape = new int[,]
                {
                    {1,1},
                    {1,1},
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

            for(var j = 0; j < literalJets.Length; j++)
            {
                Jets[j] = literalJets[j] == '<' ? -1 : 1;
            }

            var chamber = new List<int>[7];
            for(var c = 0; c < 7; c++)
            {
                chamber[c] = new List<int>();
            }


            for(var r = 0; r < 2022; r++)
            {
                var rock = new Rock(Rocks[r % 5]);
                DropRock(rock, chamber);
                if(Verbose) Console.WriteLine($"After Rock {r + 1:N0} Max Height is {MaxHeightP1:N0}");
            }

            part1 = $"{MaxHeightP1}";

            return new Tuple<string, string>(part1, part2);
        }

        public class Rock
        {
            public int[,] Shape = new int[0,0];
            public bool Falling = true;
            public int X;
            public int Y;
            public int Width;
            public int Height;

            public Rock()
            {

            }

            public Rock(Rock model)
            {
                Shape = model.Shape;
                Width = model.Width;
                Height = model.Height;
                Array.Copy(model.Shape, Shape, Shape.Length);
            }

        }

        public static void DropRock(Rock rock, List<int>[] chamber)
        {
            rock.X = 2;
            rock.Y = MaxHeightP1 + 3;

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
                                    if (rock.Shape[y - rock.Y, x - rock.X] == 1) s += "@";
                                    else if (chamber[x].Contains(y)) s += "#";
                                    else s += ".";
                                }
                                else if (chamber[x].Contains(y)) s += "#";
                                else s += ".";
                            }
                            else if (chamber[x].Contains(y)) s += "#";
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
                        if (rock.Shape[h, w] == 0) continue;

                        if(x > 0)
                            if (chamber[x-1].Contains(y)) 
                                minX = Math.Max(minX,rock.X);

                        if(x < 6)
                            if (chamber[x + 1].Contains(y))
                                maxX = Math.Min(maxX, rock.X);
                    }
                }

                // Jet

                rock.X += Jets[JetsP1 % Jets.Length];
                if(rock.X < minX) rock.X = minX;
                if(rock.X > maxX) rock.X = maxX;

                if (rock.X + rock.Width > 7)
                    throw new Exception();
                if (rock.X < 0)
                    throw new Exception();

                JetsP1++;

                for (var x = rock.X; x < rock.X + rock.Width; x++)
                {
                    for (var y = rock.Y; y < rock.Y + rock.Height; y++)
                    {
                        var (w, h) = (x - rock.X, y - rock.Y);
                        if (rock.Shape[h, w] == 0) continue;

                        if (y > 0)
                        {
                            if (chamber[x].Contains(y - 1))
                                rock.Falling = false;
                        }
                        else
                            rock.Falling = false;
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
                    if (rock.Shape[h, w] == 0) continue;

                    chamber[x].Add(y);
                    MaxHeightP1 = Math.Max(y+1, MaxHeightP1);
                }
            }

            if(MaxHeightP1 <= 0) throw new Exception();

        }

       
    }
}
