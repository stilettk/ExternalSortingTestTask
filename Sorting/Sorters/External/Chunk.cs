using System.Collections.Generic;
using System.Runtime.InteropServices;
using Domain;

namespace Sorting.Sorters.External
{
    public class Chunk
    {
        private readonly List<Row> _items = new List<Row>();
        
        public IReadOnlyCollection<Row> Items => _items;
        
        public int Size { get; private set; }

        public void Add(Row row)
        {
            _items.Add(row);
            Size += GetRowSize(row);
        }

        private static int GetRowSize(Row row)
        {
            return Marshal.SizeOf(row);
        }
    }
}