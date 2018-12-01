using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Logic.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static string ConvertDoubleToBinaryString(this double myInt, int padLeft = 32)
        {
            return Convert.ToString((int)myInt, 2).PadLeft(padLeft, '0');
        }

        public static string ToDescriptionString<T>(this T val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
