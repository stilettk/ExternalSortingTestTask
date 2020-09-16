using System.Diagnostics;
using System.Threading.Tasks;
using Generation.Generator;
using Generation.RowGenerator;
using NLog;

namespace Generation
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        static async Task Main(string[] args)
        {
            var filePath = args.Length > 0 ? args[0] : "generated.txt";

            Logger.Info($"Generating {filePath}...");
            var sw = Stopwatch.StartNew();

            var generator = GetGenerator();
            await generator.GenerateAsync("generated.txt", 1L * 1024 * 1024 * 1024);

            Logger.Info($"Finished in {sw.Elapsed}.");
        }

        private static IGenerator GetGenerator()
        {
            IRowGenerator rowGenerator = new RowGenerator.RowGenerator();
            var generator = new ParallelGenerator(new SimpleGenerator(rowGenerator));
            return generator;
        }
    }
}