using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Domain;
using Sorting.Sorters;
using Sorting.SortingStrategy;

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

            var sorter = GetSorter();
            await sorter.SortAsync(sourcePath, destPath);

            Console.WriteLine($"Finished in {sw.Elapsed}.");
        }

        private static ISorter GetSorter()
        {
            var sortingStrategy = new DefaultSortingStrategy<Row>();
            var sorter = new SimpleSorter(sortingStrategy);
            return sorter;
        }
    }
}