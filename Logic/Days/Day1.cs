using Logic.Interface;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Days
{
    class Day1 : AdventBase, AdventInterface
    {
        public Day1()
        {
            PuzzleInput = Resources.Day1;
        }

        public string[] GetSolutions()
        {
            return new string[] {
                "576",
                "77674"
            };
        }

        public string Part1()
        {
            return PuzzleInput.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(c => Convert.ToInt64(c)).Sum(c => c).ToString();
        }

        public string Part2()
        {
            var FoundFrequencies = new HashSet<int>();
            var frequencies = PuzzleInput.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(c => Convert.ToInt32(c));
            var total = 0;

            while (true)
            {
                foreach (var frequency in frequencies)
                {
                    total += frequency;
                    if (FoundFrequencies.Contains(total))
                    {
                        return total.ToString();
                    }
                    else
                    {
                        FoundFrequencies.Add(total);
                    }
                }
            }
        }
    }
}

