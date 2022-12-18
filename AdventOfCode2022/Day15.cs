using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace AdventOfCode2022
{
    internal class Day15
    {
        //1337958778//1337958778 3138881-3364986
        private static Object _lock = new Object();

        public static Tuple<string, string> Solve(string input)
        {
            var t = new Stopwatch();
            t.Start();
            var scans = File.ReadAllLines(input);
            var part1 = "";
            var part2 = "";
            //var part1Slice = 10;
            var part1Slice = 2_000_000;
            //var part2Max = 20;
            var part2Max = 4_000_000;


            var i = 0;
            var points = new List<Point>();
            foreach (var scan in scans)
            {
                i++;
                var last = 0;
                var sensor = false;
                while (last < scan.Length)
                {
                    var next = scan.IndexOf(',', last);
                    last = scan.IndexOf('=', last) + 1;
                    var s = scan.Substring(last, next - last);
                    var x = int.Parse(s);
                    last = scan.IndexOf('=', last) + 1;
                    next = scan.IndexOf(':', last);
                    next = next < 0 ? scan.Length : next;
                    s = scan.Substring(last, next - last);
                    var y = int.Parse(s);
                    last = next;
                    if (!sensor)
                    {
                        points.Add(new Point { X = x, Y = y, Type = 'S' });
                        sensor = true;
                    }
                    else
                    {
                        var p = points.Last();
                        var distance = Math.Abs(p.X - x) + Math.Abs(p.Y - y);
                        points.Add(new Point { X = x, Y = y, Type = 'B', Distance = distance });
                        sensor = true;

                    }
                }
            }
            var map = new Map();

            for (var p = 0; p < points.Count - 1; p += 2)
            {
                map.ExclusionRealFast(points[p], points[p + 1], part1Slice);
            }

            t.Stop();
            part1 = $"{map.GetNotBeacons()} in {t.ElapsedMilliseconds}ms";

            Console.WriteLine(part1);
            Console.WriteLine($"Running Part 2");
            t.Restart();

            var counter = 0;
            
            Parallel.For(0, part2Max + 1, (s, state) =>
            {
                var part2Map = new Map(part2Max);
                var found = false;
                for (var p = 0; p < points.Count - 1; p += 2)
                {
                    found = part2Map.ExclusionReallyFast(points[p].X, points[p].Y, points[p + 1].X, points[p + 1].Y, s, part2Max, points[p + 1].Distance);
                    if (found) break;
                }
                var x = part2Map.FindBeaconInSlice(part2Max);
                if (x > -1)
                {
                    BigInteger frequency = (4_000_000 * x) + s;
                    t.Stop();
                    part2 = $"{x} {x}-{s} in {t.ElapsedMilliseconds}ms";
                    state.Break();
                }
                Interlocked.Increment(ref counter);
                if (counter % 10000 == 0)
                {
                    double time = t.ElapsedTicks;
                    BigInteger seconds = (BigInteger)time / Stopwatch.Frequency;
                    long remaining = (part2Max + 1 - counter) * (long)(time / counter);
                    long elapsed = counter * (long)(time / counter);
                    Console.WriteLine($"{counter} - {seconds:N2}s at {((seconds * 1000) / counter):N2}ms/slice Remaining = {new TimeSpan(elapsed)} Elpsed = {new TimeSpan(remaining)}");
                }
            });            

            return new Tuple<string, string>(part1, part2);
        }


        private class Map
        {
            public List<Point> Points = new List<Point>();
            public Dictionary<int, string> theMap = new Dictionary<int, string>();
            public Point Max = new Point { X = 0, Y = 0 };
            public Point Min = new Point { X = 0, Y = 0 };
            public List<(int, int)> sections = new List<(int, int)>();

            public Dictionary<int, int> cells = new Dictionary<int, int>();

            public int[] part2Cells = new int[0];

            public Map(int max = 0)
            {
                if (max > 0)
                {
                    part2Cells = new int[max + 1];
                }
            }

            public int GetNotBeacons()
            {
                int sum = 0;
                foreach (var (k, v) in cells)
                {
                    if (v > 0) sum++;
                }

                return sum;
            }

            public void ExclusionRealFast(Point s, Point b, int slice)
            {
                if (b.Y == slice)
                {
                    if (cells.ContainsKey(b.X))
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
                    for (var j = from; j <= to; j++)
                    {
                        if (cells.ContainsKey(j))
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

            public bool ExclusionReallyFast(int sX, int sY, int bX, int bY, int slice, int max, int distance)
            {
                var i = Math.Abs(sY - slice);
                distance -= i;
                var from = sX - distance;
                var to = sX + distance;
                from = Math.Max(0, from);
                to = Math.Min(to, max);

                for (var j = from; j <= to;)
                {                    
                    var index = j;
                    j = part2Cells[j] == 0 ? j + 1 : part2Cells[j];
                    part2Cells[index] = to + 1;
                }
                return false;
            }

            public int FindBeaconInSlice(int max)
            {
                for (var x = 0; x <= max;)
                {
                    if (part2Cells[x] == 0)
                        return x;
                    else
                        x = part2Cells[x];
                }
                return -1;
            }

        }

        private class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public char Type { get; set; }
            public int Distance { get; set; }
            public int From { get; set; }
            public int To { get; set; }

            public void SetDistance(int slice, int max, Point sensor)
            {
                var i = Math.Abs(sensor.Y - slice);
                Distance = Math.Abs(sensor.X - X) + Math.Abs(sensor.Y - Y) - i;
            }

        }
    }
}
