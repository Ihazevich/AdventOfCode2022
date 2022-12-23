using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using static AdventOfCode2022.Day17;
using static AdventOfCode2022.Day18;

namespace AdventOfCode2022
{
    internal class Day18
    {
        public static Tuple<string, string> Solve(string input)
        {            
            var dropletScan = File.ReadAllLines(input);
            var part1 = "";
            var part2 = "";

            var droplet = new Droplet();

            foreach(var cubeScan in dropletScan)
            {
                var cube = new DropletScanCube
                {
                    Coords = cubeScan.Split(",").Select(s => int.Parse(s)).ToArray(),
                    Material = CubeMaterial.LAVA
                };
                droplet.AddLava(cube);                
            }

            var exposedSides = 0;

            foreach(var c in droplet.Cubes)
            {
                exposedSides += 6 - c.Neighbours;
            }
            part1 = $"{exposedSides}";

            droplet.AddAir();
            droplet.FillWithWater();

            var totalLavaPockets = droplet.Cubes.Count(c => c.Material == CubeMaterial.LAVA);
            var totalAirPockets = droplet.Cubes.Count(c => c.Material == CubeMaterial.AIR);
            var totalWPockets = droplet.Cubes.Count(c => c.Material == CubeMaterial.WATER);
            var totalPockets = droplet.Cubes.Count();

            droplet.RecalculateExposure();

            exposedSides = 0;

            foreach (var c in droplet.Cubes.Where(c => c.Material == CubeMaterial.LAVA))
            {
                exposedSides += 6 - c.Neighbours;
            }
            part2 = $"{exposedSides}";


            return new Tuple<string, string>(part1, part2);
        }

        public class DropletScanCube
        {
            public int[] Coords;
            public int Neighbours;
            public CubeMaterial Material;
        }

        public enum CubeMaterial
        {
            LAVA,
            AIR,
            WATER
        }

        public class Droplet
        {
            public List<DropletScanCube> Cubes = new();

            public int[] Max = new int[] { int.MinValue, int.MinValue, int.MinValue };
            public int[] Min = new int[] { int.MaxValue, int.MaxValue, int.MaxValue };

            public void AddLava(DropletScanCube cube)
            {
                foreach(var c in Cubes.Where(c => c.Material == CubeMaterial.LAVA))
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

                for(int i = 0; i < 3; i++)
                {
                    Max[i] = Math.Max(Max[i], cube.Coords[i]);
                    Min[i] = Math.Min(Min[i], cube.Coords[i]);
                }

                Cubes.Add(cube);
            }
            
            public bool HasMaterialAt(int x, int y, int z, CubeMaterial material)
            {
                return Cubes.Any(c => c.Material == material && c.Coords[0] == x && c.Coords[1] == y && c.Coords[2] == z);
            }

            public void AddAir()
            {                
                for(var z = Min[2]; z <= Max[2]; z++)
                {
                    for (var y = Min[1]; y <= Max[1]; y++)
                    {
                        for (var x = Min[0]; x <= Max[0]; x++)
                        {
                            if (HasMaterialAt(x, y, z, CubeMaterial.LAVA)) continue;
                            var airCube = new DropletScanCube
                            {
                                Coords = new int[] { x,y,z },
                                Material = CubeMaterial.AIR
                            };
                            Cubes.Add(airCube);
                        }
                    }
                }
            }

            public void ChangeMaterial(int x, int y, int z, CubeMaterial material)
            {
                var index = Cubes.FindIndex(c => c.Coords[0] == x && c.Coords[1] == y && c.Coords[2] == z);

                if (index == -1) throw new Exception();

                Cubes[index].Material = material;
            }

            public void FillWithWater()
            {
                // Round 1
                for(var z = Min[2]; z <= Max[2]; z++)
                {
                    for (var y = Min[1]; y <= Max[1]; y++)
                    {
                        for (var x = Min[0]; x <= Max[0]; x++)
                        {
                            if (HasMaterialAt(x, y, z, CubeMaterial.LAVA)) continue;
                            
                            if (x == Min[0] || x == Max[0]) ChangeMaterial(x, y, z, CubeMaterial.WATER);
                            if (y == Min[1] || y == Max[1]) ChangeMaterial(x, y, z, CubeMaterial.WATER);
                            if (z == Min[2] || z == Max[2]) ChangeMaterial(x, y, z, CubeMaterial.WATER);

                            if (HasMaterialAt(x, y, z, CubeMaterial.WATER))
                            {
                                for(var nz = z - 1; nz <= z + 1; nz++)
                                {
                                    for (var ny = y - 1; ny <= y + 1; ny++)
                                    {
                                        for (var nx = x - 1; nx <= x + 1; nx++)
                                        {
                                            if ((nz != z && ny != y) || (nz != z && nx != x) || (nx != x && ny != y)) continue;

                                            if (HasMaterialAt(nx, ny, nz, CubeMaterial.AIR))
                                            {
                                                ChangeMaterial(nx, ny, nz, CubeMaterial.WATER);
                                            }
                                        }
                                    }
                                }
                            }

                            else if (HasMaterialAt(x, y, z, CubeMaterial.AIR))
                            {
                                for (var nz = z - 1; nz <= z + 1; nz++)
                                {
                                    for (var ny = y - 1; ny <= y + 1; ny++)
                                    {
                                        for (var nx = x - 1; nx <= x + 1; nx++)
                                        {
                                            if ((nz != z && ny != y) || (nz != z && nx != x) || (nx != x && ny != y)) continue;

                                            if (HasMaterialAt(nx, ny, nz, CubeMaterial.WATER))
                                            {
                                                ChangeMaterial(x, y, z, CubeMaterial.WATER);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Round 2 Reverse
                for (var z = Max[2]; z >= Min[2]; z--)
                {
                    for (var y = Max[1]; y >= Min[1]; y--)
                    {
                        for (var x = Max[0]; x >= Min[0]; x--)
                        {
                            if (HasMaterialAt(x, y, z, CubeMaterial.LAVA)) continue;

                            if (HasMaterialAt(x, y, z, CubeMaterial.WATER))
                            {
                                for (var nz = z - 1; nz <= z + 1; nz++)
                                {
                                    for (var ny = y - 1; ny <= y + 1; ny++)
                                    {
                                        for (var nx = x - 1; nx <= x + 1; nx++)
                                        {
                                            if ((nz != z && ny != y) || (nz != z && nx != x) || (nx != x && ny != y)) continue;

                                            if (HasMaterialAt(nx, ny, nz, CubeMaterial.AIR))
                                            {
                                                ChangeMaterial(nx, ny, nz, CubeMaterial.WATER);
                                            }
                                        }
                                    }
                                }
                            }

                            else if (HasMaterialAt(x, y, z, CubeMaterial.AIR))
                            {
                                for (var nz = z - 1; nz <= z + 1; nz++)
                                {
                                    for (var ny = y - 1; ny <= y + 1; ny++)
                                    {
                                        for (var nx = x - 1; nx <= x + 1; nx++)
                                        {
                                            if ((nz != z && ny != y) || (nz != z && nx != x) || (nx != x && ny != y)) continue;

                                            if (HasMaterialAt(nx, ny, nz, CubeMaterial.WATER))
                                            {
                                                ChangeMaterial(x, y, z, CubeMaterial.WATER);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public void RecalculateExposure()
            {
                foreach (var c1 in Cubes.Where(c => c.Material == CubeMaterial.LAVA))
                {
                    foreach (var c2 in Cubes.Where(c => c.Material == CubeMaterial.AIR))
                    {
                        var total = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            total += Math.Abs(c1.Coords[i] - c2.Coords[i]);
                        }
                        if (total == 1)
                        {
                            c1.Neighbours++;
                            if (c1.Neighbours > 6) throw new Exception();
                        }
                    }
                }
            }
        }
    }
}
