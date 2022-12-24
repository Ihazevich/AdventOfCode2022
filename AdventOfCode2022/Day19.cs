using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using static AdventOfCode2022.Day17;
using static AdventOfCode2022.Day18;
using static AdventOfCode2022.Day19;

namespace AdventOfCode2022
{
    internal class Day19
    {
        public static Object _lock = new Object();
        public static Stopwatch Watch = new Stopwatch();

        public static int MINUTES = 24;
        public const int MATERIALS = 5;

        public static int Branches = 0;

        public static Tuple<string, string> Solve(string input)
        {            
            var blueprintsInput = File.ReadAllLines(input);
            var part1 = "";
            var part2 = "";

            var availableBlueprints = new List<Blueprint>();

            var index = 0;
            foreach(var blueprint in blueprintsInput)
            {
                index++;
                var cMaterials = blueprint.Split(" Each ");

                var newBlueprint = new Blueprint();
                newBlueprint.Id = index;

                var robotOre = new Robot(Material.ORE);
                var costIndex = cMaterials[1].IndexOf("costs ") + 5;
                var mIndex = cMaterials[1].IndexOf(" ore");
                var amount = cMaterials[1].Substring(costIndex, mIndex - costIndex);
                robotOre.ConstructionMaterials[(int)Material.ORE] = int.Parse(amount);
                newBlueprint.Robots[0] = robotOre.ConstructionMaterials;

                var robotClay = new Robot(Material.CLAY);
                costIndex = cMaterials[2].IndexOf("costs ") + 5;
                mIndex = cMaterials[2].IndexOf("ore");
                amount = cMaterials[2].Substring(costIndex, mIndex - costIndex);
                robotClay.ConstructionMaterials[(int)Material.ORE] = int.Parse(amount);
                newBlueprint.Robots[1] = robotClay.ConstructionMaterials;

                var robotObs = new Robot(Material.OBSIDIAN);
                costIndex = cMaterials[3].IndexOf("costs ") + 5;
                mIndex = cMaterials[3].IndexOf("ore");
                amount = cMaterials[3].Substring(costIndex, mIndex - costIndex);
                robotObs.ConstructionMaterials[(int)Material.ORE] = int.Parse(amount);
                costIndex = cMaterials[3].IndexOf("and ") + 3;
                mIndex = cMaterials[3].IndexOf("clay");
                amount = cMaterials[3].Substring(costIndex, mIndex - costIndex);
                robotObs.ConstructionMaterials[(int)Material.CLAY] = int.Parse(amount);
                newBlueprint.Robots[2] = robotObs.ConstructionMaterials;

                var robotGeo = new Robot(Material.GEODE);
                costIndex = cMaterials[4].IndexOf("costs ") + 5;
                mIndex = cMaterials[4].IndexOf("ore");
                amount = cMaterials[4].Substring(costIndex, mIndex - costIndex);
                robotGeo.ConstructionMaterials[(int)Material.ORE] = int.Parse(amount);
                costIndex = cMaterials[4].IndexOf("and ") + 3;
                mIndex = cMaterials[4].IndexOf("obsidian");
                amount = cMaterials[4].Substring(costIndex, mIndex - costIndex);
                robotGeo.ConstructionMaterials[(int)Material.OBSIDIAN] = int.Parse(amount);
                newBlueprint.Robots[3] = robotGeo.ConstructionMaterials;

                availableBlueprints.Add(newBlueprint);
            }

            var total = 0;
            
            Parallel.For(0, availableBlueprints.Count, b =>
            {
                var factory = new Factory(availableBlueprints[b]);
                factory.NextCycle();
                Interlocked.Add(ref total, availableBlueprints[b].QualityLevel);
                Console.WriteLine($"{availableBlueprints[b].Id} Quality: {availableBlueprints[b].QualityLevel:N0} B:{availableBlueprints[b].Branches:N0}");
            });
            part1 = $"{total}";

            total = 1;
            MINUTES = 32;
            
            Parallel.For(0, 3, b =>
            {
                availableBlueprints[b].MaxGeodes = 0;
                availableBlueprints[b].Branches = 0;
                var factory = new Factory(availableBlueprints[b]);
                factory.NextCycle();
                lock (_lock)
                {
                    total *= availableBlueprints[b].MaxGeodes;
                }
                Console.WriteLine($"{availableBlueprints[b].Id} Geodes: {availableBlueprints[b].MaxGeodes:N0} B:{availableBlueprints[b].Branches:N0}");
            });
            part2 = $"{total}";
            
            return new Tuple<string, string>(part1, part2);       
        }

        public class Robot
        {
            public Material Type;
            public int[] ConstructionMaterials = new int[MATERIALS];

            public Robot(Material type)
            {
                Type = type;
            }
        }

        public enum Material
        {
            ORE,
            CLAY,
            OBSIDIAN,
            GEODE,
            NONE
        }

        public class Blueprint
        {
            public int Id;
            public int[][] Robots = new int[MATERIALS][];
            public int MaxGeodes;
            public int QualityLevel => MaxGeodes * Id;
            public ulong Branches;

            public Blueprint()
            {
                Robots[4] = new int[5];
            }
        }

        public class Factory
        {
            public static int MaxGeodes;

            public bool[] BuildingRobot = new bool[MATERIALS];
            public int[] ActiveRobots = new int[MATERIALS];
            public Blueprint ActiveBlueprint;
            public int[] MaterialStock = new int[MATERIALS];

            public int CurrentMinutes;

            public Factory(Blueprint activeBlueprint)
            {
                ActiveRobots[0]++;
                ActiveBlueprint = activeBlueprint;
            }

            public Factory()
            {
            }

            public void Collect()
            {
                for(var r = 0; r < MATERIALS-1; r++)
                {
                    MaterialStock[r] += ActiveRobots[r];
                }
                ActiveBlueprint.MaxGeodes = Math.Max(MaterialStock[(int)Material.GEODE], ActiveBlueprint.MaxGeodes);
            }

            public void Build()
            {
                var building = 0;
                for(var r = 0; r < MATERIALS; r++)  
                {
                    //var canBuild = true;
                    BuildingRobot[r] = true;
                    for (var c = 0; c < MATERIALS; c++)
                    {
                        if (ActiveBlueprint.Robots[r][c] > MaterialStock[c])
                        {
                            BuildingRobot[r] = false;
                            building++;
                        }
                            //canBuild = false;
                    }
                }

                if (BuildingRobot[(int)Material.GEODE])
                {
                    BuildingRobot = new bool[MATERIALS];
                    BuildingRobot[(int)Material.GEODE] = true;
                }
                else if (BuildingRobot[(int)Material.OBSIDIAN])
                {
                    BuildingRobot = new bool[MATERIALS];
                    BuildingRobot[(int)Material.OBSIDIAN] = true;
                }                
                else if (CurrentMinutes > 3*MINUTES/4)
                {
                    BuildingRobot[(int)Material.ORE] = false;
                    BuildingRobot[(int)Material.CLAY] = false;
                }
                
                for (var r = 0; r < MATERIALS; r++)
                {
                    if (BuildingRobot[r])
                    {
                        Branch(r);
                    }
                }
            }

            public void Branch(int robotType)
            {
                ActiveBlueprint.Branches++;
                var factory = new Factory();
                factory.ActiveBlueprint= ActiveBlueprint;
                factory.CurrentMinutes= CurrentMinutes;
                Array.Copy(ActiveRobots, factory.ActiveRobots, MATERIALS);
                Array.Copy(MaterialStock, factory.MaterialStock, MATERIALS);

                for (var c = 0; c < MATERIALS-1; c++)
                {
                    // if (factory.ActiveBlueprint.Robots[robotType].ConstructionMaterials[c] == 0) continue;
                    factory.MaterialStock[c] -= factory.ActiveBlueprint.Robots[robotType][c];
                }
                factory.BuildingRobot[robotType] = true;

                factory.Collect();
                factory.Deploy();
                factory.NextCycle();
            }

            public void NextCycle()
            {
                CurrentMinutes++;

                var maxGeodes = MaterialStock[(int)Material.GEODE];

                for(var m = CurrentMinutes; m <= MINUTES; m++)
                {
                    maxGeodes += ActiveRobots[(int)Material.GEODE] + (m - CurrentMinutes);
                }

                if (maxGeodes < ActiveBlueprint.MaxGeodes || CurrentMinutes > MINUTES)
                {
                    return;
                }
                Build();
            }

            public void Deploy()
            {
                for(var r = 0; r < MATERIALS; r++)
                {
                    if (BuildingRobot[r])
                    {
                        ActiveRobots[r]++;
                        BuildingRobot[r] = false;
                    }
                }
            }
        }
    }
}
