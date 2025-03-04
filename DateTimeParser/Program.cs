using BenchmarkDotNet.Running;

namespace Benchmarks.DateTimeParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var v = DateTimeSpanParser.Parse("2025-2-25 9:0:09+01:00");

            BenchmarkRunner.Run<DateTimeParseBenchmarks>();
            Console.ReadLine();
        }
    }
}


