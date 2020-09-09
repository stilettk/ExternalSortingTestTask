using System.IO;
using System.Threading.Tasks;

namespace Generator
{
    public class Generator : IGenerator
    {
        public async Task Generate(string filePath, int rowCount)
        {
            await using var writer = File.CreateText(filePath);

            var rowGenerator = new RowGenerator();
            for (var i = 0; i < rowCount; i++)
            {
                var row = rowGenerator.Generate();
                await writer.WriteLineAsync(row.ToString());
            }
        }
    }
}