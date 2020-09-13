using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Sorting.SortingStrategy;

namespace Sorting.Sorters
{
    public class ExternalSorterOptions
    {
        public int? ChunkSizeBytes { get; set; }
    }

    public class Chunk
    {
        private readonly LinkedList<string> _items = new LinkedList<string>();
        
        public IEnumerable<string> Items => _items.AsEnumerable();
        
        public int Size { get; private set; }

        public void Add(string line)
        {
            _items.AddLast(line);
            Size += GetStringSize(line);
        }

        private static int GetStringSize(string currentLine)
        {
            return Encoding.Default.GetByteCount(currentLine);
        }
    }

    public class ExternalSorter : ISorter
    {
        private const int DefaultChunkSizeBytes = 128 * 1024 * 1024;

        private readonly ISortingStrategy _sortingStrategy;
        private readonly int _chunkSizeBytes;

        public ExternalSorter(
            ISortingStrategy sortingStrategy,
            ExternalSorterOptions options = null)
        {
            _sortingStrategy = sortingStrategy;
            _chunkSizeBytes = options?.ChunkSizeBytes ?? DefaultChunkSizeBytes;
        }

        public async Task SortAsync(string sourcePath, string destPath)
        {
            var chunkPaths = await SortByChunks(sourcePath);

            await MergeChunksAsync(destPath, chunkPaths);
        }

        private async Task<ICollection<string>> SortByChunks(string sourcePath)
        {
            using var reader = File.OpenText(sourcePath);

            var chunkPaths = new LinkedList<string>();
            var currentChunk = new Chunk();

            string currentLine;
            while ((currentLine = await reader.ReadLineAsync()) != null)
            {
                currentChunk.Add(currentLine);

                if (currentChunk.Size >= _chunkSizeBytes)
                {
                    var chunkPath = "chunk" + Guid.NewGuid();
                    chunkPaths.AddLast(chunkPath);

                    await SaveChunk(currentChunk, chunkPath);

                    currentChunk = new Chunk();
                }
            }

            return chunkPaths;
        }

        private async Task MergeChunksAsync(string destPath, ICollection<string> chunkPaths)
        {
            var bufferSize = Math.Max(4096, _chunkSizeBytes / chunkPaths.Count);
            await KWayMerge.KWayMerge.ExecuteAsync(chunkPaths, destPath, Row.StringComparer, bufferSize);
        }

        private async Task SaveChunk(Chunk chunk, string chunkPath)
        {
            var sortedChunk = _sortingStrategy.Sort(chunk.Items);
            await File.WriteAllLinesAsync(chunkPath, sortedChunk);
        }
    }
}