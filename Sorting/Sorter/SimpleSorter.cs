using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Sorting.Sorter
{
    public class SimpleSorter
    {
        public async Task Sort(string sourcePath, string destPath)
        {
            var lines = await File.ReadAllLinesAsync(sourcePath);
            var rows = lines.Select(Row.From);

            var sortedRows = rows.OrderBy(x => x);

            var sortedRowStrings = sortedRows.Select(row => row.ToString());
            await File.WriteAllLinesAsync(destPath, sortedRowStrings);
        }
    }
}