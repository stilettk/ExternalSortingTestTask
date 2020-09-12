using System.IO;
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

        public async Task GenerateAsync(string filePath, int rowCount)
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