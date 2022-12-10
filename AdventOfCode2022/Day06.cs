using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day06  
    {
        public static Tuple<string, string> Solve(string input)
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
                for (var j = 0; j < dataBuffer.Length; j++)
                {
                    if (dataBuffer[j] == datastream[i])
                    {
                        dataBuffer = dataBuffer.Substring(j + 1);
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

    }
}
