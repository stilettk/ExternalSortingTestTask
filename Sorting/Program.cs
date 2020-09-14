using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NLog;
using Sorting.Sorters;
using Sorting.Sorters.External;
using Sorting.SortingStrategy;

namespace Sorting
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static async Task Main(string[] args)
        {
            try
            {
                var sourcePath = args.Length > 0 ? args[0] : "generated.txt";
                var destPath = args.Length > 1 ? args[1] : "sorted.txt";

                Logger.Info($"Sorting {sourcePath}...");
                var sw = Stopwatch.StartNew();

                var sorter = GetSorter();
                await sorter.SortAsync(sourcePath, destPath);

                Logger.Info($"Sorted to {destPath} in {sw.Elapsed}.");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private static ISorter GetSorter()
        {
            var sortingStrategy = new HPCMergeSortingStrategy();
            var sorter = new ExternalSorter(sortingStrategy);
            return sorter;
        }
    }
}