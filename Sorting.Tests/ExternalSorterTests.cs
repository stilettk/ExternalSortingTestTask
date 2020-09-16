using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Domain;
using Generation.Generator;
using Generation.RowGenerator;
using NUnit.Framework;
using Sorting.Sorters;
using Sorting.Sorters.External;
using Sorting.SortingStrategy;

namespace Sorting.Tests
{
    public class ExternalSorterTests
    {
        private const string DirectoryName = nameof(ExternalSorterTests);

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory(DirectoryName);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(DirectoryName, true);
        }

        public static IEnumerable<string[]> ExternalSorterTestCases
        {
            get
            {
                yield return new[] {"2. a", "2. b", "1. a", "1. b"};
                yield return new[] {"1. b", "1. b", "1. a", "1. a"};
                yield return new[] {"1. a", "1. a", "2. b"};
                yield return new[] {"10. a", "1. a", "2. b"};
                yield return new[] {"101. a", "99. a"};
                yield return new[] {"3. Apple", "2. Apple", "2. Apple"};
                yield return Enumerable.Range(0, 1000)
                    .Select(i => new Row(i, i.ToString()).ToString())
                    .ToArray();
            }
        }

        [TestCaseSource(nameof(ExternalSorterTestCases))]
        [Test]
        public async Task ShouldEqualToDefaultSorter(string[] input)
        {
            var sourcePath = GetFilePath();
            await File.WriteAllLinesAsync(sourcePath, input);
            var externalSorter = GetExternalSorter();
            var simpleSorter = GetSimpleSorter();

            var externalSorterTask = SortWith(externalSorter, sourcePath);
            var simpleSorterTask = SortWith(simpleSorter, sourcePath);
            await Task.WhenAll(externalSorterTask, simpleSorterTask);

            CollectionAssert.AreEqual(simpleSorterTask.Result, externalSorterTask.Result);
        }

        [Test]
        public async Task ShouldGenerateSameFileLengthAsGenerator()
        {
            var sourcePath = GetFilePath();
            var generator = GetGenerator();
            await generator.GenerateAsync(sourcePath, 10 * 1024);

            var destPath = GetFilePath();
            var sorter = GetExternalSorter(4096);
            await sorter.SortAsync(sourcePath, destPath);

            var sourceFileSize = new FileInfo(sourcePath).Length;
            var destFileSize = new FileInfo(destPath).Length;
            Assert.AreEqual(sourceFileSize, destFileSize);
        }

        private static async Task<string[]> SortWith(ISorter sorter, string sourcePath)
        {
            var destPath = GetFilePath();
            await sorter.SortAsync(sourcePath, destPath);
            return await File.ReadAllLinesAsync(destPath);
        }

        private static IGenerator GetGenerator() => new ParallelGenerator(new SimpleGenerator(new RowGenerator()));

        private static ISorter GetSimpleSorter() => new SimpleSorter(new DefaultSortingStrategy<Row>());

        private static ISorter GetExternalSorter(int? chunkSizeBytes = 1) => new ExternalSorter(
            new HPCMergeSortingStrategy<Row>(),
            new ExternalSorterOptions {ChunkSizeBytes = chunkSizeBytes});

        private static string GetFilePath() => Path.Combine(DirectoryName, Guid.NewGuid().ToString());
    }
}