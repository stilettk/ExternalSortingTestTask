using System;
using System.Collections.Generic;

namespace Domain
{
    public class RowStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(Reverse(x), Reverse(y));
        }

        private static string Reverse(string s)
        {
            var parts = GetParts(s);
            return parts.s + ". " + parts.n;
        }

        private static (string n, string s) GetParts(string s)
        {
            var index = s.IndexOf('.', StringComparison.Ordinal);
            return (s.Substring(0, index), s.Substring(index + 2, s.Length - index - 2));
        }
    }
}