using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventCode
{
    public static class ExtensionMethods
    {
        public static string ConvertDoubleToBinaryString(this double myInt, int padLeft = 32)
        {
            return Convert.ToString((int)myInt, 2).PadLeft(padLeft, '0');
        }

    }
}
