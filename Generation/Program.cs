using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Generation.Generator;
using Generation.RowGenerator;

namespace Generation
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var filePath = args.Length > 0 ? args[0] : "generated.txt";

            Console.WriteLine($"Generating {filePath}...");
            var sw = Stopwatch.StartNew();

            var generator = GetGenerator();
            await generator.GenerateAsync("generated.txt", 10);

            Console.WriteLine($"Finished in {sw.Elapsed}.");
        }

        private static IGenerator GetGenerator()
        {
            IRowGenerator rowGenerator = new RowGenerator.RowGenerator();
            var generator = new ParallelGenerator(new SimpleGenerator(rowGenerator));
            return generator;
        }
    }
}