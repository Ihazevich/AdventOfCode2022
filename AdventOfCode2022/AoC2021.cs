using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    public class AoC2021
    {
        public static void Day1()
        {
            var measurements = File.ReadAllLines("2021input1.txt");
            var increasingMeasurements = 0;

            //Part 1
            for(var i = 1; i < measurements.Length; i++)
            {
                if (int.Parse(measurements[i]) > int.Parse(measurements[i-1]))
                {
                    increasingMeasurements++;
                }
            }

            // Part 2
            var groupsOfThree = new List<int>();

            for(var i = 0; i < measurements.Length -2;i++)
            {
                var group = int.Parse(measurements[i]) + int.Parse(measurements[i+1]) + int.Parse(measurements[i+2]);
                groupsOfThree.Add(group);
            }

            var increasingGTMeasurements = 0;
            
            for (var i = 1; i < groupsOfThree.Count; i++)
            {
                if (groupsOfThree[i] > groupsOfThree[i - 1])
                {
                    increasingGTMeasurements++;
                }

            }

            Console.WriteLine(increasingMeasurements);
            Console.WriteLine(increasingGTMeasurements);
        }

        public static void Day2()
        {
            var commands = File.ReadAllLines("2021input2.txt");
            
        }
    }
}
