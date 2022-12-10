using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    public class AoC2022
    {
        public const int DAYS = 10;

        public static Tuple<string, string> DayX(string input)
        {
            var file = File.ReadAllLines(input);


            var part1 = $"";
            var part2 = $"";

            return new Tuple<string, string>(part1, part2);
        }

        public static Tuple<string, string> Day1(string input)
        {
            var file = File.ReadAllLines(input);
            var currentElf = 0;
            int[] top3  = { 0, 0 , 0 };

            foreach (var line in file)
            {
                if (int.TryParse(line, out int i))
                {
                    currentElf += i;
                }
                else
                {
                    if (currentElf > top3[2])
                    {
                        top3[0] = top3[1];
                        top3[1] = top3[2];
                        top3[2] = currentElf;
                    }
                    currentElf = 0;
                }
            }

            var part1 = $"{top3[2]}";
            var part2 = $"{top3[0] + top3[1] + top3[2]}";

            return new Tuple<string, string>(part1, part2);
        }

        public static Tuple<string, string> Day2(string input)
        {
            var file = File.ReadAllLines(input);

            var totalScore = 0;
            var totalExpectedScore = 0;

            foreach (var line in file)
            {
                var enemyMove = line[0];
                var myMove = line[2];

                var enemyScore = enemyMove == 'A' ? 1 : enemyMove == 'B' ? 2 : enemyMove == 'C' ? 3 : -1;
                var myScore = myMove == 'X' ? 1 : myMove == 'Y' ? 2 : myMove == 'Z' ? 3 : -1;
                var matchScore = ((enemyScore == 1 && myScore == 3) || (enemyScore == 2 && myScore == 1) || (enemyScore == 3 && myScore == 2)) ? 0 : enemyScore == myScore ? 3 : 6;

                totalScore += matchScore + myScore;

                var expectedResult = myScore;
                var expectedScore = 0;

                switch (expectedResult)
                {
                    case 1:
                        expectedScore = enemyScore == 1 ? 3 : enemyScore == 2 ? 1 : 2;
                        break;
                    case 2:
                        expectedScore = enemyScore;
                        break;
                    case 3:
                        expectedScore = enemyScore == 1 ? 2 : enemyScore == 2 ? 3 : 1;
                        break;
                }

                var expectedMatchScore = myScore == 1 ? 0 : myScore == 2 ? 3 : 6;
                totalExpectedScore += expectedScore + expectedMatchScore;
            }

            var part1 = $"{totalScore}";
            var part2 = $"{totalExpectedScore}";

            return new Tuple<string, string>(part1, part2);
        }

        public static Tuple<string, string> Day3(string input)
        {
            var rucksacks = File.ReadAllLines(input);
            var totalPriority = 0;

            // Part 1
            foreach (var rucksack in rucksacks)
            {
                var items = rucksack.Length / 2;

                var firstCompartment = rucksack.Substring(0, items);
                var secondCompartment = rucksack.Substring(items);

                var found = false;

                foreach (var item in firstCompartment)
                {
                    for (var i = 0; i < items; i++)
                    {
                        if (item == secondCompartment[i])
                        {
                            var priority = 0;
                            if ((int)secondCompartment[i] > 90)
                            {
                                priority = (int)secondCompartment[i] - 96;
                            }
                            else
                            {
                                priority = (int)secondCompartment[i] - 38;
                            }

                            totalPriority += priority;
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                }
            }

            // Part 2

            var totalPriority2 = 0;

            Parallel.For(0, rucksacks.Length / 3, index =>
            {
                var found = false;
                for (int i = 0; i < rucksacks[index * 3].Length; i++)
                {
                    for (int j = 0; j < rucksacks[index * 3 + 1].Length; j++)
                    {
                        if (rucksacks[index * 3][i] == rucksacks[index * 3 + 1][j])
                        {
                            for (int k = 0; k < rucksacks[index * 3 + 2].Length; k++)
                            {
                                if (rucksacks[index * 3][i] == rucksacks[index * 3 + 2][k])
                                {
                                    found = true;

                                    if ((int)rucksacks[index * 3][i] > 90)
                                    {
                                        Interlocked.Add(ref totalPriority2, (int)rucksacks[index * 3][i] - 96);
                                    }
                                    else
                                    {
                                        Interlocked.Add(ref totalPriority2, (int)rucksacks[index * 3][i] - 38);
                                    }
                                    break;
                                }
                            }
                        }
                        if (found) { break; }
                    }
                    if (found) { break; }
                }
            });

            var part1 = $"{totalPriority}";
            var part2 = $"{totalPriority2}";

            return new Tuple<string, string>(part1, part2);
        }

        public static Tuple<string, string> Day4(string input)
        {
            var assignments = File.ReadAllLines(input);

            var totalOverlaps = 0;
            var partialOverlaps = 0;

            foreach (var assignment in assignments)
            {
                var elf1Min = int.Parse(assignment.Substring(0, assignment.IndexOf('-')));
                var elf1Max = int.Parse(assignment.Substring(assignment.IndexOf('-') + 1, assignment.IndexOf(',') - assignment.IndexOf('-') - 1));

                var elf2Min = int.Parse(assignment.Substring(assignment.IndexOf(',') + 1, assignment.LastIndexOf('-') - assignment.IndexOf(',') - 1));
                var elf2Max = int.Parse(assignment.Substring(assignment.LastIndexOf('-') + 1, assignment.Length - 1 - assignment.LastIndexOf('-')));

                if (elf1Min <= elf2Min && elf1Max >= elf2Max)
                {
                    totalOverlaps++;
                    partialOverlaps++;
                }
                else if ((elf2Min <= elf1Min) && (elf2Max >= elf1Max))
                {
                    totalOverlaps++;
                    partialOverlaps++;
                }
                else if (elf1Min >= elf2Min && elf1Min <= elf2Max)
                {
                    partialOverlaps++;
                }
                else if (elf1Max <= elf2Max && elf1Max >= elf2Min)
                {
                    partialOverlaps++;
                }
                else if (elf2Min >= elf1Min && elf2Min <= elf1Max)
                {
                    partialOverlaps++;
                }
                else if (elf2Max <= elf1Max && elf2Max >= elf1Min)
                {
                    partialOverlaps++;
                }
            }

            var part1 = $"{totalOverlaps}";
            var part2 = $"{partialOverlaps}";

            return new Tuple<string, string>(part1, part2);
        }

        public static Tuple<string, string> Day5(string input)
        {
            var instructions = File.ReadAllLines(input);

            var stacksNumberLine = 0;
            var numberOfStacks = 0;
            var stacksPart1 = new List<List<char>>();
            var stacksPart2 = new List<List<char>>();

            foreach (var instruction in instructions)
            {
                stacksNumberLine++;
                if (instruction.StartsWith(" 1"))
                {
                    numberOfStacks = int.Parse(instruction[instruction.Length - 2].ToString());
                    break;
                }
            }

            for (int i = 0; i < numberOfStacks; i++)
            {
                stacksPart1.Add(new List<char>());
                stacksPart2.Add(new List<char>());
            }

            for (int i = stacksNumberLine - 2; i >= 0; i--)
            {
                for (int j = 0; j < numberOfStacks; j++)
                {
                    var instruction = instructions[i];

                    if ((j * 4) + 1 < instruction.Length)
                    {
                        var box = instruction[(j * 4) + 1];
                        if (box.ToString() != " ")
                        {
                            stacksPart1[j].Add(instructions[i][(j * 4) + 1]);
                            stacksPart2[j].Add(instructions[i][(j * 4) + 1]);
                        }
                    }
                }
            }

            for (int i = stacksNumberLine; i < instructions.Length; i++)
            {
                var instruction = instructions[i];

                if (instruction.StartsWith("move"))
                {
                    var qty = int.Parse(instruction.Substring(5, instruction.IndexOf('f') - 5));
                    var from = int.Parse(instruction.Substring(instruction.LastIndexOf('m') + 1, instruction.IndexOf('t') - 1 - (instruction.LastIndexOf('m') + 1)));
                    var to = int.Parse(instruction.Substring(instruction.LastIndexOf('o') + 1));

                    // Part 1
                    var tempqty = qty;
                    while(tempqty > 0)
                    {
                        var movingbox = stacksPart1[from - 1].Last();
                        stacksPart1[to - 1].Add(movingbox);
                        stacksPart1[from - 1].RemoveAt(stacksPart1[from -1].Count - 1);
                        tempqty--;
                    }

                    // Part 2
                    tempqty = qty;
                    while (tempqty > 0)
                    {
                        var movingIndex = stacksPart2[from - 1].Count - tempqty;
                        var movingbox = stacksPart2[from - 1][movingIndex];
                        stacksPart2[to - 1].Add(movingbox);
                        stacksPart2[from - 1].RemoveAt(movingIndex);
                        tempqty--;
                    }
                }
            }

            var part1 = "";
            var part2 = "";

            foreach (var stack in stacksPart1)
            {
                part1 += stack.Last();
            }

            foreach (var stack in stacksPart2)
            {
                part2 += stack.Last();
            }

            return new Tuple<string, string>(part1, part2);
        }

        public static Tuple<string, string> Day6(string input)
        {
            var datastream = File.ReadAllText(input);
            var dataBuffer = "";
            dataBuffer += datastream[0];

            var part1ran = false;
            var part1 = "";
            var part2 = "";

            for (var i = 0; i < datastream.Length; i++)
            {
                var repeat = false;    
                for(var j = 0; j < dataBuffer.Length; j++)
                {
                    if (dataBuffer[j] == datastream[i])
                    {
                        dataBuffer = dataBuffer.Substring(j+1);
                        dataBuffer += datastream[i];
                        repeat = true;
                        break;
                    }
                }
                if (repeat) continue;

                dataBuffer += datastream[i];

                if (dataBuffer.Length == 4 && !part1ran)
                {
                    part1 += $"{i + 1}";
                    part1ran = true;
                }

                if (dataBuffer.Length == 14 && part1ran)
                {
                    part2 += $"{i + 1}";
                    break;
                }
            }

            return new Tuple<string, string>(part1, part2);
        }

        public static Tuple<string, string> Day7(string input)
        {
            var terminalOutput = File.ReadAllLines(input);
            var listOfDirectories = new Dictionary<string,int>();
            var currentDirectory = "";

            foreach(var line in terminalOutput)
            {
                var fileSize = 0;

                if (line.StartsWith("$ cd .."))
                {
                    currentDirectory = currentDirectory.Remove(currentDirectory.LastIndexOf("/"));
                    var c = currentDirectory.Last();
                    while(c != '/')
                    {
                        currentDirectory = currentDirectory.Remove(currentDirectory.Length-1);
                        c = currentDirectory.Last();
                    }
                }
                else if (line.StartsWith("$ cd"))
                {
                    currentDirectory += line.Substring(5) + "/";
                    listOfDirectories.Add(currentDirectory,0);
                }
                else if (int.TryParse(line.Substring(0, line.IndexOf(" ")),out fileSize))
                {
                    listOfDirectories[currentDirectory] += fileSize;
                }
            }

            var updatedSizes = listOfDirectories;
            foreach(var (dir1,size1) in listOfDirectories)
            {
                foreach (var (dir2, size2) in listOfDirectories)
                {
                    if(dir1 != dir2)
                    {
                        if (dir2.StartsWith(dir1))
                        {
                            updatedSizes[dir1] += updatedSizes[dir2];
                        }
                    }
                }
            }

            var totalSize = 0;
            foreach(var (dir,size) in updatedSizes)
            {
                if (dir == "//") continue;
                if (size <= 100000)
                {
                    totalSize += size;
                }
            }

            var part1 = $"{totalSize}";

            var targetSize = 30000000;
            var remainingSize = 70000000 - listOfDirectories["//"];
            var deleteTarget = targetSize - remainingSize;
            var deleteTargets = new Dictionary<string, int>();

            foreach (var (dir, size) in updatedSizes)
            {
                if (size >= deleteTarget)
                {
                    deleteTargets.Add(dir, size);
                }
            }

            var min = deleteTargets.ElementAt(0).Value;
            foreach(var (dir,size) in deleteTargets)
            {
                if(size < min)
                {
                    min = size;
                }
            }

            var part2 = $"{min}";

            return new Tuple<string, string>(part1, part2);
        }


        public static Tuple<string, string> Day8(string input)
        {
            var map = File.ReadAllLines(input);

            var maxColHeights = new List<int>();
            var maxRowHeights = new List<int>();

            var visibleTrees = 0;

            foreach(var tree in map[0])
            {
                var height = int.Parse(tree.ToString());
                maxColHeights.Add(height);
                visibleTrees++;
            }

            foreach (var tree in map[map.Length-1])
            {
                var height = int.Parse(tree.ToString());
                visibleTrees++;
            }

            for (int i = 1; i < map.Length - 1; i++)
            {
                visibleTrees++;
                maxRowHeights.Add(int.Parse(map[i][0].ToString()));

                visibleTrees++;
            }

            maxRowHeights.Add(int.Parse(map[0][0].ToString()));

            for (int i = 1; i < map.Length-1; i++)
            {
                for (int j = 1; j < map[i].Length-1; j++)
                {
                    var height = int.Parse(map[i][j].ToString());

                    var thisTreeVisible = false;

                    if(height > maxColHeights[j])
                    {
                        thisTreeVisible = true;
                        maxColHeights[j] = height;
                    }
                    else
                    {
                        var visible = true;
                        for(int k = i+1; k < map.Length; k++)
                        {
                            if(height <= int.Parse(map[k][j].ToString()))
                            {
                                visible = false; break;
                            }
                        }
                        if(visible)
                        {
                            thisTreeVisible = true;
                        }
                    }

                    if(height > maxRowHeights[i])
                    {
                        thisTreeVisible = true;
                        maxRowHeights[i] = height;
                    }
                    else
                    {
                        var visible = true;
                        for (int k = j + 1; k < map[i].Length; k++)
                        {
                            if (height <= int.Parse(map[i][k].ToString()))
                            {
                                visible = false; break;
                            }
                        }
                        if (visible)
                        {
                            thisTreeVisible = true;
                        }
                    }

                    if(thisTreeVisible)
                    {
                        visibleTrees++;
                    }
                }
            }

            var part1 = $"{visibleTrees}";

            var maxScenicScore = 0;
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    var height = int.Parse(map[i][j].ToString());

                    var scoreU = 0;
                    var scoreD = 0;
                    var scoreL = 0;
                    var scoreR = 0;
                    for (int l = j+1; l < map[i].Length; l++)
                    {
                        var height2 = int.Parse(map[i][l].ToString());
                        if (height > height2)
                        {
                            scoreL++;
                        }
                        else
                        {
                            scoreL++;
                            break;
                        }
                    }

                    for (int r = j - 1; r >= 0; r--)
                    {
                        var height2 = int.Parse(map[i][r].ToString());
                        if (height > height2)
                        {
                            scoreR++;
                        }
                        else
                        {
                            scoreR++;
                            break;
                        }
                    }

                    for (int u = i - 1; u >= 0; u--)
                    {
                        var height2 = int.Parse(map[u][j].ToString());
                        if (height > height2)
                        {
                            scoreU++;
                        }
                        else
                        {
                            scoreU++;
                            break;
                        }
                    }

                    for (int d = i + 1; d < map.Length; d++)
                    {
                        var height2 = int.Parse(map[d][j].ToString());
                        if (height > height2)
                        {
                            scoreD++;
                        }
                        else
                        {
                            scoreD++;
                            break;
                        }
                    }

                    var scenicScore = scoreD * scoreU * scoreL * scoreR;

                    if(scenicScore > maxScenicScore)
                    {
                        maxScenicScore= scenicScore;
                    }
                }
            }

            var part2 = $"{maxScenicScore}";

            return new Tuple<string, string>(part1, part2);
        }

        public static Tuple<string, string> Day9(string input)
        {
            var movements = File.ReadAllLines(input);

            var knots = 10;
            var rope = new List<(int, int)>(knots);

            for(var i = 0; i < knots; i++)
            {
                rope.Add((0, 0));
            }

            var visitedPart1 = new Dictionary<(int, int), int>();
            var visitedPart2 = new Dictionary<(int, int), int>();

            visitedPart1.Add(rope[knots-1], 1);
            visitedPart2.Add(rope[knots - 1], 1);

            foreach (var movement in movements)
            {
                var distance = int.Parse(movement.Substring(movement.IndexOf(' ') + 1));

                for(var d = distance; d > 0; d--)
                {
                    var headPos = rope[0];

                    switch (movement[0])
                    {
                        case 'U':
                            headPos.Item2 += 1;
                            break;
                        case 'D':
                            headPos.Item2 -= 1;
                            break;
                        case 'L':
                            headPos.Item1 -= 1;
                            break;
                        case 'R':
                            headPos.Item1 += 1;
                            break;
                    }

                    rope[0] = headPos;

                    for (var k = 1; k < knots; k++)
                    {
                        var xDistance = rope[k - 1].Item1 - rope[k].Item1;
                        var yDistance = rope[k - 1].Item2 - rope[k].Item2;

                        while (Math.Abs(xDistance) > 1 || Math.Abs(yDistance) > 1)
                        {
                            var knotPos = rope[k];

                            // Horizontal correction
                            if (yDistance == 0)
                            {
                                knotPos.Item1 += xDistance / Math.Abs(xDistance);
                            }

                            // Vertical correction
                            if (xDistance == 0)
                            {
                                knotPos.Item2 += yDistance / Math.Abs(yDistance);
                            }

                            // Diagonal correction 1
                            if ((xDistance > 1 || xDistance < -1) && (yDistance != 0))
                            {
                                knotPos.Item1 += xDistance / Math.Abs(xDistance);
                                knotPos.Item2 += yDistance / Math.Abs(yDistance);
                            }
                            else if ((yDistance > 1 || yDistance < -1) && (xDistance != 0))
                            {
                                knotPos.Item1 += xDistance / Math.Abs(xDistance);
                                knotPos.Item2 += yDistance / Math.Abs(yDistance);
                            }

                            if(k == knots - 1)
                            {
                                if (visitedPart2.ContainsKey(knotPos))
                                {
                                    visitedPart2[knotPos]++;
                                }
                                else
                                {
                                    visitedPart2.Add(knotPos, 1);
                                }
                            } 
                            else if(k == 1)
                            {
                                if (visitedPart1.ContainsKey(knotPos))
                                {
                                    visitedPart1[knotPos]++;
                                }
                                else
                                {
                                    visitedPart1.Add(knotPos, 1);
                                }
                            }

                            rope[k] = knotPos;

                            xDistance = rope[k - 1].Item1 - rope[k].Item1;
                            yDistance = rope[k - 1].Item2 - rope[k].Item2;
                        }
                    }
                }
            }

            var part1 = $"{visitedPart1.Count}";
            var part2 = $"{visitedPart2.Count}";

            return new Tuple<string, string>(part1, part2);
        }


        public static Tuple<string, string> Day10(string input)
        {
            var commands = File.ReadAllLines(input);

            var cycles = 0;
            var register = 1;

            var runCpu = true;
            var commandRunning = false;
            var commandCycle = 0;
            var lastCommand = 0;
            var lastLine = 0;
            var firstSignal = false;

            var signalsStrenght = new List<int>();

            var crtWidth = 40;
            var crtHeight = 6;

            var crt = new List<string>(6);
            var currentRow = 0;
            var pixelPos = 0;

            crt.Add("");

            while(runCpu)
            {
                cycles++;

                if (commandRunning)
                {
                    if(commandCycle%2 == 0)
                    {
                        register += lastCommand;


                        if (lastLine == commands.Length)
                        {
                            runCpu = false;
                            break;
                        }
                        var command = commands[lastLine];

                        if (command.StartsWith("a"))
                        {
                            lastCommand = int.Parse(command.Substring(5));
                            commandRunning = true;
                            commandCycle = 1;
                        }
                        else
                        {
                            commandRunning = false;
                        }

                        lastLine++;
                    }
                    else
                    {
                        commandCycle--;
                    }
                }
                else
                {
                    if (lastLine == commands.Length)
                    {
                        runCpu = false;
                        break;
                    }
                    var command = commands[lastLine];
                    if (command.StartsWith("a"))
                    {
                        lastCommand = int.Parse(command.Substring(5));
                        commandRunning = true;
                        commandCycle = 1;
                    }
                    else
                    {
                        commandRunning = false;
                    }
                    lastLine++;
                }

                if (cycles % 20 == 0 && firstSignal)
                {
                    signalsStrenght.Add(cycles * register);
                    firstSignal = false;
                }
                else if ((cycles - 20) % 40 == 0)
                {
                    signalsStrenght.Add(cycles * register);
                }
                
                if (pixelPos == register - 1 || pixelPos == register || pixelPos == register + 1)
                {
                    crt[currentRow] += "#"; 
                }
                else
                {
                    crt[currentRow] += ".";
                }
                
                pixelPos++;
                if(pixelPos%crtWidth == 0) 
                {
                    pixelPos = 0;
                    currentRow++;
                    crt.Add("");
                    if (currentRow >= crtHeight)
                    {
                        break;
                    }
                }

            }

            var sum = 0;

            foreach(var signal in signalsStrenght)
            {
                sum += signal;
            }

            var part1 = $"{sum}";
            var part2 = "";
            foreach (var row in crt)
            {
                part2 += row + "\n";
            }

            return new Tuple<string, string>(part1, part2);
        }
    }
}
