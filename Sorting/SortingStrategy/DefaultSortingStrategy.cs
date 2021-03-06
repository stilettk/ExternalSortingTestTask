﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sorting.SortingStrategy
{
    public class DefaultSortingStrategy<T> : ISortingStrategy<T> where T : IComparable
    {
        public IEnumerable<T> Sort(IEnumerable<T> collection, IComparer<T> comparer = null)
        {
            return collection.AsParallel().OrderBy(x => x, comparer);
        }
    }
}