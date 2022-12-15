using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AdventOfCode2022
{
    internal class Day13
    {
        public static int sum = 0;
        public static int total = 0;
        public static bool verbose = true;

        public static Tuple<string, string> Solve(string input)
        {
            var packets = File.ReadAllLines(input);
            var pairs = 0;
            var part1 = "";
            var part2 = "";
            var sumOfRight = 0;

            var dataPackets = new List<DataPacket>();

            for (var i = 0; i < packets.Length;i++)
            {
                int packetIndex = (i + 1) / 3;
                if ((i + 1) % 3 == 0)
                {
                    ComparePackets(dataPackets[dataPackets.Count-2], dataPackets.Last());
                    Console.WriteLine("-----------");
                    continue;
                }
                Console.WriteLine($"{packets[i]}");
                dataPackets.Add(new DataPacket() { Index = (i + 3) / 3, CurrentLevel = -1 });
                var activePacketNode = new DataNode() { Level = -1 };

                var level = 0;
                var maxLevel = 0;

                dataPackets.Last().Add(activePacketNode);

                var firstClose = false;
                var packet = packets[i].Replace(" ", "");

                for (var j = 0; j < packet.Length; j++)
                {
                    if (packet[j] == '[')
                    {
                        maxLevel = level > maxLevel ? level : maxLevel;

                        var index = dataPackets.Last().Nodes.Count(p => p.Level == level);

                        var n = new DataNode() { Parent = activePacketNode, Level = level, Index = index };
                        dataPackets.Last().Add(n);
                        activePacketNode.FirstChild = activePacketNode.FirstChild == null ? n : activePacketNode.FirstChild;
                        activePacketNode = n;
                        level++;
                        if (packet[j+1] == ']')
                        {
                            var n2 = new DataNode()
                            {
                                Parent = activePacketNode,
                                Level = level,
                                Index = dataPackets.Last().Nodes.Count(p => p.Level == level),
                                Value = null,
                                Next = false
                            };
                            activePacketNode.FirstChild = activePacketNode.FirstChild == null ? n2 : activePacketNode.FirstChild;
                            dataPackets.Last().Add(n2);
                            level--;
                            activePacketNode.Next = j < packet.Length - 2 && packet[j + 2] == ',';
                            activePacketNode = activePacketNode?.Parent;
                            j++;
                        }
                    }
                    else if (packet[j] == ']')
                    {
                        firstClose = true;
                        level--;
                        activePacketNode.Next = j < packet.Length - 1 && packet[j + 1] == ',';
                        activePacketNode = activePacketNode?.Parent;
                    }
                    else if (packet[j] != ',')
                    {
                        firstClose = true;
                        var checkClose = packet.IndexOf(']', j);
                        var checkOpen = packet.IndexOf('[', j);
                        checkOpen = checkOpen == -1 ? packet.Length : checkOpen;
                        var nextStop = checkClose < checkOpen ? checkClose : checkOpen;
                        var elements = packet.Substring(j, nextStop - j).Split(',');
                        var index = dataPackets.Last().Nodes.Count(p => p.Level == level);
                        foreach (var element in elements.Where(e => !string.IsNullOrEmpty(e)))
                        {
                            var n = new DataNode() {
                                Parent = activePacketNode,
                                Level = level,
                                Index = index,
                                Value = int.TryParse(element.ToString(), out _) ? int.Parse(element.ToString()) : null,
                                Next = elements.Count(e => !string.IsNullOrEmpty(e)) > 1
                            };
                            activePacketNode.FirstChild = activePacketNode.FirstChild == null ? n : activePacketNode.FirstChild;
                            dataPackets.Last().Add(n);
                            index++;
                        }
                        dataPackets.Last().Nodes.Last().Next = elements.Count() != elements.Count(e => !string.IsNullOrEmpty(e));
                        j = nextStop - 1;
                    }
                }

                if (i == packets.Length - 1)
                {
                    ComparePackets(dataPackets[dataPackets.Count - 2], dataPackets.Last());
                    continue;
                }
            }

            part1 = $"Sum:{sum} Total:{total}";
            return new Tuple<string, string>(part1, part2);

        }


        private static (DataPacket, DataPacket) ComparePackets(DataPacket p1, DataPacket p2)
        {
            if (p1.Ended)
            {
                return (p1, p2);
            }

            DataNode? node1;
            DataNode? node2;

            if (!p1.Nodes.Any(n => n.Level == p1.CurrentLevel && !n.Explored) && !p1.FoundValue)
            {
                if (p1.CurrentLevel > 0)
                {
                    p1.LevelIndex[p1.CurrentLevel]++;
                    p1.CurrentLevel--;
                    if(verbose) Console.WriteLine("P1 Going up");
                    ComparePackets(p1, p2);
                }
            }
            else if(!p1.FoundValue)
            {
                node1 = p1.Nodes.First(n => n.Level == p1.CurrentLevel && n.Index == p1.LevelIndex[p1.CurrentLevel]);
                // If it has a child
                if (node1.FirstChild != null)
                {
                    // If not explored
                    if (!node1.Explored)
                    {
                        p1.CurrentLevel++;
                        if (!p1.LevelIndex.ContainsKey(p1.CurrentLevel)) p1.LevelIndex.Add(p1.CurrentLevel, 0);
                        node1.Explored = true;
                        if (verbose) Console.WriteLine("P1 Going down");
                        (p1,p2) = ComparePackets(p1, p2);
                    }
                    // Does it have a next element on the same level?
                    else if (node1.Next)
                    {
                        p1.LevelIndex[p1.CurrentLevel]++;
                        if (verbose) Console.WriteLine("P1 Going next");
                        (p1, p2) = ComparePackets(p1, p2);
                    }
                    // Can't go down or side, go back up a level
                    else
                    {
                        p1.LevelIndex[p1.CurrentLevel]++;
                        p1.CurrentLevel--;
                        if (verbose) Console.WriteLine("P1 Going up");
                        (p1, p2) = ComparePackets(p1, p2);
                    }

                }
                // No Child, explored
                else if (node1.Explored)
                {
                    // Does it have a next element on the same level?
                    if (node1.Next)
                    {
                        p1.LevelIndex[p1.CurrentLevel]++;
                        if (verbose) Console.WriteLine("P1 Going next");
                        (p1, p2) = ComparePackets(p1, p2);
                    }
                    // Can't go side, go back up a level
                    else
                    {
                        p1.LevelIndex[p1.CurrentLevel]++;
                        p1.CurrentLevel--;
                        if (verbose) Console.WriteLine("P1 Going up");
                        (p1, p2) = ComparePackets(p1, p2);
                    }
                }
                // No Child, not explored
                else
                {
                    p1.Value = node1.Value;
                    p1.HasNext = node1.Next;
                    p1.FoundValue = true;
                    if (verbose) Console.WriteLine($"P1 Found value {p1.Value!}");
                    node1.Explored = true;
                    return (p1, p2);
                }
            }

            if (p1.Ended)
            {
                return (p1, p2);
            }

            if (!p2.Nodes.Any(n => n.Level == p2.CurrentLevel && !n.Explored) && !p2.FoundValue)
            {
                if (p2.CurrentLevel > 0)
                {
                    p2.LevelIndex[p2.CurrentLevel]++;
                    p2.CurrentLevel--;
                    if (verbose) Console.WriteLine("P2 Going up");
                    (p1, p2) = ComparePackets(p1, p2);
                }
            }
            else if (!p2.FoundValue) 
            {
                node2 = p2.Nodes.First(n => n.Level == p2.CurrentLevel && n.Index == p2.LevelIndex[p2.CurrentLevel]);

                // If it has a child
                if (node2.FirstChild != null)
                {
                    // If not explored
                    if (!node2.Explored)
                    {
                        p2.LevelIndex[p2.CurrentLevel]++;
                        p2.CurrentLevel++;
                        if (!p2.LevelIndex.ContainsKey(p2.CurrentLevel)) p2.LevelIndex.Add(p2.CurrentLevel, 0);
                        node2.Explored = true;
                        if (verbose) Console.WriteLine("P2 Going down");
                        (p1, p2) = ComparePackets(p1, p2);
                    }
                    // Does it have a next element on the same level?
                    else if (node2.Next)
                    {
                        p2.LevelIndex[p2.CurrentLevel]++;
                        if (verbose) Console.WriteLine("P2 Going next");
                        (p1, p2) = ComparePackets(p1, p2);
                    }
                    // Can't go down or side, go back up a level
                    else
                    {
                        p2.LevelIndex[p2.CurrentLevel]++;
                        p2.CurrentLevel--;
                        if (verbose) Console.WriteLine("P2 Going up");
                        (p1, p2) = ComparePackets(p1, p2);
                    }

                }
                // No Child, explored
                else if (node2.Explored)
                {
                    // Does it have a next element on the same level?
                    if (node2.Next)
                    {
                        p2.LevelIndex[p2.CurrentLevel]++;
                        if (verbose) Console.WriteLine("P2 Going next");
                        (p1, p2) = ComparePackets(p1, p2);
                    }
                    // Can't go side, go back up a level
                    else
                    {
                        p2.LevelIndex[p2.CurrentLevel]++;
                        p2.CurrentLevel--;
                        if (verbose) Console.WriteLine("P2 Going up");
                        (p1, p2) = ComparePackets(p1, p2);
                    }
                }
                // No Child, not explored
                else
                {
                    p2.Value = node2.Value;
                    p2.FoundValue = true;
                    p2.HasNext = node2.Next;
                    node2.Explored = true;
                    if (verbose) Console.WriteLine($"P2 Found value {p2.Value!}");
                    return (p1, p2);
                }
            }
            
            if (p1.Ended)
            {
                return (p1, p2);
            }

            if(p2.FoundValue)
            {
                if(p1.FoundValue)
                {
                    if(p1.Value == p2.Value)
                    {
                        Console.WriteLine($"Equals [{p1.Value}]-[{p2.Value}]");

                        if (p1.CurrentLevel > p2.CurrentLevel)
                        {
                            p1.Ended = true;
                            Console.WriteLine($"Right ended, not right [{p1.Value}]-[{p2.Value}]");
                            return (p1, p2);
                        }
                        else if (p1.CurrentLevel < p2.CurrentLevel)
                        {
                            p1.Ended = true;
                            sum += p1.Index;
                            total++;
                            Console.WriteLine($"Left ended, is right {p1.Index} [{p1.Value}]-[{p2.Value}] +{p1.Index}={sum}");
                            return (p1, p2);
                        }


                        if ((p1.HasNext && !p2.HasNext))
                        {
                            p1.Ended = true;
                            Console.WriteLine($"Right ended, not right [{p1.Value}]-[{p2.Value}]");
                            return (p1, p2);
                        }

                        if ((!p1.HasNext && p2.HasNext))
                        {
                            p1.Ended = true;
                            sum += p1.Index;
                            total++;
                            Console.WriteLine($"Left ended, is right {p1.Index} [{p1.Value}]-[{p2.Value}] +{p1.Index}={sum}");
                            return (p1, p2);
                        }


                        p1.Value = null; p2.Value = null;
                        p1.FoundValue = false; p2.FoundValue = false;

                        (p1, p2) = ComparePackets(p1, p2);
                    }
                    else if (p1.Value > p2.Value)
                    {
                        p1.Ended = true;
                        Console.WriteLine($"Left higher, not right [{p1.Value}]-[{p2.Value}]");
                        return (p1, p2);
                    }
                    else if (p1.Value < p2.Value)
                    {
                        p1.Ended = true;
                        sum += p1.Index;
                        total++;
                        Console.WriteLine($"Left lower, is right [{p1.Value}]-[{p2.Value}] +{p1.Index}={sum}");
                        return (p1, p2);
                    }
                    else if(p1.Value == null )
                    {
                        p1.Ended = true;
                        sum += p1.Index;
                        total++;
                        Console.WriteLine($"Left ended, is right {p1.Index} [{p1.Value}]-[{p2.Value}] +{p1.Index}={sum}");
                        return (p1, p2);
                    }
                    else if (p2.Value == null)
                    {
                        p1.Ended = true;
                        Console.WriteLine($"Right ended, not right [{p1.Value}]-[{p2.Value}]");
                        return (p1, p2);
                    }
                }
                else
                {
                    p1.Ended = true;
                    sum += p1.Index;
                    total++;
                    Console.WriteLine($"Left ended, is right {p1.Index} [{p1.Value}]-[{p2.Value}] +{p1.Index}={sum}");
                    return (p1, p2);
                }
            }
            else
            {
                p1.Ended = true;
                Console.WriteLine($"Right ended, not right [{p1.Value}]-[{p2.Value}]");
                return (p1, p2);
            }
            /**
            if (p2.Value == null )
            {
                p1.Ended = true;
                Console.WriteLine($"Right ended, not right [{p1.Value}-{p2.Value}]");
                return (p1, p2);
            } 
            else if (p1.Value == null )
            {
                p1.Ended = true;
                sum += p1.Index;
                Console.WriteLine($"Left ended, is right {p1.Index} [{p1.Value}-{p2.Value}] +{p1.Index}={sum}");
                return (p1, p2);
            } 
            else if (p1.Value > p2.Value)
            {
                p1.Ended = true;
                Console.WriteLine($"Left higher, not right [{p1.Value}-{p2.Value}]");
                return (p1, p2);
            }
            else if (p1.Value < p2.Value)
            {
                p1.Ended = true;
                sum += p1.Index;
                Console.WriteLine($"Left lower, is right [{p1.Value}-{p2.Value}] +{p1.Index}={sum}");
                return (p1, p2);
            }
            Console.WriteLine($"Equals [{p1.Value}-{p2.Value}]");
            p1.Value = null; p2.Value = null;
            p1.FoundValue = false; p2.FoundValue = false;

            (p1, p2) = ComparePackets(p1, p2);
            **/

            return (p1, p2);
        }

        private class DataNode
        {
            public DataNode? Parent { get; set; }
            public DataNode? FirstChild { get; set; }
            public bool Next { get; set; }
            public int? Value { get; set; }
            public int Level { get; set; }
            public int Index { get; set; }
            public bool Explored { get; set; }

        }

        private class DataPacket
        {
            public List<DataNode> Nodes { get; set; }
            public Dictionary<int,int> LevelIndex { get; set; }
            public int LastLevel { get; set; }
            public int CurrentLevel { get; set; }
            public bool Ended { get; set; }
            public int Index { get; set; }
            public int? Value { get; set; }
            public bool FoundValue { get; set; }
            public bool HasNext { get; set; }

            public DataPacket()
            {
                Nodes = new List<DataNode>();
                LevelIndex = new Dictionary<int,int>();
                LevelIndex.Add(-1, 0);
            }

            public void Add(DataNode node)
            {
                Nodes.Add(node);
            }
        }



    }
}
