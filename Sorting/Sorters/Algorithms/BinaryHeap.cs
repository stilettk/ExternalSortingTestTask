using System.Collections.Generic;
using System.Linq;

namespace Sorting.Sorters.Algorithms
{
    public class BinaryHeap<T>
    {
        private readonly IComparer<T> _comparer;
        private readonly List<T> _items;

        public int Count => _items.Count;

        public BinaryHeap(IEnumerable<T> items, IComparer<T> comparer)
        {
            _comparer = comparer;
            _items = items.ToList();

            for (var i = Count / 2; i >= 0; i--)
            {
                Heapify(i);
            }
        }

        public T GetMin()
        {
            var result = _items[0];
            _items[0] = _items[Count - 1];
            _items.RemoveAt(_items.Count - 1);
            Heapify(0);
            return result;
        }

        public void Add(T item)
        {
            _items.Add(item);
            Heapify(0);
        }

        private void Heapify(int i)
        {
            while (true)
            {
                var leftChild = 2 * i + 1;
                var rightChild = 2 * i + 2;
                var minChild = i;

                if (leftChild < Count && _comparer.Compare(_items[leftChild], _items[minChild]) < 0)
                {
                    minChild = leftChild;
                }

                if (rightChild < Count && _comparer.Compare(_items[rightChild], _items[minChild]) < 0)
                {
                    minChild = rightChild;
                }

                if (minChild == i)
                {
                    break;
                }

                var temp = _items[i];
                _items[i] = _items[minChild];
                _items[minChild] = temp;
                i = minChild;
            }
        }
    }
}