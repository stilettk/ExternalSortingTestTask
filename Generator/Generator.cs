using System.IO;
using System.Threading.Tasks;

namespace Generator
{
    public class Generator
    {
        public async Task Generate(string filePath, int rowCount)
        {
            await using var fs = File.CreateText(filePath);

            var rowGenerator = new RowGenerator();
            for (var i = 0; i < rowCount; i++)
            {
                var row = rowGenerator.Generate();
                await fs.WriteLineAsync(row.ToString());
            }
        }
    }
}