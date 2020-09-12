using System.IO;
using System.Threading.Tasks;

namespace Generation
{
    public class Generator : IGenerator
    {
        private readonly IRowGenerator _rowGenerator;

        public Generator(IRowGenerator rowGenerator)
        {
            _rowGenerator = rowGenerator;
        }

        public async Task Generate(string filePath, int rowCount)
        {
            await using var writer = File.CreateText(filePath);

            for (var i = 0; i < rowCount; i++)
            {
                var row = _rowGenerator.Generate();
                await writer.WriteLineAsync(row.ToString());
            }
        }
    }
}