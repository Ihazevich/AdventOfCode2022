using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace AdventOfCode2022
{
    public class AoC2022
    {
        public static void Day1()
        {
            Console.WriteLine("Advent Challenge 1");

            var file = File.ReadAllLines("input1.txt");

            var elves = new List<int>();

            var currentElf = 0;

            foreach (var line in file)
            {

                if (int.TryParse(line, out int i))
                {
                    currentElf += i;
                }
                else
                {
                    elves.Add(currentElf);
                    Console.WriteLine($"New elf! Total: {currentElf}");
                    currentElf = 0;
                }
            }

            var maxElf = 0;

            foreach (var elf in elves)
            {
                if (elf > maxElf)
                {
                    maxElf = elf;
                }
            }

            var topElves = elves.OrderByDescending(x => x).ToList();

            Console.WriteLine($"The max elf is: {maxElf}");
            Console.WriteLine($"The top three elves are {topElves[0]},{topElves[1]},{topElves[2]}");
            Console.WriteLine($"The total of the top three is {topElves[0] + topElves[1] + topElves[2]}");
        }

        public static void Day2()
        {
            var file = File.ReadAllLines("input2.txt");

            var totalScore = 0;
            var totalExpectedScore = 0;

            foreach (var line in file)
            {
                var enemyMove = line[0];
                var myMove = line[2];

                var enemyScore = enemyMove == 'A' ? 1 : enemyMove == 'B' ? 2 : enemyMove == 'C' ? 3 : -1;
                var myScore = myMove == 'X' ? 1 : myMove == 'Y' ? 2 : myMove == 'Z' ? 3 : -1;


                var matchScore = ((enemyScore == 1 && myScore == 3) || (enemyScore == 2 && myScore == 1) || (enemyScore == 3 && myScore == 2)) ? 0 : enemyScore == myScore ? 3 : 6;

                Console.WriteLine($"My score {myScore} - {enemyScore}: {myScore + matchScore}");
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
            Console.WriteLine($"Total Score:{totalScore}");
            Console.WriteLine($"Total Expected Score:{totalExpectedScore}");
        }

        public static void Day3()
        {
            var rucksacks = File.ReadAllLines("2022input3.txt");
            var totalPriority = 0;
            object keyAndLock = new();

            /**
            Parallel.ForEach(rucksacks, rucksack =>
            {
                var items = rucksack.Length/2;

                for(var i = 0; i < items; i++)
                {
                    if (rucksack[i] == rucksack[items + i])
                    {
                        var priority = 0;
                        if ((int)rucksack[i] > 90)
                        {
                            priority = (int)rucksack[i] - 96;
                        }
                        else
                        {
                            priority = (int)rucksack[i] - 38;
                        }
                        Console.WriteLine($"Found item {rucksack[i]} with priority {priority} in \n {rucksack.Substring(0,items)} \n {rucksack.Substring(items)}");
                        
                        Interlocked.Add(ref totalPriority, priority);
                    }
                }
            });
            **/

            var counter = 0;

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
                            counter++;
                            Console.WriteLine($"{counter}/{rucksacks.Length} - Found item {item} with priority {priority} in \n {firstCompartment} \n {secondCompartment}");

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

                if (!found)
                {
                    counter++;
                    Console.WriteLine($"{counter}/{rucksacks.Length} - Repeated item not found in \n {rucksack.Substring(0, items)} \n {rucksack.Substring(items)}");
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

            Console.WriteLine($"Sum of priorities is: {totalPriority}");
            Console.WriteLine($"Sum of p2 priorities is: {totalPriority2}");
        }

        public static void Day4()
        {
            var assignments = File.ReadAllLines("2022input4.txt");

            var totalOverlaps = 0;
            var partialOverlaps = 0;

            foreach (var assignment in assignments)
            {
                var elf1Min = assignment.Substring(0, assignment.IndexOf('-'));
                var elf1Max = assignment.Substring(assignment.IndexOf('-') + 1, assignment.IndexOf(',') - assignment.IndexOf('-') - 1);

                var elf2Min = assignment.Substring(assignment.IndexOf(',') + 1, assignment.LastIndexOf('-') - assignment.IndexOf(',') - 1);
                var elf2Max = assignment.Substring(assignment.LastIndexOf('-') + 1, assignment.Length - 1 - assignment.LastIndexOf('-'));

                Console.WriteLine($"{elf1Min} to {elf1Max} and {elf2Min} to {elf2Max}");

                var first = false;
                var second = false;

                if (int.Parse(elf1Min) <= int.Parse(elf2Min) && int.Parse(elf1Max) >= int.Parse(elf2Max))
                {
                    totalOverlaps++;
                    partialOverlaps++;
                    Console.WriteLine("-1 contains 2!");
                    first = true;
                }
                else if (int.Parse(elf2Min) <= int.Parse(elf1Min) && int.Parse(elf2Max) >= int.Parse(elf1Max))
                {
                    totalOverlaps++;
                    Console.WriteLine("-2 contains 1!");
                    partialOverlaps++;
                    second = true;
                }

                //Console.WriteLine($"-Total complete overlaps: {totalOverlaps}");

                // Part 2

                else if (int.Parse(elf1Min) >= int.Parse(elf2Min) && int.Parse(elf1Min) <= int.Parse(elf2Max))
                {
                    partialOverlaps++;
                    Console.WriteLine("--1 partially overlaps 2!");
                }
                else if (int.Parse(elf1Max) <= int.Parse(elf2Max) && int.Parse(elf1Max) >= int.Parse(elf2Min))
                {
                    partialOverlaps++;
                    Console.WriteLine("--1 partially overlaps 2!");
                }
                else if (int.Parse(elf2Min) >= int.Parse(elf1Min) && int.Parse(elf2Min) <= int.Parse(elf1Max))
                {
                    partialOverlaps++;
                    Console.WriteLine("--2 partially overlaps 1!");
                }
                else if (int.Parse(elf2Max) <= int.Parse(elf1Max) && int.Parse(elf2Max) >= int.Parse(elf1Min))
                {
                    partialOverlaps++;
                    Console.WriteLine("--2 partially overlaps 1!");
                }

                Console.WriteLine($"---Total partial overlaps: {partialOverlaps}");
            }
        }

        public static void Day5()
        {
            var instructions = File.ReadAllLines("2022input5.txt");


            var stacksNumberLine = 0;
            var numberOfStacks = 0;
            var stacks = new List<List<char>>();

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
                stacks.Add(new List<char>());
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
                            stacks[j].Add(instructions[i][(j * 4) + 1]);
                        }
                    }
                }
            }

            var counter = 0;
            foreach (var stack in stacks)
            {
                counter++;
                Console.WriteLine($"Stack {counter}");
                foreach (var box in stack)
                {
                    Console.Write($"[{box}] - ");
                }
                Console.Write($" Last: {stack.Last()}");
                Console.WriteLine();
            }

            for (int i = stacksNumberLine; i < instructions.Length; i++)
            {
                var instruction = instructions[i];

                if (instruction.StartsWith("move"))
                {
                    var qty = int.Parse(instruction.Substring(5, instruction.IndexOf('f') - 5));
                    var from = int.Parse(instruction.Substring(instruction.LastIndexOf('m') + 1, instruction.IndexOf('t') - 1 - (instruction.LastIndexOf('m') + 1)));
                    var to = int.Parse(instruction.Substring(instruction.LastIndexOf('o') + 1));

                    Console.WriteLine($"move {qty} from {from} to {to}");

                    /**
                     * Part 1
                    while(qty > 0)
                    {
                        var movingbox = stacks[from - 1].Last();
                        Console.WriteLine($"moving {movingbox} from {from} to {to}");
                        stacks[to - 1].Add(movingbox);
                        stacks[from - 1].RemoveAt(stacks[from -1].Count - 1);
                        qty--;

                        counter = 0;
                        foreach (var stack in stacks)
                        {
                            counter++;
                            Console.WriteLine($"Stack {counter}");
                            foreach (var box in stack)
                            {
                                Console.Write($"[{box}] - ");
                            }
                            Console.WriteLine();
                        }
                    }
                    **/


                    // Part 2

                    while (qty > 0)
                    {
                        var movingIndex = stacks[from - 1].Count - qty;
                        var movingbox = stacks[from - 1][movingIndex];
                        Console.WriteLine($"moving {movingbox} from {from} to {to}");
                        stacks[to - 1].Add(movingbox);
                        stacks[from - 1].RemoveAt(movingIndex);
                        qty--;

                        counter = 0;
                        foreach (var stack in stacks)
                        {
                            counter++;
                            Console.WriteLine($"Stack {counter}");
                            foreach (var box in stack)
                            {
                                Console.Write($"[{box}] - ");
                            }
                            Console.WriteLine();
                        }
                    }

                }
            }

            counter = 0;
            foreach (var stack in stacks)
            {
                counter++;
                Console.WriteLine($"Stack {counter}");
                foreach (var box in stack)
                {
                    Console.Write($"[{box}] - ");
                }
                Console.WriteLine();
            }

            foreach (var stack in stacks)
            {
                Console.Write(stack.Last());
            }

        }

        public static void Day6()
        {
            var datastream = File.ReadAllText("2022input6.txt");
            //datastream = "mjqjpqmgbljsphdztnvjfqwrcgsmlb";

            //char[] dataBuffer = { (char)datastream[0],'/', '/', '/', '/', '/', '/', '/', '/', '/', '/', '/', '/', '/' };


            var dataBuffer = "";

            dataBuffer += datastream[0];

            Console.WriteLine(datastream);
            Console.WriteLine(datastream.Length);

            for(var i = 0; i < datastream.Length; i++)
            {
                var repeat = false;    
                for(var j = 0; j < dataBuffer.Length; j++)
                {
                    //Console.WriteLine($"Comparing {c} with {(char)datastream[i]}");
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

                if (dataBuffer.Length == 14)
                {
                    Console.Write("Found ");
                    foreach(var c in dataBuffer)
                    {
                        Console.Write(c);
                    }
                    Console.Write($" at {i + 1}");
                    break;
                }
                if(dataBuffer.Length > 10)
                Console.WriteLine($"{i}: {dataBuffer.Length} / {datastream.Length}" );
            }

        }

        public static void Day7()
        {
            var terminalOutput = File.ReadAllLines("2022input7.txt");
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
                Console.WriteLine($"{dir} - {size}");
                if (size <= 100000)
                {
                    totalSize += size;
                }
            }
            Console.WriteLine($"Total Size: {totalSize}");

            var targetSize = 30000000;

            var remainingSize = 70000000 - listOfDirectories["//"];

            var deleteTarget = targetSize - remainingSize;

            var deleteTargets = new Dictionary<string, int>();

            foreach (var (dir, size) in updatedSizes)
            {
                if (size >= deleteTarget)
                {
                    deleteTargets.Add(dir, size);
                    Console.WriteLine($"{dir} - {size}");
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
            Console.WriteLine($"Delete Target Size: {min}");

        }


        public static void Day8()
        {
            var map = File.ReadAllLines("2022input8.txt");

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
                var height = int.Parse(map[i][0].ToString().ToString());
                visibleTrees++;
                maxRowHeights.Add(int.Parse(map[i][0].ToString()));

                height = int.Parse(map[i][map[i].Length-1].ToString());
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

                Console.WriteLine($"{i+1} Total visible trees: {visibleTrees}");
            }

            Console.WriteLine($"Total visible trees: {visibleTrees}");

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


            Console.WriteLine($"Max Scenic Score: {maxScenicScore}");

        }

        public static void Day9()
        {
            var movements = File.ReadAllLines("2022input9.txt");

            var knots = 10;
            var rope = new List<(int, int)>(knots);

            for(var i = 0; i < knots; i++)
            {
                rope.Add((0, 0));
            }

            var visited = new Dictionary<(int, int), int>();

            visited.Add(rope[knots-1], 1);

            foreach(var movement in movements)
            {
                var distance = int.Parse(movement.Substring(movement.IndexOf(' ') + 1));
                Console.WriteLine($"H ({rope[0].Item1},{rope[0].Item2}) - T ({rope[1].Item1},{rope[1].Item2})");


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

                        Console.WriteLine($"{xDistance} {yDistance}");

                        while (Math.Abs(xDistance) > 1 || Math.Abs(yDistance) > 1)
                        {
                            var knotPos = rope[k];

                            // Horizontal correction
                            if (yDistance == 0)
                            {
                                // Move towards head one step
                                Console.WriteLine($"Moving Knot {k} x {xDistance / Math.Abs(xDistance)} units");
                                knotPos.Item1 += xDistance / Math.Abs(xDistance);
                            }

                            // Vertical correction
                            if (xDistance == 0)
                            {
                                // Move towards head one step
                                Console.WriteLine($"Moving Knot {k} y {yDistance / Math.Abs(yDistance)} units");
                                knotPos.Item2 += yDistance / Math.Abs(yDistance);
                            }

                            // Diagonal correction 1
                            if ((xDistance > 1 || xDistance < -1) && (yDistance != 0))
                            {
                                // Move towards head one diagonal step
                                Console.WriteLine($"Moving Knot {k} x {xDistance / Math.Abs(xDistance)} units");
                                knotPos.Item1 += xDistance / Math.Abs(xDistance);
                                Console.WriteLine($"Moving Knot {k} y {yDistance / Math.Abs(yDistance)} units");
                                knotPos.Item2 += yDistance / Math.Abs(yDistance);
                            }
                            else if ((yDistance > 1 || yDistance < -1) && (xDistance != 0))
                            {
                                // Move towards head one diagonal step
                                Console.WriteLine($"Moving Knot {k} x {xDistance / Math.Abs(xDistance)} units");
                                knotPos.Item1 += xDistance / Math.Abs(xDistance);
                                Console.WriteLine($"Moving Knot {k} y {yDistance / Math.Abs(yDistance)} units");
                                knotPos.Item2 += yDistance / Math.Abs(yDistance);
                            }

                            if(k == knots - 1)
                            {
                                if (visited.ContainsKey(knotPos))
                                {
                                    visited[knotPos]++;
                                }
                                else
                                {
                                    visited.Add(knotPos, 1);
                                }
                            }

                            rope[k] = knotPos;

                            xDistance = rope[k - 1].Item1 - rope[k].Item1;
                            yDistance = rope[k - 1].Item2 - rope[k].Item2;
                        }
                    }
                }
            }

            var uniqueVisits = visited.Count;

            Console.WriteLine($"The tail visited {uniqueVisits} positions at least once");
        }


        public static void Day10()
        {
            var commands = File.ReadAllLines("2022input10.txt");

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
                Console.WriteLine($"SIGNAL {signal}");
                sum += signal;
            }

            Console.WriteLine($"SUm of signals: {sum}");

            foreach (var row in crt)
            {
                Console.WriteLine(row);
            }

        }
    }
}
