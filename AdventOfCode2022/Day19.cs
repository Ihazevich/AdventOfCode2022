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
        public static Stopwatch Watch = new Stopwatch();

        public const int MINUTES = 24;
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
                newBlueprint.Robots[0] = robotOre;

                var robotClay = new Robot(Material.CLAY);
                costIndex = cMaterials[2].IndexOf("costs ") + 5;
                mIndex = cMaterials[2].IndexOf("ore");
                amount = cMaterials[2].Substring(costIndex, mIndex - costIndex);
                robotClay.ConstructionMaterials[(int)Material.ORE] = int.Parse(amount);
                newBlueprint.Robots[1] = robotClay;

                var robotObs = new Robot(Material.OBSIDIAN);
                costIndex = cMaterials[3].IndexOf("costs ") + 5;
                mIndex = cMaterials[3].IndexOf("ore");
                amount = cMaterials[3].Substring(costIndex, mIndex - costIndex);
                robotObs.ConstructionMaterials[(int)Material.ORE] = int.Parse(amount);
                costIndex = cMaterials[3].IndexOf("and ") + 3;
                mIndex = cMaterials[3].IndexOf("clay");
                amount = cMaterials[3].Substring(costIndex, mIndex - costIndex);
                robotObs.ConstructionMaterials[(int)Material.CLAY] = int.Parse(amount);
                newBlueprint.Robots[2] = robotObs;

                var robotGeo = new Robot(Material.GEODE);
                costIndex = cMaterials[4].IndexOf("costs ") + 5;
                mIndex = cMaterials[4].IndexOf("ore");
                amount = cMaterials[4].Substring(costIndex, mIndex - costIndex);
                robotGeo.ConstructionMaterials[(int)Material.ORE] = int.Parse(amount);
                costIndex = cMaterials[4].IndexOf("and ") + 3;
                mIndex = cMaterials[4].IndexOf("obsidian");
                amount = cMaterials[4].Substring(costIndex, mIndex - costIndex);
                robotGeo.ConstructionMaterials[(int)Material.OBSIDIAN] = int.Parse(amount);
                newBlueprint.Robots[3] = robotGeo;

                availableBlueprints.Add(newBlueprint);
            }

            var total = 0;

            //foreach(var blueprint in availableBlueprints)
            Parallel.For(0, availableBlueprints.Count, b =>
            //for(var b = 0; b < availableBlueprints.Count; b++)
            {
                //Branches = 0;
                //Watch.Start();
                Console.WriteLine($"Initiaing Blueprint {availableBlueprints[b].Id}");
                var factory = new Factory(availableBlueprints[b]);
                factory.NextCycle();
                Interlocked.Add(ref total, availableBlueprints[b].QualityLevel);
                //Console.WriteLine(availableBlueprints[b].BestLog);
                Console.WriteLine($"{availableBlueprints[b].Id} Quality: {availableBlueprints[b].QualityLevel:N0} B:{availableBlueprints[b].Branches:N0}");
            });


            part1 = $"{total}";

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
            public Robot[] Robots = new Robot[MATERIALS];
            public int MaxGeodes;
            public int QualityLevel => MaxGeodes * Id;
            //public List<string> BestLog;
            public ulong Branches;

            public Blueprint()
            {
                Robots[4] = new Robot(Material.NONE);
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
            //public List<string> Log = new();

            public Factory(Blueprint activeBlueprint)
            {
                ActiveRobots[0]++;
                ActiveBlueprint = activeBlueprint;
            }

            public Factory(Blueprint activeBlueprint, int minutes)
            {
                CurrentMinutes = minutes;
                ActiveBlueprint = activeBlueprint;
            }

            public void Collect()
            {
                for(var r = 0; r < MATERIALS-1; r++)
                {
                    MaterialStock[r] += ActiveRobots[r];
                    //Log.Add($"{ActiveRobots[r]} {((Material)r).ToString()}-collecting robot collects {ActiveRobots[r]} ore; you now have {MaterialStock[r]} {((Material)r).ToString()}");
                }

                //var prevMax = ActiveBlueprint.MaxGeodes;
                ActiveBlueprint.MaxGeodes = Math.Max(MaterialStock[(int)Material.GEODE], ActiveBlueprint.MaxGeodes);
                /*if (ActiveBlueprint.MaxGeodes != prevMax)
                {
                    ActiveBlueprint.BestLog = new();
                    Log.ForEach(l => {
                        Console.WriteLine(l);
                        ActiveBlueprint.BestLog.Add(l);
                    });
                }*/
            }

            public void Build()
            {         
                for(var r = 0; r < MATERIALS; r++)  
                {
                    var canBuild = true;

                    for (var c = 0; c < MATERIALS; c++)
                    {
                        if (ActiveBlueprint.Robots[r].ConstructionMaterials[c] > MaterialStock[c])
                            canBuild = false;
                    }

                    if(canBuild)
                    {
                        Branch(r);
                    }
                }
            }

            public void Branch(int robotType)
            {
                ActiveBlueprint.Branches++;
                /*if (Branches % 15_000_000 == 0)
                {
                    Watch.Stop();
                    double seconds = Watch.ElapsedTicks / (double)Stopwatch.Frequency;
                    Console.WriteLine($" - {ActiveBlueprint.Id}-{Branches:N0}: {ActiveBlueprint.MaxGeodes:N0} in {seconds:N4}s.");
                    Watch.Restart();
                }*/
                var factory = new Factory(ActiveBlueprint, CurrentMinutes);
                //Log.ForEach(l => { factory.Log.Add(new string(l)); });
                Array.Copy(ActiveRobots, factory.ActiveRobots, MATERIALS);
                Array.Copy(MaterialStock, factory.MaterialStock, MATERIALS);


                //var s = $"Spend ";
                for (var c = 0; c < MATERIALS; c++)
                {
                    if (factory.ActiveBlueprint.Robots[robotType].ConstructionMaterials[c] == 0) continue;
                    factory.MaterialStock[c] -= factory.ActiveBlueprint.Robots[robotType].ConstructionMaterials[c];
                    //s += $"{factory.ActiveBlueprint.Robots[robotType].ConstructionMaterials[c]} {((Material)c).ToString()} ";
                    //if (factory.MaterialStock[c] < 0) throw new Exception();
                }
                //s += $"to start building a {((Material)robotType).ToString()}-collecting robot";

                //factory.Log.Add(s);
                factory.BuildingRobot[robotType] = true;

                factory.Collect();
                factory.Deploy();
                factory.NextCycle();
            }

            public void NextCycle()
            {
                CurrentMinutes++;
                var maxGeodes = MaterialStock[(int)Material.GEODE];
                var maxActive = ActiveRobots[(int)Material.GEODE];
                for(var m = CurrentMinutes; m <= MINUTES; m++)
                {
                    maxActive += (m - CurrentMinutes + 1) % 2;
                    maxGeodes += maxActive;
                }
                if (maxGeodes < ActiveBlueprint.MaxGeodes) return;
               
                if (CurrentMinutes > MINUTES)
                {
                    return;
                }
                //Log.Add($"== Minute {CurrentMinutes} ==");
                //DeployRobots();
                Build();
            }

            public void Deploy()
            {
                //var deployed = false;
                for(var r = 0; r < MATERIALS; r++)
                {
                    if (BuildingRobot[r])
                    {
                        //if (deployed) throw new Exception();
                        ActiveRobots[r]++;
                        BuildingRobot[r] = false;
                        // Log.Add($"The new {((Material)r).ToString()}-collecting robot is ready; you now have {ActiveRobots[r]} of them\n");
                    }
                }

            }
        }
    }
}
