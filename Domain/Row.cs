using System;

namespace Domain
{
    public readonly struct Row : IComparable<Row>, IComparable
    {
        private readonly string _string;
        
        public int Number { get; }
        public string String { get; }

        public Row(int number, string s)
        {
            Number = number;
            String = s ?? "";
            _string = $"{number}. {s}";
        }

        public Row(string rowString)
        {
            if (rowString == null)
            {
                throw new ArgumentNullException(nameof(rowString));
            }

            var (numberPart, stringPart) = RowStringParser.Parse(rowString);

            if (!int.TryParse(numberPart, out var number))
            {
                throw new ArgumentException($"Failed to parse row \"{rowString}\": invalid number part.");
            }

            Number = number;
            String = stringPart;
            _string = rowString;
        }

        public override string ToString() => _string;

        #region IComparable

        public int CompareTo(Row other)
        {
            var stringComparison = string.Compare(String, other.String, StringComparison.Ordinal);
            return stringComparison != 0 ? stringComparison : Number.CompareTo(other.Number);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is Row other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(Row)}");
        }

        #endregion
    }
}