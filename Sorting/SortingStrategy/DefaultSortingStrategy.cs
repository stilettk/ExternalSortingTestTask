using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Sorting.SortingStrategy
{
    public class DefaultSortingStrategy : ISortingStrategy
    {
        public IEnumerable<string> Sort(IEnumerable<string> collection)
        {
            return collection
                .Select(Row.From)
                .OrderBy(x => x)
                .Select(x => x.ToString());
        }
    }
}