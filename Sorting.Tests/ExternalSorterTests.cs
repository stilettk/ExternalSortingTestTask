using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
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

        public static IEnumerable<string[]> ExternalSorterTestCases
        {
            get
            {
                yield return new[] {"2. a", "2. b", "1. a", "1. b"};
                yield return new[] {"1. b", "1. b", "1. a", "1. a"};
                yield return new[] {"1. a", "1. a", "2. b"};
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