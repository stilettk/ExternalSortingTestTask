using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using NLog;
using Sorting.Sorters.Algorithms;
using Sorting.SortingStrategy;

namespace Sorting.Sorters.External
{
    public class ExternalSorterOptions
    {
        public int? ChunkSizeBytes { get; set; }
    }

    public class ExternalSorter : ISorter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly SemaphoreSlim _diskSemaphore = new SemaphoreSlim(1, 1);

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
            var sw = Stopwatch.StartNew();
            var chunkPaths = await SortByChunks(sourcePath);
            Logger.Debug($"Chunks created in {sw.Elapsed}.");

            sw.Restart();
            Logger.Debug("Merging chunks...");
            await MergeChunksAsync(destPath, chunkPaths);
            Logger.Debug($"Chunks merged in {sw.Elapsed}.");
        }

        private async Task<ICollection<string>> SortByChunks(string sourcePath)
        {
            using var reader = File.OpenText(sourcePath);

            var chunkPaths = new LinkedList<string>();
            var chunkSaveTasks = new LinkedList<Task>();
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

            await Task.WhenAll(chunkSaveTasks);

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

            try
            {
                // await _diskSemaphore.WaitAsync();
                sw.Restart();
                await File.WriteAllLinesAsync(chunkPath, sortedChunk.Select(x => x.ToString())).ConfigureAwait(false);
                Logger.Debug($"Chunk {chunkPath} saved in {sw.Elapsed}.");
            }
            finally
            {
                // _diskSemaphore.Release();
            }
        }
    }
}