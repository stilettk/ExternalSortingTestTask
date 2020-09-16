using System.Collections.Generic;
using Domain;

namespace Sorting.Sorters.Algorithms.KWayMerge
{
    public class ChunkRecord
    {
        private sealed class ItemComparer : IComparer<ChunkRecord>
        {
            public int Compare(ChunkRecord x, ChunkRecord y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                return x.Item.CompareTo(y.Item);
            }
        }

        public static IComparer<ChunkRecord> Comparer { get; } = new ItemComparer();

        public ChunkRecord(Row item, string filePath)
        {
            Item = item;
            FilePath = filePath;
        }

        public Row Item { get; }

        public string FilePath { get; }
    }
}