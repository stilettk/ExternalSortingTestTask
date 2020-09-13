using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain;
using NLog;

namespace Sorting.SortingStrategy
{
    public class HPCMergeSortingStrategy : ISortingStrategy<Row>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public IEnumerable<Row> Sort(IEnumerable<Row> collection, IComparer<Row> comparer = null)
        {
            var sw = Stopwatch.StartNew();
            var list = collection.ToList();
            Logger.Debug("Prepare: " + sw.Elapsed);
            sw.Restart();
            
            HPCsharp.ParallelAlgorithm.SortMergeInPlacePar(ref list, comparer);
            Logger.Debug("Sort: " + sw.Elapsed);
            
            return list;
        }
    }
}