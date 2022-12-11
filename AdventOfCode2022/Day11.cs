using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day11
    {
        public static Tuple<string, string> Solve(string input)
        {
            var monkeysInfo = File.ReadAllLines(input);

            var monkeyHoldsPart1 = new List<List<long>>();
            var monkeyHoldsPart2 = new List<List<double>>();
            var monkeyOperation = new List<(string, long)>();
            var monkeyTest = new List<(long, (int,int))>();
            var monkeyNumber = 0;
            var inspectedPart1 = new List<long>();
            var inspectedPart2 = new List<long>();
            double[] maxInspect = { 0, 0 };
            long mod = 1;

            for (int i = 0; i < monkeysInfo.Length; i++)
            {
                var line = monkeysInfo[i];
                if (line.StartsWith("Monkey"))
                {
                    var n = line.Substring(line.LastIndexOf("y") + 1, line.Length - 2 - line.LastIndexOf("y"));
                    monkeyNumber = int.Parse(n);
                    monkeyHoldsPart1.Add(new List<long>());
                    monkeyHoldsPart2.Add(new List<double>());
                    inspectedPart1.Add(0);
                    inspectedPart2.Add(0);
                }
                else if (line.Contains("Starting"))
                {
                    foreach(var num in line.Substring(line.LastIndexOf(":")+2).Split(", "))
                    {
                        monkeyHoldsPart1[monkeyNumber].Add(long.Parse(num));
                        monkeyHoldsPart2[monkeyNumber].Add(double.Parse(num));
                    }
                }
                else if(line.Contains("Operation"))
                {
                    var op = line.Substring(line.LastIndexOf("=") + 6, 1);
                    var n = line.Substring(line.LastIndexOf(op) + 1);
                    long qty = 0;
                    if(!long.TryParse(n, out qty))
                    {
                        op += "self";
                    }
                    monkeyOperation.Add((op, qty));
                }
                else if (line.Contains("Test"))
                {
                    var tst = long.Parse(line.Substring(line.LastIndexOf("y") + 2));
                    var nextTLine = monkeysInfo[i + 1];
                    var nextFLine = monkeysInfo[i + 2];
                    var t = int.Parse(nextTLine.Substring(nextTLine.LastIndexOf("y") + 1));
                    var f = int.Parse(nextFLine.Substring(nextFLine.LastIndexOf("y") + 1));
                    monkeyTest.Add((tst,(t, f)));
                    mod *= tst;
                }

            }

            for(var round = 0; round < 20; round++)
            {
                for(var m = 0; m < monkeyHoldsPart1.Count; m++)
                {
                    for (var i = 0; i < monkeyHoldsPart1[m].Count; i++)
                    {
                        var worry = monkeyHoldsPart1[m][i];
                        inspectedPart1[m]++;
                        switch (monkeyOperation[m].Item1)
                        {
                            case "*":
                                worry *= monkeyOperation[m].Item2;
                                break;
                            case "+":
                                worry += monkeyOperation[m].Item2;
                                break;
                            case "*self":
                                worry *= worry;
                                break;
                            case "+self":
                                worry += worry;
                                break;
                        }
                        worry /= 3;
                        worry %= mod;
                        if (worry % monkeyTest[m].Item1 == 0)
                        {
                            monkeyHoldsPart1[monkeyTest[m].Item2.Item1].Add(worry);
                        }
                        else
                        {
                            monkeyHoldsPart1[monkeyTest[m].Item2.Item2].Add(worry);
                        }
                    }
                    monkeyHoldsPart1[m] = new List<long>();
                }
                /**
                if ((round + 1) % 1000 == 0 || round == 0 || round == 19)
                {
                    Console.WriteLine($"== After round {round + 1} ==");
                    for (var m = 0; m < monkeyHoldsPart1.Count; m++)
                    {
                        Console.WriteLine($"Monkey {m} - Inspected {inspectedPart1[m]} items.");
                    }
                }
                **/
            }

            for (var m = 0; m < monkeyHoldsPart1.Count; m++)
            {
                if (inspectedPart1[m] > maxInspect[1])
                {
                    maxInspect[0] = maxInspect[1];
                    maxInspect[1] = inspectedPart1[m];
                }
                else if (inspectedPart1[m] > maxInspect[0])
                {
                    maxInspect[0] = inspectedPart1[m];
                }
            }

            var part1 = $"{maxInspect[0]},{maxInspect[1]}: {maxInspect[0] * maxInspect[1]}";
            maxInspect[0] = maxInspect[1] = 0;

            for (var round = 0; round < 10000; round++)
            {
                for (var m = 0; m < monkeyHoldsPart2.Count; m++)
                {
                    for (var i = 0; i < monkeyHoldsPart2[m].Count; i++)
                    {
                        var worry = monkeyHoldsPart2[m][i];
                        inspectedPart2[m]++;
                        switch (monkeyOperation[m].Item1)
                        {
                            case "*":
                                worry *= monkeyOperation[m].Item2;
                                break;
                            case "+":
                                worry += monkeyOperation[m].Item2;
                                break;
                            case "*self":
                                worry *= worry;
                                break;
                            case "+self":
                                worry += worry;
                                break;
                        }
                        worry %= mod;

                        if (worry % monkeyTest[m].Item1 == 0)
                        {
                            monkeyHoldsPart2[monkeyTest[m].Item2.Item1].Add(worry);
                        }
                        else
                        {
                            monkeyHoldsPart2[monkeyTest[m].Item2.Item2].Add(worry);
                        }
                    }
                    monkeyHoldsPart2[m] = new List<double>();
                }
                /**
                if((round+1) % 1000 == 0 || round == 0 || round == 19)
                {
                    Console.WriteLine($"== After round {round+1} ==");
                    for (var m = 0; m < monkeyHoldsPart2.Count; m++)
                    {                        
                        Console.WriteLine($"Monkey {m} - Inspected {inspectedPart2[m]} items.");
                    }
                }
                **/
            }

            for (var m = 0; m < monkeyHoldsPart2.Count; m++)
            {
                if (inspectedPart2[m] > maxInspect[1])
                {
                    maxInspect[0] = maxInspect[1];
                    maxInspect[1] = inspectedPart2[m];
                }
                else if (inspectedPart2[m] > maxInspect[0])
                {
                    maxInspect[0] = inspectedPart2[m];
                }
            }
            var part2 = $"{maxInspect[0]},{maxInspect[1]}: {maxInspect[0] * maxInspect[1]}";
            return new Tuple<string, string>(part1, part2);
        }
    }
}
