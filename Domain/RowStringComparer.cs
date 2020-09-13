using System.Collections.Generic;

namespace Domain
{
    public class RowStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return Row.From(x).CompareTo(Row.From(y));
        }
    }
}