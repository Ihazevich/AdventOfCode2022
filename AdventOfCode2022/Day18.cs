using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using static AdventOfCode2022.Day17;

namespace AdventOfCode2022
{
    internal class Day18
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
            var t = new Stopwatch();
            t.Start();

            var dropletScan = File.ReadAllLines(input);
            var part1 = "";
            var part2 = "";

            var droplet = new Droplet();

            foreach(var cubeScan in dropletScan)
            {
                var cube = new DropletScanCube
                {
                    Coords = cubeScan.Split(",").Select(s => int.Parse(s)).ToArray()
                };
                droplet.Add(cube);                
            }

            var exposedSides = 0;

            foreach(var c in droplet.Cubes)
            {
                exposedSides += 6 - c.Neighbours;
            }
            part1 = $"{exposedSides}";



            return new Tuple<string, string>(part1, part2);
        }

        public class DropletScanCube
        {
            public int[] Coords;
            public int Neighbours;
        }

        public class Droplet
        {
            public List<DropletScanCube> Cubes = new();

            public int ExposedSides = 0;

            public void Add(DropletScanCube cube)
            {
                foreach(var c in Cubes)
                {
                    //var adjacent = new int[3];
                    var total = 0;
                    for(int i = 0; i < 3; i++)
                    {
                        total += Math.Abs(cube.Coords[i] - c.Coords[i]);
                    }
                    if(total == 1)
                    {
                        cube.Neighbours++;
                        c.Neighbours++;
                    }
                }
                Cubes.Add(cube);
            }
        }
    }
}
