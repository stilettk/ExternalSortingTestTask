using System;

namespace Domain
{
    public static class RowStringParser
    {
        public static (string Number, string String) Parse(string s)
        {
            var index = s.IndexOf('.', StringComparison.Ordinal);
            if (index == -1)
            {
                throw new ArgumentException($"Failed to parse row \"{s}\": '.' symbol not found.");
            }

            return (s.Substring(0, index), s.Substring(index + 2, s.Length - index - 2));
        }
    }
}