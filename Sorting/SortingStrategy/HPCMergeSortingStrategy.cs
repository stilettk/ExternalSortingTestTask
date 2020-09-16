using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain;
using NLog;

namespace Sorting.SortingStrategy
{
    public class HPCMergeSortingStrategy<T> : ISortingStrategy<T>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public IEnumerable<T> Sort(IEnumerable<T> collection, IComparer<T> comparer = null)
        {
            var sw = Stopwatch.StartNew();
            var list = collection.ToList();
            _logger.Debug("Prepare: " + sw.Elapsed);
            sw.Restart();

            HPCsharp.ParallelAlgorithm.SortMergeInPlacePar(ref list, comparer);
            _logger.Debug("Sort: " + sw.Elapsed);

            return list;
        }
    }
}