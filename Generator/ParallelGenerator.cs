using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Generator
{
    public class ParallelGenerator : IGenerator
    {
        private static readonly int MaxDegreeOfParallelism = Environment.ProcessorCount;

        private readonly IGenerator _innerGenerator;

        public ParallelGenerator(IGenerator innerGenerator)
        {
            _innerGenerator = innerGenerator;
        }

        public async Task Generate(string filePath, int rowCount)
        {
            var tempFileNames = Enumerable.Range(0, MaxDegreeOfParallelism)
                .Select(_ => Guid.NewGuid().ToString())
                .ToList();

            var generateTasks = tempFileNames.Select(tempFile =>
                _innerGenerator.Generate(tempFile, rowCount / MaxDegreeOfParallelism));
            await Task.WhenAll(generateTasks);

            await CombineFiles(tempFileNames, filePath);
        }

        private static async Task CombineFiles(IEnumerable<string> sourceFiles, string destinationFile)
        {
            await using var fs = new FileStream(destinationFile, FileMode.Create, FileAccess.Write);

            foreach (var tempFileName in sourceFiles)
            {
                try
                {
                    await using var tempFileStream = File.OpenRead(tempFileName);
                    await tempFileStream.CopyToAsync(fs);
                }
                finally
                {
                    File.Delete(tempFileName);
                }
            }
        }
    }
}