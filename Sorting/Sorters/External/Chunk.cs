using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Domain;

namespace Sorting.Sorters.External
{
    public class Chunk
    {
        private readonly LinkedList<Row> _items = new LinkedList<Row>();
        
        public IReadOnlyCollection<Row> Items => _items;
        
        public int Size { get; private set; }

        public void Add(Row row)
        {
            _items.AddLast(row);
            Size += GetRowSize(row);
        }

        private static int GetRowSize(Row row)
        {
            return Marshal.SizeOf(row);
        }

        private static int GetStringSize(string currentLine)
        {
            return Encoding.Default.GetByteCount(currentLine);
        }
    }
}