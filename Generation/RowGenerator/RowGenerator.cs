using System;
using Domain;

namespace Generation.RowGenerator
{
    public class RowGenerator : IRowGenerator
    {
        private readonly Random _random = new Random();

        public Row Generate() => new Row(_random.Next(), GenerateRandomString());

        private string GenerateRandomString() => Guid.NewGuid().ToString();
    }
}