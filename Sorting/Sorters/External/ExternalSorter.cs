using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Domain;
using NLog;
using Sorting.Sorters.Algorithms;
using Sorting.SortingStrategy;

namespace Sorting.Sorters
{
    public class ExternalSorterOptions
    {
        public int? ChunkSizeBytes { get; set; }
    }

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

    public class ExternalSorter : ISorter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private const int DefaultChunkSizeBytes = 128 * 1024 * 1024;

        private readonly ISortingStrategy<Row> _sortingStrategy;
        private readonly int _chunkSizeBytes;

        public ExternalSorter(ISortingStrategy<Row> sortingStrategy, ExternalSorterOptions options = null)
        {
            _sortingStrategy = sortingStrategy;
            _chunkSizeBytes = options?.ChunkSizeBytes ?? DefaultChunkSizeBytes;
        }

        public async Task SortAsync(string sourcePath, string destPath)
        {
            var chunkPaths = await SortByChunks(sourcePath);
            
            Logger.Debug("Merging chunks...");

            await MergeChunksAsync(destPath, chunkPaths);
        }

        private async Task<ICollection<string>> SortByChunks(string sourcePath)
        {
            using var reader = File.OpenText(sourcePath);

            var chunkPaths = new LinkedList<string>();
            // var chunkSaveTasks = new LinkedList<Task>();
            var currentChunk = new Chunk();
            var sw = Stopwatch.StartNew();

            foreach (var currentLine in File.ReadLines(sourcePath))
            {
                currentChunk.Add(Row.From(currentLine));

                if (currentChunk.Size >= _chunkSizeBytes)
                {
                    var chunkPath = "chunk" + Guid.NewGuid();
                    chunkPaths.AddLast(chunkPath);

                    await SaveChunk(currentChunk, chunkPath);
                    Logger.Debug($"Chunk processed in {sw.Elapsed}");
                    // chunkSaveTasks.AddLast(SaveChunk(currentChunk, chunkPath));

                    Logger.Debug("Created new chunk");
                    currentChunk = new Chunk();
                    sw.Restart();
                }
            }

            // await Task.WhenAll(chunkSaveTasks);

            return chunkPaths;
        }

        private async Task MergeChunksAsync(string destPath, ICollection<string> chunkPaths)
        {
            var bufferSize = Math.Max(4096, _chunkSizeBytes / chunkPaths.Count);
            await KWayMerge.ExecuteAsync(chunkPaths, destPath, Row.StringComparer, bufferSize);
        }

        private async Task SaveChunk(Chunk chunk, string chunkPath)
        {
            var sw = Stopwatch.StartNew();
            Logger.Debug($"Saving chunk {chunkPath} ({chunk.Items.Count} items)...");
            
            var sortedChunk = _sortingStrategy.Sort(chunk.Items).ToList();
            Logger.Debug($"Chunk {chunkPath} sorted in {sw.Elapsed}.");
            
            sw.Restart();
            await File.WriteAllLinesAsync(chunkPath, sortedChunk.Select(x => x.ToString()));
            Logger.Debug($"Chunk {chunkPath} saved in {sw.Elapsed}.");
        }
    }
}