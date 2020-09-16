using System;

namespace Domain
{
    public readonly struct Row : IComparable<Row>, IComparable
    {
        public int Number { get; }
        public string String { get; }

        public Row(int number, string s)
        {
            Number = number;
            String = s ?? "";
        }

        public static Row From(string rowString)
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

            return new Row(number, stringPart);
        }

        public override string ToString() => $"{Number}. {String}";

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