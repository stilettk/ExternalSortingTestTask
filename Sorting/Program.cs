using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sorting.Sorter;

namespace Sorting
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sourcePath = args.Length > 0 ? args[0] : "generated.txt";
            var destPath = args.Length > 1 ? args[1] : "sorted.txt";

            var sw = new Stopwatch();
            sw.Start();
            
            var sorter = new SimpleSorter();
            await sorter.Sort(sourcePath, destPath);
            
            Console.WriteLine($"Finished in {sw.Elapsed}.");
        }
    }
}