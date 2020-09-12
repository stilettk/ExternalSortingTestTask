using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sorting.SortingStrategy
{
    public interface ISortingStrategy
    {
        IEnumerable<string> Sort(IEnumerable<string> collection);
    }
}