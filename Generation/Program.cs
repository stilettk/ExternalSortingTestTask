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

            IRowGenerator rowGenerator = new RowGenerator.RowGenerator();
            var generator = new ParallelGenerator(new SimpleGenerator(rowGenerator));


            Console.WriteLine($"Generating {filePath}...");
            var sw = Stopwatch.StartNew();

            await generator.Generate("generated.txt", 10);

            Console.WriteLine($"Finished in {sw.Elapsed}.");
        }
    }
}