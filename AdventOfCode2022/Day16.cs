using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace AdventOfCode2022
{
    internal class Day16
    {
        public static List<int> Totals = new();
        public static List<string> TotalsPaths = new();
        public static int MaxWorthValves = 0;
        public static int MaxTotal = 0;
        public static List<Task<(int, string)>> Tasks = new();

        public static Tuple<string, string> Solve(string input)
        {
            var t = new Stopwatch();
            t.Start();
            var scans = File.ReadAllLines(input);
            var part1 = "";
            var part2 = "";
            var startIndex = 0;

            var valveData = new ValveData[scans.Length];
            var valves = new Valve[scans.Length];
            var indexes = new Dictionary<string, int>();

            for(var s = 0; s < scans.Length; s++)
            {                
                var valve = new ValveData();
                //valve.Index = s;
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
                var valve = new Valve();
                valve.Exits = new int[valveData[v].Exits.Length];
                valve.Opened = valveData[v].FlowRate == 0;
                valve.FlowRate = valveData[v].FlowRate;
                for (var e = 0; e < valveData[v].Exits.Length; e++)
                {
                    valve.Exits[e] = indexes[valveData[v].Exits[e]];
                }
                valves[v] = valve;
            }

            Console.WriteLine($"Starting at {valveData[startIndex].Name}");
            var r = Valve.SearchBestPath(valves, startIndex, -1, 0, 0, 0, 0);
            part1 = $"{r}";
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
            public int[] Exits;
            public int FlowRate;
            public bool Opened;
            public bool Visited;

            public static int SearchBestPath(Valve[] valves, int cIndex, int pIndex, int min, int total,  int oValves, int cFlowRate)
            {
                var result = 0;
                min++;
                if (min > 30)
                {
                    return total;
                } else if (min == 30)
                {
                    return total + cFlowRate;
                }
                total += cFlowRate;
                valves[cIndex].Visited = true;

                var openedNow = false;
                var bTotal = total;
                var bFlowRate = cFlowRate;

                if (!valves[cIndex].Opened)
                {
                    min++;
                    cFlowRate += valves[cIndex].FlowRate;
                    total += cFlowRate;
                    valves[cIndex].Opened = true;
                    oValves++;
                    openedNow = true;
                    
                }
                
                if (oValves == MaxWorthValves)
                {
                    total += cFlowRate * (30 - min);
                    return total;
                }
                
                var exits = valves[cIndex].Exits.Where(e => e != -1);
                var count = exits.Count();

                var gotResult = false;

                foreach (var index in exits)
                {
                    if (valves[index].Visited && !openedNow) { continue; }
                    var newCopy2 = new Valve[valves.Length];

                    if(openedNow)
                    {
                        for (var v = 0; v < valves.Length; v++)
                        {
                            newCopy2[v] = new();
                            newCopy2[v].Exits = new int[valves[v].Exits.Length];
                            newCopy2[v].FlowRate = valves[v].FlowRate;
                            newCopy2[v].Opened = valves[v].Opened;
                            newCopy2[v].Visited = false;
                            Array.Copy(valves[v].Exits, newCopy2[v].Exits, valves[v].Exits.Length);
                        }
                    }
                    else
                    {
                        for (var v = 0; v < valves.Length; v++)
                        {
                            newCopy2[v] = new();
                            newCopy2[v].Exits = new int[valves[v].Exits.Length];
                            newCopy2[v].FlowRate = valves[v].FlowRate;
                            newCopy2[v].Opened = valves[v].Opened;
                            newCopy2[v].Visited = valves[v].Visited;
                            Array.Copy(valves[v].Exits, newCopy2[v].Exits, valves[v].Exits.Length);
                        }
                    }

                    // If the only way forward is where we came from
                    if (count == 1 & index == pIndex)
                    {
                        var i = 0;
                        for (i = 0; i < newCopy2[index].Exits.Length; i++)
                        {
                            if (newCopy2[index].Exits[i].Equals(cIndex))
                                break;
                        }
                        // Eliminate the connection from previous to here
                        newCopy2[index].Exits[i] = -1;
                        var r3 = SearchBestPath(newCopy2, index, cIndex, min, total, oValves, cFlowRate);
                        result = Math.Max(result, r3);
                        gotResult = true;
                    }
                    // if our destination is different than where we came from
                    else if (index != pIndex)
                    {
                        // If there was a valve here, branch into opened and unopened scenarios
                        if (openedNow)
                        {
                            var newCopy = new Valve[valves.Length];
                            for (var v = 0; v < valves.Length; v++)
                            {
                                newCopy[v] = new();
                                newCopy[v].Exits = new int[valves[v].Exits.Length];
                                newCopy[v].FlowRate = valves[v].FlowRate;
                                newCopy[v].Opened = valves[v].Opened;
                                newCopy[v].Visited = newCopy[v].Visited;
                                Array.Copy(valves[v].Exits, newCopy[v].Exits, valves[v].Exits.Length);
                            }
                            newCopy[cIndex].Opened = false;
                            var r1 = SearchBestPath(newCopy, index, cIndex, min - 1, bTotal, oValves - 1, bFlowRate);
                            result = Math.Max(result, r1);
                            gotResult = true;
                            var r2 = SearchBestPath(newCopy2, index, cIndex, min, total, oValves, cFlowRate);
                            result = Math.Max(result, r2);
                            gotResult = true;
                        }
                        else
                        {
                            var r2 = SearchBestPath(newCopy2, index, cIndex, min, total, oValves, cFlowRate);
                            result = Math.Max(result, r2);
                            gotResult = true;
                        }
                    }
                }

                if(gotResult)
                {
                    return result;
                }
                else
                {
                    total += cFlowRate * (30 - min);
                    return total;
                }
            }
        }

       
    }
}
