using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day02  
    {
        public static Tuple<string, string> Solve(string input)
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

    }
}
