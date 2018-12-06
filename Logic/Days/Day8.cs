using Logic.Interface;
using Logic.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day8: AdventBase, AdventInterface
    {
        public Day8()
        {
            // Test = true;
            PuzzleInput = Test ? Resources.Day8Example : Resources.Day8;
        }

        public string[] Solution()
        {
            return new string[] {
                "",
                ""
            };
        }

        public string Part1()
        {
            return "";
        }

        public string Part2()
        {
            return "";
        }


        public string GetListName()
        {
            return "Day 8";
        }

        public int GetID()
        {
            return 8;
        }
    }
}

