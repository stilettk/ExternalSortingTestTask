using System;

namespace Domain
{
    public class Row
    {
        public Row(int number, string s)
        {
            Number = number;
            String = s;
        }

        public int Number { get; }
        public string String { get; }

        public override string ToString() => $"{Number}. {String}";
    }
}