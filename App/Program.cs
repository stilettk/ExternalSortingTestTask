﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Generation;

namespace App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string filePath = "generated.txt";

            Console.WriteLine($"Generating {filePath}...");

            var sw = new Stopwatch();
            sw.Start();

            var rowGenerator = new RowGenerator();
            var generator = new ParallelGenerator(new Generator(rowGenerator));
            await generator.Generate("generated.txt", 100000000);

            Console.WriteLine($"Finished in {sw.Elapsed}.");
        }
    }
}