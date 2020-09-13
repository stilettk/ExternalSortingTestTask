using System;
using System.Collections.Generic;

namespace Sorting.SortingStrategy
{
    public interface ISortingStrategy<T> where T: IComparable
    {
        IEnumerable<T> Sort(IEnumerable<T> collection, IComparer<T> comparer = null);
    }
}