using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Sorting.Sorters.Algorithms.KWayMerge
{
    public static class KWayMerge
    {
        public static void Execute(
            IEnumerable<string> chunkPaths,
            string destPath,
            int? bufferSize = null)
        {
            using var destWriter = File.CreateText(destPath);

            var chunkReaders = chunkPaths
                .Select(path => new ChunkReader(path, bufferSize))
                .ToDictionary(x => x.Path);

            var heap = CreateHeap(chunkReaders.Values);
            while (heap.Count > 0)
            {
                var minRecord = heap.GetMin();
                var newLine = minRecord.Item + (chunkReaders.Count > 0 ? Environment.NewLine : "");
                destWriter.Write(newLine);

                FillNewRow(chunkReaders, minRecord.FilePath, heap);
            }

            foreach (var reader in chunkReaders.Values)
            {
                reader.Dispose();
                File.Delete(reader.Path);
            }
        }

        private static BinaryHeap<ChunkRecord> CreateHeap(IReadOnlyCollection<ChunkReader> chunkReaders)
        {
            var initialItems = new List<ChunkRecord>(chunkReaders.Count);
            foreach (var reader in chunkReaders)
            {
                var row = reader.Read();
                if (row != null)
                {
                    initialItems.Add(new ChunkRecord(row.Value, reader.Path));
                }
            }

            var heap = new BinaryHeap<ChunkRecord>(initialItems, ChunkRecord.Comparer);
            return heap;
        }

        private static void FillNewRow(
            IDictionary<string, ChunkReader> chunkReaders,
            string chunkPath,
            BinaryHeap<ChunkRecord> heap)
        {
            var reader = chunkReaders[chunkPath];
            var newRow = reader.Read();
            if (newRow == null)
            {
                reader.Dispose();
                File.Delete(reader.Path);
                chunkReaders.Remove(chunkPath);
                return;
            }

            heap.Add(new ChunkRecord(newRow.Value, reader.Path));
        }
    }
}