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
            var rows = lines.Select(Row.From);

            var sortedRows = _sortingStrategy.Sort(rows);

            var sortedRowStrings = sortedRows.Select(row => row.ToString());
            await File.WriteAllLinesAsync(destPath, sortedRowStrings);
        }
    }
}