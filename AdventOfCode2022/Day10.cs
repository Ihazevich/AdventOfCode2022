using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day10
    {
        public static Tuple<string, string> Solve(string input)
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

            while (runCpu)
            {
                cycles++;

                if (commandRunning)
                {
                    if (commandCycle % 2 == 0)
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
                if (pixelPos % crtWidth == 0)
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

            foreach (var signal in signalsStrenght)
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
