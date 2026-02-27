using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPLibrary.Extensions
{
    public static class TypeHelpers
    {
        public static bool IsDate(this string input)
        {
            DateTime dValue;
            return DateTime.TryParse(input, out dValue);
        }
    }
}
