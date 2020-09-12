using System.Threading.Tasks;

namespace Sorting.Sorters
{
    public interface ISorter
    {
        Task SortAsync(string sourcePath, string destPath);
    }
}