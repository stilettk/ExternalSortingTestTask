using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Sorters.Algorithms.KWayMerge
{
    public class ChunkReader : IDisposable
    {
        private readonly StreamReader _reader;

        public ChunkReader(string chunkPath, int? bufferSize)
        {
            Path = chunkPath;
            _reader = new StreamReader(chunkPath, Encoding.Default, true, bufferSize ?? -1);
        }

        public string Path { get; }

        public async Task<string> ReadAsync() => await _reader.ReadLineAsync().ConfigureAwait(false);

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}