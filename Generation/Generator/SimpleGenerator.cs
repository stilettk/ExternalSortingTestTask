using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Generation.RowGenerator;

namespace Generation.Generator
{
    public class SimpleGenerator : IGenerator
    {
        private readonly IRowGenerator _rowGenerator;

        public SimpleGenerator(IRowGenerator rowGenerator)
        {
            _rowGenerator = rowGenerator;
        }

        public async Task GenerateAsync(string filePath, long maxBytes)
        {
            await File.WriteAllLinesAsync(filePath, GenerateLines(maxBytes));
        }

        private IEnumerable<string> GenerateLines(long maxBytes)
        {
            long bytesGenerated = 0;
            while (bytesGenerated < maxBytes)
            {
                var row = _rowGenerator.Generate().ToString();

                var bytes = Encoding.Default.GetByteCount(row);
                bytesGenerated += bytes;

                yield return row;
            }
        }
    }
}