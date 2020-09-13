using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Sorting.Sorters;
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

        [TestCase("2. a", "2. b", "1. a", "1. b")]
        [TestCase("1. b", "1. b", "1. a", "1. a")]
        [TestCase("1. a", "1. a", "2. b")]
        [Test]
        public async Task ShouldEqualToDefaultSorter(params string[] input)
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

        private static async Task<string[]> SortWith(ISorter sorter, string sourcePath)
        {
            var destPath = GetFilePath();
            await sorter.SortAsync(sourcePath, destPath);
            return await File.ReadAllLinesAsync(destPath);
        }

        private static ISorter GetSimpleSorter() => new SimpleSorter(new DefaultSortingStrategy());

        private static ISorter GetExternalSorter() => new ExternalSorter(new DefaultSortingStrategy(),
            new ExternalSorterOptions {ChunkSizeBytes = 1});

        private static string GetFilePath() => Path.Combine(DirectoryName, Guid.NewGuid().ToString());
    }
}