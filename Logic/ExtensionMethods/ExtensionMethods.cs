using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Diagnostics;

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

        public static void Dump(this object obj)
        {
            Debug.WriteLine(JsonConvert.SerializeObject(obj));
        }


        public static string GetStringTillClosing(this string str, char start = '(', char end = ')')
        {
            int s = 0;
            int i = -1;
            int newNest = 0;
            //while (++i < str.Length)
            //    if (str[i] == start)
            //    {
            //        s = i;
            //        break;
            //    }
            int e = 0;
            while (++i < str.Length)
            {
                if (str[i] == start)
                {
                    newNest++;
                }
                else if (str[i] == end)
                {

                    if (newNest == 0)
                    {
                        e = i;
                        break;
                    }
                    newNest--;
                }
            }
            if (e > s)
                return str.Substring(s, e - s);
            //return str.Substring(s + 1 , e - s - 1);
            return string.Empty;
        }


        //public static string GetFirstBranch(this string str, char start = '(', char end = ')')
        //{
        //    int s = -1;
        //    int i = -1;
        //    int newNest = 0;
        //    while (++i < str.Length)
        //        if (str[i] == start)
        //        {
        //            s = i;
        //            break;
        //        }
        //    int e = -1;
        //    while (++i < str.Length)
        //    {
        //        if (str[i] == start)
        //        {
        //            newNest++;
        //        }
        //        else if (str[i] == end)
        //        {

        //            if (newNest == 0)
        //            {
        //                e = i;
        //                break;
        //            }
        //            newNest--;
        //        }
        //    }
        //    if (e > s)
        //        return str.Substring(s , e - s + 1);
        //    //return str.Substring(s + 1 , e - s - 1);
        //    return string.Empty;
        //}

        public static List<string> GetOptions(this string str, char optionChar = '|')
        {
            var start = '(';
            var end = ')';
            int i = -1;
            int insideParenthesis = 0;
            var optionIndexes = new List<int>();
            var options = new List<string>();

            while (++i < str.Length)
            {
                if (str[i] == start)
                {
                    insideParenthesis++;
                }
                else if (str[i] == end)
                {
                    insideParenthesis--;
                }
                else if (str[i] == optionChar && insideParenthesis == 0)
                {
                    optionIndexes.Add(i);
                }
            }
            if (optionIndexes.Any())
            {
                var index = 0;
                // optionIndexes.Add(0);
                foreach (var option in optionIndexes.OrderBy(c => c))
                {
                    var isPipe = str[index] == '|' ? 1 : 0;
                    options.Add(str.Substring(index + isPipe, option - (index + isPipe))); //.Replace("|", ""));
                    index = option;
                }
                //add lastoption
                var isPipee = str[index] == '|' ? 1 : 0;
                options.Add(str.Substring(index + isPipee, str.Length - (index + isPipee))); //.Replace("|", ""));
                //options.Add(str.Substring(index, str.Length - index)); //.Replace("|", ""));
                //index = option;
            }
            //return str.Substring(s + 1 , e - s - 1);
            return options;
        }

    }
}
