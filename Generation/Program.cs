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
            const string filePath = "generated.txt";
            
            Console.WriteLine($"Generating {filePath}...");
            
            var sw = new Stopwatch();
            sw.Start();
            
            IRowGenerator rowGenerator = new RowGenerator.RowGenerator();
            var generator = new ParallelGenerator(new SimpleGenerator(rowGenerator));
            await generator.Generate("generated.txt", 100000000);
            
            Console.WriteLine($"Finished in {sw.Elapsed}.");
        }
    }
}