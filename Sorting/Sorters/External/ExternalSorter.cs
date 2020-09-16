using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using NLog;
using Sorting.Sorters.Algorithms.KWayMerge;
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
            var chunkPaths = new List<string>();
            var chunkSaveTasks = new List<Task>();
            Chunk currentChunk = null;
            var sw = Stopwatch.StartNew();

            using var fs = File.OpenText(sourcePath);
            while (true)
            {
                var currentLine = await fs.ReadLineAsync();
                if (currentLine == null) break;

                if (currentChunk == null)
                {
                    currentChunk = new Chunk();
                    Logger.Debug("Created new chunk");
                    sw.Restart();
                }
                
                currentChunk.Add(Row.From(currentLine));

                if (currentChunk.Size >= _chunkSizeBytes)
                {
                    var chunkPath = "chunk" + Guid.NewGuid();
                    chunkPaths.Add(chunkPath);

                    // await SaveChunk(currentChunk, chunkPath);
                    Logger.Debug($"Chunk processed in {sw.Elapsed}");
                    chunkSaveTasks.Add(SortAndSaveChunk(currentChunk, chunkPath));

                    currentChunk = null;
                }
            }

            await Task.WhenAll(chunkSaveTasks);

            return chunkPaths;
        }

        private async Task MergeChunksAsync(string destPath, ICollection<string> chunkPaths)
        {
            var bufferSize = Math.Max(4096, _chunkSizeBytes / chunkPaths.Count);
            await KWayMerge.ExecuteAsync(chunkPaths, destPath, bufferSize);
        }

        private async Task SortAndSaveChunk(Chunk chunk, string chunkPath)
        {
            var sw = Stopwatch.StartNew();
            Logger.Debug($"Saving chunk {chunkPath} ({chunk.Items.Count} items)...");

            var sortedChunk = await Task.Run(() => _sortingStrategy.Sort(chunk.Items).ToList());
            Logger.Debug($"Chunk {chunkPath} sorted in {sw.Elapsed}.");

            sw.Restart();
            await File.WriteAllLinesAsync(chunkPath, sortedChunk.Select(x => x.ToString())).ConfigureAwait(false);
            Logger.Debug($"Chunk {chunkPath} saved in {sw.Elapsed}.");
        }
    }
}