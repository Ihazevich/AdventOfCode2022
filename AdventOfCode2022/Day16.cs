using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode2022
{
    internal class Day16
    {
        public static bool Verbose = true;

        public static int MaxWorthValves = 0;
        public static int MaxTotalP1 = 0;
        public static int MaxTotalP2 = 0;
        public static int Branches;

        public static Stopwatch Watch = new();

        public static Tuple<string, string> Solve(string input)
        {
            var t = new Stopwatch();
            t.Start();
            var scans = File.ReadAllLines(input);
            var part1 = "";
            var part2 = "";
            var startIndex = 0;

            var valveData = new ValveData[scans.Length];

            var valvesP1 = new Valve[scans.Length];
            var valvesP2 = new Valve[scans.Length];

            var indexes = new Dictionary<string, int>();

            for(var s = 0; s < scans.Length; s++)
            {                
                var valve = new ValveData();
                valve.Name = scans[s].Substring(6, 2).Trim();
                indexes.Add(valve.Name, s);
                var stop = scans[s].IndexOf(";");
                valve.FlowRate = int.Parse(scans[s].Substring(23, stop - 23));
                MaxWorthValves += valve.FlowRate > 0 ? 1 : 0;
                stop = scans[s].IndexOf(" ", scans[s].LastIndexOf("valve"));
                valve.Exits = scans[s].Substring(stop + 1).Replace(" ","").Split(",");
                valveData[s] = valve;

                if (valve.Name == "AA") startIndex = s;
            }


            for(var v = 0; v < valveData.Length; v++)
            {
                var valveP1 = new Valve(valveData[v].Exits.Length);
                valveP1.Opened = valveData[v].FlowRate == 0;
                valveP1.FlowRate = valveData[v].FlowRate;

                for (var e = 0; e < valveData[v].Exits.Length; e++)
                {
                    valveP1.ExitsP1[e] = indexes[valveData[v].Exits[e]];
                }

                valvesP1[v] = valveP1;

                var valveP2 = new Valve(valveData[v].Exits.Length, valveData[v].Exits.Length);
                valveP2.Opened = valveData[v].FlowRate == 0;
                valveP2.FlowRate = valveData[v].FlowRate;

                for (var e = 0; e < valveData[v].Exits.Length; e++)
                {
                    valveP2.Exits[0][e] = indexes[valveData[v].Exits[e]];
                    valveP2.Exits[1][e] = indexes[valveData[v].Exits[e]];
                }

                valvesP2[v] = valveP2;
            }

            Watch.Start();
            if (Verbose) Console.WriteLine($"Starting Part 1 at {valveData[startIndex].Name}");
            Valve.SearchBestPathPart1(valvesP1, startIndex, -1, 0, 0, 0, 0);
            if (Verbose) Console.WriteLine($"Max: {MaxTotalP1} | Branches: {Branches:N0}");
            part1 = $"{MaxTotalP1} Search Space:{Branches:N0}";
            Branches = 0;
            Watch.Restart();
            if (Verbose) Console.WriteLine($"Starting Part 2 at {valveData[startIndex].Name}");
            int[] start = { startIndex, startIndex };
            int[] prev = { -1, -1 };
            Valve.SearchBestPathPart2(valvesP2, start, prev, 0, 0, 0, 0, 0);
            if (Verbose) Console.WriteLine($"Max: {MaxTotalP2} | Search Space: {Branches:N0}");

            part2 = $"{MaxTotalP2} Search Space:{Branches:N0}";
            return new Tuple<string, string>(part1, part2);
        }

        private class ValveData
        {
            public string Name;
            public int FlowRate;
            public string[] Exits;
        }

        private class Valve 
        {
            public int[][] Exits = new int[2][];
            public int FlowRate;
            public bool Opened;
            public bool[] Visited = new bool[2];
            public bool VisitedP1;
            public int[] ExitsP1;
            
            public Valve(int size)
            {
                ExitsP1 = new int[size];
            }

            public Valve(int size1, int size2)
            {
                Exits[0] = new int[size1];
                Exits[1] = new int[size2];
            }

            public static void SearchBestPathPart1(Valve[] valves, int cIndex, int pIndex, int min, int total, int oValves, int cFlowRate)
            {
                Branches++;

                if (Verbose && Branches % 100000 == 0)
                {
                    Watch.Stop();
                    double seconds = Watch.ElapsedTicks / (double)Stopwatch.Frequency;
                    Console.WriteLine($"  - Branches: {Branches:N0} | Max: {MaxTotalP1:N0} | In {seconds:N4}s.");
                    Watch.Restart();
                }
                
                min++;

                if (min > 30)
                {
                    MaxTotalP1 = Math.Max(total, MaxTotalP1);
                    return;

                }
                else if (min == 30)
                {
                    total += cFlowRate;
                    MaxTotalP1 = Math.Max(total, MaxTotalP1);
                    return;
                }

                total += cFlowRate;
                valves[cIndex].VisitedP1 = true;

                var flows = new List<int>();

                foreach (var v in valves)
                {
                    if (!v.Opened)
                        flows.Add(v.FlowRate);
                }

                flows.Sort();

                var maxFlow = cFlowRate;
                var maxPossible = total;

                for (var m = min + 1; m <= 30; m++)
                {
                    if (flows.Count() > 0)
                    {
                        maxFlow += flows.Last();
                        flows.Remove(flows.Last());
                    }
                    if (flows.Count() > 0)
                    {
                        maxFlow += flows.Last();
                        flows.Remove(flows.Last());
                    }
                    maxPossible += maxFlow;
                }

                if (maxPossible <= MaxTotalP1) return;

                bool openedNow = false;
                var bTotal = total;
                int flowRateChange = 0;

                if (!valves[cIndex].Opened)
                {
                    cFlowRate += valves[cIndex].FlowRate;
                    flowRateChange = valves[cIndex].FlowRate;
                    valves[cIndex].Opened = true;
                    oValves++;
                    openedNow = true;
                }

                if (oValves == MaxWorthValves)
                {
                    total += cFlowRate * (30 - min);
                    MaxTotalP1 = Math.Max(total, MaxTotalP1);
                    return;
                }

                var choices = new List<Decision>();
                
                var exits = valves[cIndex].ExitsP1.Where(e => e != -1).ToArray();
                var count = exits.Count();


                if (openedNow)
                {
                    var decision = new Decision { MoveTo = cIndex, Opened = true, Delete = -1, Exit = -1 };
                    choices.Add(decision);
                }

                for (var e = 0; e < count; e++)
                {
                    if ((valves[exits[e]].VisitedP1 && !openedNow)) { continue; }

                    if (exits[e] != pIndex)
                    {
                        var decision = new Decision { MoveTo = exits[e], Opened = false, Delete = -1, Exit = e };
                        choices.Add(decision);
                    }
                    else if (exits[e] == pIndex && count == 1)
                    {
                        var i = 0;
                        var found = false;
                        for (i = 0; i < valves[pIndex].ExitsP1.Length; i++)
                        {
                            if (valves[pIndex].ExitsP1[i] == cIndex)
                            {
                                found = true;
                                break;
                            }
                        }
                        i = found ? i : -1;
                        var decision = new Decision { MoveTo = exits[e], Opened = false, Delete = i, Exit = e };
                        choices.Add(decision);
                    }

                }

                if (choices.Count == 0)
                {
                    total += cFlowRate * (30 - min);
                    MaxTotalP1 = Math.Max(total, MaxTotalP1);
                    return;
                }

                foreach (var choiceMan in choices)
                {
                    Valve[] newValves = new Valve[valves.Length];

                    for (var v = 0; v < valves.Length; v++)
                    {
                        newValves[v] = new(valves[v].ExitsP1.Length);
                        newValves[v].FlowRate = valves[v].FlowRate;
                        newValves[v].Opened = valves[v].Opened;
                        Array.Copy(valves[v].ExitsP1, newValves[v].ExitsP1, newValves[v].ExitsP1.Length);
                        newValves[v].VisitedP1 = choiceMan.Opened ? false : valves[v].VisitedP1;
                    }

                    newValves[cIndex].VisitedP1 = true;

                    var newFlowRate = cFlowRate;
                    var newOValves = oValves;

                    if (choiceMan.Opened != openedNow)
                    {
                        newValves[cIndex].Opened = false;
                        newFlowRate -= flowRateChange;
                        newOValves--;
                    }

                    if (choiceMan.Delete > -1)
                    {
                        newValves[pIndex].ExitsP1[choiceMan.Delete] = -1;
                    }

                    SearchBestPathPart1(
                        newValves,
                        choiceMan.MoveTo,
                        choiceMan.MoveTo == cIndex ? pIndex : cIndex,
                        min,
                        total,
                        newOValves,
                        newFlowRate
                    );
                }
            }

            public static void SearchBestPathPart2(Valve[] valves, int[] cIndex, int[] pIndex, int min, int total,  int oValves, int cFlowRate, int ticktickBoom)
            {
                Branches++;

                if (Verbose && Branches % 1_000_000 == 0)
                {
                    Watch.Stop();
                    double seconds = Watch.ElapsedTicks / (double)Stopwatch.Frequency;
                    Console.WriteLine($"  - Branches: {Branches:N0} | Max: {MaxTotalP2:N0} | In {seconds:N4}s.");
                    Watch.Restart();
                }
                
                min++;
                ticktickBoom++;

                if (min > 26)
                {
                    MaxTotalP2 = Math.Max(total, MaxTotalP2);
                    return;

                } else if (min == 26)
                {
                    total += cFlowRate;
                    MaxTotalP2 = Math.Max(total, MaxTotalP2);
                    return;
                }

                total += cFlowRate;
                valves[cIndex[0]].Visited[0] = true;
                valves[cIndex[1]].Visited[1] = true;

                var flows = new List<int>();

                foreach (var v in valves)
                {
                    if (!v.Opened)
                        flows.Add(v.FlowRate);
                }

                flows.Sort();
                
                var maxFlow = cFlowRate;
                var maxPossible = total;
                for (var m = min+1; m <= 26; m++)
                {
                    if (flows.Count() > 0)
                    {
                        maxFlow += flows.Last();
                        flows.Remove(flows.Last()); 
                    }
                    if(flows.Count()>0)
                    {
                        maxFlow += flows.Last();
                        flows.Remove(flows.Last());
                    }
                    maxPossible += maxFlow;
                }

                if (maxPossible <= MaxTotalP2) return;

                bool[] openedNow = { false, false };
                var bTotal = total;
                int[] flowRateChange = { 0, 0 };

                if (!valves[cIndex[0]].Opened)
                {
                    cFlowRate += valves[cIndex[0]].FlowRate;
                    flowRateChange[0] = valves[cIndex[0]].FlowRate;
                    valves[cIndex[0]].Opened = true;
                    oValves++;
                    openedNow[0] = true;
                    ticktickBoom = 0;
                }

                if (!valves[cIndex[1]].Opened)
                {
                    cFlowRate += valves[cIndex[1]].FlowRate;
                    flowRateChange[1] = valves[cIndex[1]].FlowRate;
                    valves[cIndex[1]].Opened = true;
                    oValves++;
                    openedNow[1] = true;
                    ticktickBoom = 0;
                }

                if (oValves == MaxWorthValves || ticktickBoom > 4)
                {
                    total += cFlowRate * (26 - min);
                    MaxTotalP2 = Math.Max(total, MaxTotalP2);
                    return;
                }

                List<Decision>[] choices = { new List<Decision>(), new List<Decision>() };

                for (var w = 0; w < 2; w++)
                {
                    var exits = valves[cIndex[w]].Exits[w].Where(e => e != -1).ToArray();
                    var count = exits.Count();

                    if (openedNow[w])
                    {
                        var decision = new Decision { MoveTo = cIndex[w], Opened = true, Delete = -1, Exit = -1 };
                        choices[w].Add(decision);
                    }
                    
                    for(var e = 0; e < count; e++)
                    {
                        if ((valves[exits[e]].Visited[w] && !openedNow[w])) { continue; }
                        
                        if (exits[e] != pIndex[w])
                        {
                            var decision = new Decision { MoveTo = exits[e], Opened = false, Delete = -1, Exit = e };
                            choices[w].Add(decision);
                        }
                        else if (exits[e] == pIndex[w] && count == 1)
                        {
                            var i = 0;
                            var found = false;
                            for (i = 0; i < valves[pIndex[w]].Exits[w].Length; i++)
                            {
                                if (valves[pIndex[w]].Exits[w][i] == cIndex[w])
                                {
                                    found = true;
                                    break;
                                }
                            }
                            i = found ? i : -1;
                            var decision = new Decision { MoveTo = exits[e], Opened = false, Delete = i, Exit = e };
                            choices[w].Add(decision);
                        }
                        
                    }
                }

                if (choices[0].Count == 0)
                {
                    var decision = new Decision { MoveTo = cIndex[0], Opened = openedNow[0], Delete = -1, Exit = -1 };
                    choices[0].Add(decision);

                }

                if (choices[1].Count == 0)
                {
                    var decision = new Decision { MoveTo = cIndex[1], Opened = openedNow[1], Delete = -1, Exit = -1 };
                    choices[1].Add(decision);
                }

                var explored = new List<(int, int)>();

                foreach (var choiceMan in choices[0])
                {
                    foreach (var choiceElephant in choices[1])
                    {
                        if (explored.Contains((choiceElephant.MoveTo, choiceMan.MoveTo)) || explored.Contains((choiceMan.MoveTo, choiceElephant.MoveTo))) continue;
                        if (choiceMan.MoveTo == choiceElephant.MoveTo) continue;
                        
                        Valve[] newValves = new Valve[valves.Length];
                        
                        for (var v = 0; v < valves.Length; v++)
                        {
                            newValves[v] = new(valves[v].Exits[0].Length, valves[v].Exits[1].Length);
                            newValves[v].FlowRate = valves[v].FlowRate;
                            newValves[v].Opened = valves[v].Opened;
                            Array.Copy(valves[v].Exits[0], newValves[v].Exits[0], newValves[v].Exits[0].Length);
                            Array.Copy(valves[v].Exits[1], newValves[v].Exits[1], newValves[v].Exits[1].Length);
                            newValves[v].Visited[0] = choiceMan.Opened ? false : valves[v].Visited[0];
                            newValves[v].Visited[1] = choiceElephant.Opened ? false : valves[v].Visited[1];
                        }

                        newValves[cIndex[0]].Visited[0] = true;
                        newValves[cIndex[1]].Visited[1] = true;

                        var newFlowRate = cFlowRate;
                        var newOValves = oValves;

                        if (choiceMan.Opened != openedNow[0])
                        {
                            newValves[cIndex[0]].Opened = false;
                            newFlowRate -= flowRateChange[0];
                            newOValves--;
                        }

                        if (choiceElephant.Opened != openedNow[1])
                        {
                            newValves[cIndex[1]].Opened = false;
                            newFlowRate -= flowRateChange[1];
                            newOValves--;
                        }
                        
                        if (choiceMan.Delete > -1)
                        {
                            newValves[pIndex[0]].Exits[0][choiceMan.Delete] = -1;
                            newValves[pIndex[0]].Exits[1][choiceMan.Delete] = -1;
                        }
                        if (choiceElephant.Delete > -1)
                        {
                            newValves[pIndex[1]].Exits[0][choiceElephant.Delete] = -1;
                            newValves[pIndex[1]].Exits[1][choiceElephant.Delete] = -1;
                        }
                        
                        int[] nextIndexes = { choiceMan.MoveTo, choiceElephant.MoveTo };
                        int[] prevIndexes = { 
                            choiceMan.MoveTo == cIndex[0] ? pIndex[0] : cIndex[0],
                            choiceElephant.MoveTo == cIndex[1] ? pIndex[1] : cIndex[1]
                        };

                        SearchBestPathPart2(
                            newValves,
                            nextIndexes,
                            prevIndexes,
                            min,
                            total,
                            newOValves,
                            newFlowRate,
                            ticktickBoom
                        );
                        
                        explored.Add((choiceMan.MoveTo, choiceElephant.MoveTo));
                    }
                }
            }
        }

        private class Decision
        {
            public int MoveTo;
            public bool Opened;
            public int Delete;
            public int Exit;
        }
       
    }
}
