using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Sorters.Algorithms.KWayMerge
{
    public class ChunkReader : IDisposable
    {
        private readonly FileStream _fileStream;
        private readonly StreamReader _reader;

        public ChunkReader(string chunkPath, int? bufferSize)
        {
            Path = chunkPath;
            _fileStream = new FileStream(
                chunkPath,
                FileMode.Open, FileAccess.Read, FileShare.None,
                bufferSize ?? -1,
                FileOptions.SequentialScan);
            _reader = new StreamReader(_fileStream);
        }

        public string Path { get; }

        public async Task<string> ReadAsync() => await _reader.ReadLineAsync();

        public void Dispose()
        {
            _fileStream.Dispose();
            _reader.Dispose();
        }
    }
}