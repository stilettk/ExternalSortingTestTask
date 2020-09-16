using System.Collections.Generic;

namespace Sorting.SortingStrategy
{
    public interface ISortingStrategy<T>
    {
        IEnumerable<T> Sort(IEnumerable<T> collection, IComparer<T> comparer = null);
    }
}