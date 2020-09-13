using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Sorting.SortingStrategy;

namespace Sorting.Sorters
{
    public class SimpleSorter : ISorter
    {
        private readonly ISortingStrategy<string> _sortingStrategy;

        public SimpleSorter(ISortingStrategy<string> sortingStrategy)
        {
            _sortingStrategy = sortingStrategy;
        }

        public async Task SortAsync(string sourcePath, string destPath)
        {
            var lines = await File.ReadAllLinesAsync(sourcePath);
            var sortedLines = _sortingStrategy.Sort(lines, Row.StringComparer);
            await File.WriteAllLinesAsync(destPath, sortedLines);
        }
    }
}