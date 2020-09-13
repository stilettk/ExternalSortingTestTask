using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Generation.Generator
{
    public class ParallelGenerator : IGenerator
    {
        private static readonly int MaxDegreeOfParallelism = Environment.ProcessorCount;
        private readonly SemaphoreSlim _fileSemaphore = new SemaphoreSlim(1, 1);

        private readonly IGenerator _innerGenerator;

        public ParallelGenerator(IGenerator innerGenerator)
        {
            _innerGenerator = innerGenerator;
        }

        public async Task GenerateAsync(string filePath, int rowCount)
        {
            var tempFileNames = Enumerable.Range(0, MaxDegreeOfParallelism)
                .Select(_ => Guid.NewGuid().ToString())
                .ToList();

            // TODO the amount of generated rows won't always equal to rowCount
            var generateTasks = tempFileNames.Select(async tempFile =>
            {
                await _innerGenerator.GenerateAsync(tempFile, rowCount / MaxDegreeOfParallelism);
                Console.WriteLine($"Generated temp file {tempFile}");

                await CombineFilesAsync(tempFile, filePath);
                Console.WriteLine($"Combined {tempFile} into ${filePath}");
            });
            await Task.WhenAll(generateTasks);
        }

        private async Task CombineFilesAsync(string tempFile, string destinationFile)
        {
            try
            {
                await _fileSemaphore.WaitAsync();

                await using var fs = new FileStream(destinationFile, FileMode.Append, FileAccess.Write);
                
                await using var tempFileStream = File.OpenRead(tempFile);
                await tempFileStream.CopyToAsync(fs);
            }
            finally
            {
                File.Delete(tempFile);
                _fileSemaphore.Release();
            }
        }
    }
}