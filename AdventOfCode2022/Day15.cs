using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AdventOfCode2022
{
    internal class Day15
    {
        public static Tuple<string, string> Solve(string input)
        {
            var t = new Stopwatch();
            t.Start();

            var scans = File.ReadAllLines(input);
            var part1 = "";
            var part2 = "";
            //var part1Slice = 10;
            var part1Slice = 2_000_000;

            var map = new Map(part1Slice);

            var i = 0;
            foreach(var scan in scans)
            {
                i++;
                var last = 0;
                var points = new List<Point>();
                var sensor = false;
                while(last < scan.Length)
                {
                    var next = scan.IndexOf(',', last);
                    last = scan.IndexOf('=',last) + 1;
                    var s = scan.Substring(last, next - last);
                    var x = int.Parse(s);
                    last = scan.IndexOf('=', last) + 1; 
                    next = scan.IndexOf(':',last);
                    next = next < 0 ? scan.Length : next;
                    s = scan.Substring(last, next - last);
                    var y = int.Parse(s);
                    last = next;
                    if(!sensor)
                    {
                        points.Add(new Point { X = x, Y = y, Type = 'S' });
                        sensor = true;
                    }
                    else
                    {
                        points.Add(new Point { X = x, Y = y, Type = 'B' });
                        sensor = true;
                    }
                }

                for(var p = 0; p < points.Count - 1; p += 2) 
                {
                    Console.WriteLine($"Calculating Exclusion for sensor {i}");
                    map.ExclusionRealFast(points[p], points[p + 1], part1Slice);
                }
            }
            var count = map.Points.Count(p => p.Y == part1Slice && p.Type == 'B');
            //map.ConfirmedNoAtSlice -= count;
            //map.Print();
            t.Stop();
            part1 = $"{map.GetNotBeacons()} in {t.ElapsedMilliseconds}ms";

            return new Tuple<string, string>(part1, part2);
        }


        private class Map
        {
            public List<Point> Points = new List<Point>();
            public Dictionary<int,string> theMap = new Dictionary<int, string>();
            public Point Max = new Point { X = 0, Y = 0 };
            public Point Min = new Point { X = 0, Y = 0 };
            public int Slice = 0;
            public int ConfirmedNoAtSlice = 0;
            public List<(int,int)> sections = new List<(int,int)> ();

            public Dictionary<int,int> cells = new Dictionary<int,int>();

            public Map(int slice)
            {
                Slice = slice;
            }

            public void Add(Point p)
            {
                var index = Points.FindIndex(p1 => p1.X == p.X && p1.Y == p.Y);
                if ((p.Type == 'B' || p.Type == 'S') && index >= 0)
                {
                    Points.RemoveAt(index);
                    //ConfirmedNoAtSlice -= p.Type == 'B' && p.Y == Slice ? 1 : 0;
                }
                else if(index >= 0)
                {
                    return;
                }
                Points.Add(p);
                ConfirmedNoAtSlice += p.Y == Slice && p.Type != 'B' ? 1 : 0;
                /**
                Max.X = p.X > Max.X ? p.X : Max.X;
                Max.Y = p.Y > Max.Y ? p.Y : Max.Y;
                Min.X = p.X < Min.X ? p.X : Min.X;
                Min.Y = p.Y < Min.Y ? p.Y : Min.Y;
                theMap = new Dictionary<int, string>();
                **/
            }

            public int GetNotBeacons()
            {
                int sum = 0;

                foreach(var (k,v) in cells)
                {
                    if (v > 0) sum++;
                }

                return sum;
            }

            public void DrawExclusionZone(Point s, Point b, int slice)
            {
                Add(s);
                Add(b);

                
                var distance = Math.Abs(s.X - b.X) + Math.Abs(s.Y - b.Y);
                var target = -distance;
                for(var i = distance; i >= target; i--)
                {
                    var y = s.Y - i;
                    if (y != slice) continue;
                    var from = s.X - (distance - Math.Abs(i));
                    var to = s.X + (distance - Math.Abs(i));


                    for(var d = from; d <= to; d++)
                    {
                        var found = false;
                        foreach (var (start, end) in sections)
                        {
                            found = (d >= start && d <= end);
                            if (found)
                            {
                                d = end;
                                break;
                            }
                        }
                        if (found) continue;
                        Console.WriteLine($"{d}/{to}");
                        Add(new Point { X = d, Y = y, Type = '#' });
                    }

                    sections.Add((from, to));
                }           
                
            }

            public void ExclusionFast(Point s, Point b, int slice)
            {
                Add(s);
                Add(b);
                var distance = Math.Abs(s.X - b.X) + Math.Abs(s.Y - b.Y);
                var target = -distance;

                for (var i = distance; i >= target; i--)
                {
                    var y = s.Y - i;
                    if (y != slice) continue;
                    var from = s.X - (distance - Math.Abs(i));
                    var to = s.X + (distance - Math.Abs(i));
                    //Console.WriteLine($"From {from} to {to}");

                    var totalBlocks = from == to ? 1 :Math.Abs(from - to);

                    var found = false;
                    var newFrom = from;
                    var newTo = to;
                    foreach(var (start, end) in sections)
                    {
                        if(IsBetween(from,start,end))
                        {
                            newFrom = end + 1;
                            if (IsBetween(to, start, end))
                            {
                                newTo = start - 1;
                            }
                            found = true; break;
                        }
                        if(IsBetween(to, start, end))
                        {
                            newTo = start - 1;
                            if (IsBetween(from, start, end))
                            {
                                newFrom = end + 1;
                            }
                            found = true; break;
                        }
                    }

                    if(found)
                    {
                        for (var d = newFrom; d <= newTo; d++)
                        {
                            var exists = false;
                            foreach (var (start, end) in sections)
                            {
                                if (IsBetween(d, start, end))
                                {
                                    d = end;
                                    exists = true; break;
                                }
                            }
                            if (exists) continue;
                            //Console.WriteLine($"{d}/{to}");
                            Add(new Point { X = d, Y = y, Type = '#' });
                        }
                    }
                    else
                    {
                        ConfirmedNoAtSlice += totalBlocks;
                        sections.Add((from, to));
                    }

                }

                //Console.WriteLine($"Confirmed: {ConfirmedNoAtSlice}");
            }

            public void ExclusionRealFast(Point s, Point b, int slice)
            {
                if (b.Y == slice)
                {
                    if(cells.ContainsKey(b.X))
                        cells[b.X]--;
                    else
                    {
                        cells.Add(b.X, -1);
                    }
                }

                var distance = Math.Abs(s.X - b.X) + Math.Abs(s.Y - b.Y);
                var target = -distance;

                for (var i = distance; i >= target; i--)
                {
                    var y = s.Y - i;
                    if (y != slice) continue;
                    var from = s.X - (distance - Math.Abs(i));
                    var to = s.X + (distance - Math.Abs(i));
                    //Console.WriteLine($"From {from} to {to}");
                    for(var j = from; j <= to; j++)
                    {
                        if(cells.ContainsKey(j))
                        {
                            cells[j]++;
                        }
                        else
                        {
                            cells.Add(j, 1);
                        }
                    }


                }
            }

            public bool IsBetween(int a, int start, int end)
            {
                return(a >= start && a <= end);
            }

            public void Print()
            {
                for(var y = Min.Y; y <= Max.Y; y++)
                {
                    Console.WriteLine($"{y,2} {theMap[y]}");
                }
            }

            public void Generate()
            {
                for (var y = Min.Y; y <= Max.Y; y++)
                {
                    var s = "";
                    for (var x = Min.X; x <= Max.X; x++)
                    {
                        var index = Points.FindIndex(p => p.X == x && p.Y == y);
                        if (index > -1)
                        {
                            var p = Points[index];
                            s += p.Type;
                        }
                        else
                        {
                            s += '.';
                        }
                    }
                    theMap.Add(y, s);
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
