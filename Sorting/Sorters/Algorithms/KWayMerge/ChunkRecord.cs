using System.Collections.Generic;

namespace Sorting.Sorters.Algorithms.KWayMerge
{
    public class ChunkRecord
    {
        public sealed class Comparer : IComparer<ChunkRecord>
        {
            private readonly IComparer<string> _itemComparer;

            public Comparer(IComparer<string> itemComparer)
            {
                _itemComparer = itemComparer;
            }

            public int Compare(ChunkRecord x, ChunkRecord y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                return _itemComparer.Compare(x.Item, y.Item);
            }
        }

        public ChunkRecord(string item, string filePath)
        {
            Item = item;
            FilePath = filePath;
        }

        public string Item { get; }

        public string FilePath { get; }
    }
}