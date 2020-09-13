using System;
using System.IO;
using System.Threading.Tasks;
using Domain;
using NUnit.Framework;
using Sorting.Sorters;
using Sorting.SortingStrategy;

namespace Sorting.Tests
{
    public class SimpleSorterTests
    {
        private const string DirectoryName = nameof(SimpleSorterTests);

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

        [TestCase(new[] {"2. a", "2. b", "1. a", "1. b"}, new[] {"1. a", "2. a", "1. b", "2. b"})]
        [TestCase(new[] {"1. b", "1. b", "1. a", "1. a"}, new[] {"1. a", "1. a", "1. b", "1. b"})]
        [TestCase(new[] {"1. a", "1. a", "2. b"}, new[] {"1. a", "1. a", "2. b"})]
        [TestCase(new[] {"10. a", "1. a"}, new[] {"1. a", "10. a"})]
        [Test]
        public async Task WhenSort_ShouldSortCorrectly(string[] input, string[] expected)
        {
            var sourcePath = GetFilePath();
            await File.WriteAllLinesAsync(sourcePath, input);

            var destPath = GetFilePath();
            var sorter = GetSorter();
            await sorter.SortAsync(sourcePath, destPath);

            var result = await File.ReadAllLinesAsync(destPath);
            CollectionAssert.AreEqual(result, expected);
        }

        private static ISorter GetSorter() => new SimpleSorter(new DefaultSortingStrategy<string>());

        private static string GetFilePath() => Path.Combine(DirectoryName, Guid.NewGuid().ToString());
    }
}