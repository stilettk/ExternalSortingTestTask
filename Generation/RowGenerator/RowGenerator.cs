using System;
using System.IO;
using Domain;

namespace Generation.RowGenerator
{
    public class RowGenerator : IRowGenerator
    {
        private readonly Random _random = new Random();

        public Row Generate() => new Row(_random.Next(), GenerateRandomString());

        private static string GenerateRandomString() => Path.GetRandomFileName().Replace('.', ' ');
    }
}