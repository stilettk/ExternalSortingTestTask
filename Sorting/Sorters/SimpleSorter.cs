using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Sorting.SortingStrategy;

namespace Sorting.Sorters
{
    public class SimpleSorter : ISorter
    {
        private readonly ISortingStrategy<Row> _sortingStrategy;

        public SimpleSorter(ISortingStrategy<Row> sortingStrategy)
        {
            _sortingStrategy = sortingStrategy;
        }

        public async Task SortAsync(string sourcePath, string destPath)
        {
            var lines = await File.ReadAllLinesAsync(sourcePath);
            var sortedLines = _sortingStrategy.Sort(lines.Select(x => new Row(x)));
            await File.WriteAllLinesAsync(destPath, sortedLines.Select(x => x.ToString()));
        }
    }
}