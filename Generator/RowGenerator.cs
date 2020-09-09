using System;
using System.IO;
using Domain;

namespace Generator
{
    public class RowGenerator
    {
        private readonly Random _random = new Random();

        public Row Generate() => new Row(_random.Next(), GenerateRandomString());

        private static string GenerateRandomString() => Path.GetRandomFileName().Replace('.', ' ');
    }
}