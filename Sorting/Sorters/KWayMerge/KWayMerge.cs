using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Sorters.KWayMerge
{
    public static class KWayMerge
    {
        private class ChunkReader : IDisposable
        {
            private readonly StreamReader _reader;
            private string _headRow;

            public ChunkReader(string chunkPath, int? bufferSize)
            {
                _reader = new StreamReader(chunkPath, Encoding.Default, true, bufferSize ?? -1);
            }

            public async Task<string> PeekAsync() => _headRow ??= await _reader.ReadLineAsync();

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
            IComparer<string> comparer, int? bufferSize)
        {
            await using var destWriter = File.CreateText(destPath);

            var chunkReaders = chunkPaths.ToDictionary(
                x => x, path => new ChunkReader(path, bufferSize));
            while (chunkReaders.Count > 0)
            {
                ChunkReader minChunk = null;
                foreach (var (chunkPath, reader) in chunkReaders)
                {
                    var row = await reader.PeekAsync();
                    if (row == null)
                    {
                        reader.Dispose();
                        chunkReaders.Remove(chunkPath);
                        continue;
                    }

                    if (minChunk == null || comparer.Compare(row, await minChunk.PeekAsync()) < 0)
                    {
                        minChunk = reader;
                    }
                }

                if (minChunk != null)
                {
                    var minRow = await minChunk.PopAsync();
                    var newLine = minRow + (chunkReaders.Count > 0 ? Environment.NewLine : "");
                    await destWriter.WriteAsync(newLine);
                }
            }
        }
    }
}