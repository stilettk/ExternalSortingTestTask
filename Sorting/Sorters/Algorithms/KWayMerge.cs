using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Sorters.Algorithms
{
    public static class KWayMerge
    {
        private class ChunkReader : IDisposable
        {
            private readonly StreamReader _reader;
            private string _headRow;

            public ChunkReader(string chunkPath, int? bufferSize)
            {
                Path = chunkPath;
                _reader = new StreamReader(chunkPath, Encoding.Default, true, bufferSize ?? -1);
            }

            public string Path { get; }

            public async Task<string> PeekAsync() => _headRow ??= await _reader.ReadLineAsync().ConfigureAwait(false);

            public async Task<string> PopAsync()
            {
                var result = await PeekAsync();
                _headRow = null;
                return result;
            }

            public void Dispose()
            {
                _reader.Dispose();
            }
        }

        public static async Task ExecuteAsync(
            IEnumerable<string> chunkPaths,
            string destPath,
            IComparer<string> comparer,
            int? bufferSize)
        {
            await using var destWriter = File.CreateText(destPath);

            var chunkReaders = chunkPaths
                .Select(path => new ChunkReader(path, bufferSize))
                .ToList();

            while (true)
            {
                var minChunk = await GetChunkWithMinValue(chunkReaders, comparer);
                if (minChunk == null) break;

                var minRow = await minChunk.PopAsync();
                var newLine = minRow + (chunkReaders.Count > 0 ? Environment.NewLine : "");
                await destWriter.WriteAsync(newLine);
            }

            foreach (var reader in chunkReaders)
            {
                reader.Dispose();
                File.Delete(reader.Path);
            }
        }

        private static async Task<ChunkReader> GetChunkWithMinValue(
            IEnumerable<ChunkReader> chunkReaders,
            IComparer<string> comparer)
        {
            ChunkReader minChunk = null;
            foreach (var reader in chunkReaders)
            {
                var row = await reader.PeekAsync();
                if (row != null && (minChunk == null || comparer.Compare(row, await minChunk.PeekAsync()) < 0))
                {
                    minChunk = reader;
                }
            }

            return minChunk;
        }
    }
}