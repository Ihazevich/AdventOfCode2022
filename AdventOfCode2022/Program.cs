using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AdventOfCode2022
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int DAYS = 20;
            int samples = 100;

            var day = 0;

            Console.WriteLine("Advent of Code 2022");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++");

            if(day == 0)
            {
                Console.WriteLine($"Select your day (1 - {DAYS} or -1 to benchmark all): ");
                while (!int.TryParse(Console.ReadLine(), out day))
                {
                    Console.WriteLine("Invalid input");
                }
            }

            Console.WriteLine($"Starting at: {DateTime.Now}");

            if (day == -1)
            {
                BenchmarkAll2022(samples, DAYS);
            }
            else
            {
                var result = Execute2022Day(day);

                Console.WriteLine("------------------------------------");
                Console.WriteLine($"Day {day} in {result.Item1:N3}us ");
                Console.WriteLine("------------------------------------");
                
                Console.WriteLine($"Part 1: {result.Item2.Item1}");
                Console.WriteLine($"Part 2: {result.Item2.Item2}");
            }
        }

        public static (double, Tuple<string, string>) Execute2022Day(int day)
        {
            var type = Type.GetType($"AdventOfCode2022.Day{day}");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var results = (Tuple<string, string>)Type.GetType($"AdventOfCode2022.Day{day:D2}")?.GetMethod("Solve")?.Invoke(null,new[] { $"2022input{day}.txt" });
            stopWatch.Stop();
            double timestamp = stopWatch.ElapsedTicks;
            double microseconds = 1_000_000.0 * timestamp / Stopwatch.Frequency;

            return (microseconds, results);
        }

        public static async void BenchmarkAll2022(int samples, int days)
        {
            var scores = new List<double>();
            var solutions = new List<Tuple<string, string>>();

            for(var i = 0; i < days; i++)
            {
                scores.Add(0);
                for(int j = 0; j < samples; j++)
                {
                    var result = Execute2022Day(i+1);
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

            for(var i = 0; i < days; i++)
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