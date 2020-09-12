using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sorting.SortingStrategy
{
    public interface ISortingStrategy<T> where T : IComparable
    {
        IEnumerable<T> Sort(IEnumerable<T> collection);
    }
}