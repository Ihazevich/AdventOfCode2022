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
    internal class Day14
    {
        public static Tuple<string, string> Solve(string input)
        {
            var scans = File.ReadAllLines(input);
            var part1 = "";
            var part2 = "";

            var map = new Map();

            foreach(var scan in scans)
            {
                var last = 0;
                var points = new List<Point>();
                while(last < scan.Length)
                {
                    var next = scan.IndexOf(',',last);
                    next = next < 0 ? scan.Length : next;
                    var s = scan.Substring(last, next - last);
                    var x = int.Parse(s);
                    last = next + 1; 
                    next = scan.IndexOf('>',last);
                    next = next < 0 ? scan.Length : next - 2;
                    s = scan.Substring(last, next - last);
                    var y = int.Parse(s);
                    last = next + 3;

                    points.Add(new Point { X = x, Y = y ,Type = '#'});
                }

                for(var p = 0; p < points.Count - 1; p++) 
                {
                    map.DrawLine(points[p], points[p + 1]);
                }
            }

            map.Generate(false);
            var sand = 0;
            while(map.DropSand())
            {
                sand++;
                //map.Print();
                //Console.WriteLine(sand);
            }
            //map.Print();
            part1 = $"{sand}";


            map.Generate(true);
            sand = 0;
            while (map.DropSand())
            {
                sand++;
                //map.Print();
                //Console.WriteLine(sand);
            }
            sand++;
            //map.Print();
            part2 = $"{sand}";


            return new Tuple<string, string>(part1, part2);
        }


        private class Map
        {
            public List<Point> Points = new List<Point>();
            public Point Max { get; set; }
            public Point Min { get; set; }
            public Dictionary<int,string> theMap = new Dictionary<int, string>();

            public Map()
            {
                Points.Add(new Point { Type = '+', X = 500, Y = 0 });
                Max = new Point { X = 500, Y = 0 };
                Min = new Point { X = 500, Y = 0 };

            }

            public void Add(Point p)
            {
                Points.Add(p);
            }

            public void DrawLine(Point a, Point b)
            {
                Points.Add(a);
                Points.Add(b);

                if (a.X > Max.X || b.X > Max.X)
                {
                    Max = new Point { X = (a.X > b.X) ? a.X : b.X, Y = Max.Y };
                }
                else if (a.X < Min.X || b.X < Min.X)
                {
                    Min = new Point { X = (a.X < b.X) ? a.X : b.X, Y = Min.Y };
                }
                if (a.Y > Max.Y || b.Y > Max.Y)
                {
                    Max = new Point { X = Max.X, Y = (a.Y > b.Y) ? a.Y : b.Y };
                }
                else if (a.Y < Min.Y || b.Y < Min.Y)
                {
                    Min = new Point { X = Min.X, Y = (a.Y < b.Y) ? a.Y : b.Y };
                }

                if (a.X == b.X)
                {
                    var max = Math.Abs(a.Y - b.Y);
                    var dir = (max / (a.Y - b.Y)) * -1;

                    for(var i = 1; i < max; i++)
                    {
                        var y = a.Y + (i * dir);
                        Points.Add(new Point { X = a.X, Y = y, Type = '#' });
                        if (y > Max.Y)
                        {
                            Max = new Point { X = Max.X, Y = y };
                        }
                        else if (y < Min.Y)
                        {
                            Min = new Point { X = Min.X, Y = y };
                        }
                    }
                }
                else
                {
                    var max = Math.Abs(a.X - b.X);
                    var dir = - max / (a.X - b.X);
                    for (var i = 1; i < max; i++)
                    {
                        var x = a.X + (i * dir);
                        Points.Add(new Point { X = x, Y = a.Y, Type = '#'});
                        if (x > Max.X)
                        {
                            Max = new Point { X = x, Y = Max.Y };
                        }
                        else if (x < Min.X)
                        {
                            Min = new Point { X = x, Y = Min.Y };
                        }
                    }
                }
            }

            public bool DropSand()
            {
                var moving = true;
                var overflow = false;
                var (x, y) = (500 - Min.X, 0);
                while(moving)
                {
                    if(y + 1 > Max.Y)
                    {
                        overflow = true;
                        break;
                    }
                    var scanLevel = theMap[y + 1];
                    var scan = scanLevel[x] == '.';
                    if (!scan)
                    {
                        if (x - 1 < 0)
                        {
                            overflow = true;
                            break;
                        }

                        scan = scanLevel[x - 1] == '.';
                        if(!scan)
                        {
                            if (x + 1 >= scanLevel.Length)
                            {
                                overflow = true;
                                break;
                            }

                            scan = scanLevel[x + 1] == '.';
                            if (!scan)
                            {
                                moving = false;
                            }
                            else
                            {
                                (x, y) = (x + 1, y + 1);
                            }
                        }
                        else
                        {
                            (x, y) = (x - 1, y + 1);
                        }
                    }
                    else
                    {
                        (x, y) = (x, y + 1);
                    }
                }

                if(!overflow)
                {
                    theMap[y] = theMap[y].Remove(x, 1).Insert(x, "o");
                    overflow = (x == 500 - Min.X && y == 0);
                }

                return !overflow;
            }

            public void Print()
            {
                for(var y = Min.Y; y <= Max.Y; y++)
                {
                    Console.WriteLine(theMap[y]);
                }
            }

            public void Generate(bool isPart2)
            {
                if(isPart2)
                {
                    Min = new Point
                    {
                        X = (500 - Min.X) > Max.Y + 2 ? Min.X : 500 - (Max.Y + 2),
                        Y = Min.Y
                    };

                    Max = new Point
                    {
                        X = (Max.X - 500) > Max.Y + 2 ? Max.X : 500 + (Max.Y + 2),
                        Y = Max.Y + 2
                    };

                    theMap = new Dictionary<int, string>();
                }


                for (var y = Min.Y; y <= Max.Y; y++)
                {
                    var s = "";
                    for (var x = Min.X; x <= Max.X; x++)
                    {
                        if (Points.Any(p => p.X == x && p.Y == y))
                        {
                            var p = Points.First(p => p.X == x && p.Y == y);
                            s += p.Type;
                        }
                        else if (y == Max.Y && isPart2)
                        {
                            s += '#';
                        }
                        else
                        {
                            s += '.';
                        }
                    }
                    theMap.Add(y,s);
                }
                //Print();
            }


        }

        private struct Point
        {
            public int X;
            public int Y;
            public char Type;

        }
    }
}
