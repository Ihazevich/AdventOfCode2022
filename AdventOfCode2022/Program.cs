using System.Diagnostics;
using System.Linq;

namespace AdventOfCode2022
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var samples = 100;

            if(args?.Length != 0)
            {
                int.TryParse(args?[0], out samples);
            }

            Console.WriteLine("Advent of Code 2022");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine($"Select your day (1 - {AoC2022.DAYS} or -1 to benchmark all): ");

            var day = 0;

            while (!int.TryParse(Console.ReadLine(), out day))
            {
                Console.WriteLine("Invalid input");
            }

            var solver = new AoC2022();

            if (day == -1)
            {
                BenchmarkAll2022(10, solver);
            }
            else
            {
                var result = Execute2022Day(day, solver);

                Console.WriteLine("------------------------------------");
                Console.WriteLine($"Day {day} in {result.Item1:N3}us ");
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"Part 1: {result.Item2.Item1}");
                Console.WriteLine($"Part 2: {result.Item2.Item2}");
            }
        }

        public static (double, Tuple<string, string>) Execute2022Day(int day, AoC2022 solver)
        {

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var results = (Tuple<string, string>)typeof(AoC2022).GetMethod($"Day{day}")?.Invoke(solver, new[] { $"2022input{day}.txt" });
            stopWatch.Stop();
            double timestamp = stopWatch.ElapsedTicks;
            double microseconds = 1_000_000.0 * timestamp / Stopwatch.Frequency;

            return (microseconds, results);
        }

        public static async void BenchmarkAll2022(int samples, AoC2022 solver)
        {
            var scores = new List<double>();
            var solutions = new List<Tuple<string, string>>();

            for(var i = 0; i < AoC2022.DAYS; i++)
            {
                scores.Add(0);
                for(int j = 0; j < samples; j++)
                {
                    var result = Execute2022Day(i+1, solver);
                    scores[i] += result.Item1;
                    if(j == 0)
                    {
                        solutions.Add(result.Item2);
                    }
                }
                scores[i] /= samples;
                
            }

            Console.Clear();

            using StreamWriter benchmarksFile = new("BenchmarkResults.txt");

            for(var i = 0; i < AoC2022.DAYS; i++)
            {
                var lines = "------------------------------------\n";
                lines += $"Day {i + 1} solved in {scores[i]:N3}us (avg.)\n";
                lines += "------------------------------------\n";
                lines += $"Part 1: {solutions[i].Item1}\n";
                lines += $"Part 2: {solutions[i].Item2}\n";

                Console.WriteLine(lines);

                await benchmarksFile.WriteLineAsync(lines);
            }
        }
    }
}