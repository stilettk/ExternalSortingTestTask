using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sorting.SortingStrategy;

namespace Sorting.Sorters
{
    public class ExternalSorterOptions
    {
        public int ChunkSizeBytes { get; set; }
    }

    public class ExternalSorter : ISorter
    {
        private readonly ISortingStrategy _sortingStrategy;
        private readonly ExternalSorterOptions _options;

        private readonly ExternalSorterOptions _defaultOptions = new ExternalSorterOptions
        {
            ChunkSizeBytes = 128 * 1024 * 1024
        };

        public ExternalSorter(ISortingStrategy sortingStrategy, ExternalSorterOptions options = null)
        {
            _sortingStrategy = sortingStrategy;
            _options = options ?? _defaultOptions;
        }

        public async Task SortAsync(string sourcePath, string destPath)
        {
            using var reader = File.OpenText(sourcePath);

            var chunkPaths = new LinkedList<string>();
            LinkedList<string> currentChunk = null;
            var currentChunkSize = 0;

            string currentLine;
            while ((currentLine = await reader.ReadLineAsync()) != null)
            {
                if (currentChunk == null)
                {
                    currentChunk = new LinkedList<string>();
                    currentChunkSize = 0;
                }

                currentChunk.AddLast(currentLine);
                currentChunkSize += GetStringSize(currentLine);

                if (currentChunkSize >= _options.ChunkSizeBytes)
                {
                    var chunkPath = "chunk" + Guid.NewGuid();
                    chunkPaths.AddLast(chunkPath);
                    await SaveChunk(currentChunk, chunkPath);
                    currentChunk = null;
                }
            }

            await MergeChunksAsync(chunkPaths, destPath);
        }

        private async Task SaveChunk(IEnumerable<string> currentChunk, string chunkPath)
        {
            var sortedChunk = _sortingStrategy.Sort(currentChunk);
            await File.WriteAllLinesAsync(chunkPath, sortedChunk);
        }

        private async Task MergeChunksAsync(IReadOnlyCollection<string> chunkPaths, string destPath)
        {
            var allLines = chunkPaths.SelectMany(File.ReadAllLines);
            await File.WriteAllLinesAsync(destPath, _sortingStrategy.Sort(allLines));
        }

        private static int GetStringSize(string currentLine)
        {
            return Encoding.Default.GetByteCount(currentLine);
        }
    }
}