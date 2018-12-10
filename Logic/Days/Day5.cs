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
    public class Day5 : AdventBase
    {
        public Day5()
        {
            PuzzleInput = Test ? Resources.Day5Example : Resources.Day5;
            ID = 5;
            Name = "Day 5: Alchemical Reduction";
        }

        public override string[] Solution()
        {
            return new string[] {
                "11152",
                "6136"
            };
        }
        public override string Part1()
        {
            var shifts = PuzzleInput.ToList();
            bool changed;
            var prevCount = shifts.Count();
            do
            {
                changed = false;
                for (int i = 0; i < shifts.Count() - 1; i++)
                {
                    if (((int)shifts[i] - (int)shifts[i + 1]) % 32 == 0 && (shifts[i] != shifts[i + 1]))
                    {
                        shifts.RemoveAt(i);
                        shifts.RemoveAt(i);
                        changed = true;
                    }
                }
            } while (changed);
            return shifts.Count().ToString();
        }

        private object varSafe = new object();

        public override string Part2()
        {
            var testChars = PuzzleInput.ToUpper().Distinct().ToHashSet();

            var polymerLength = PuzzleInput.Count();

            Parallel.ForEach(testChars, (testChar) =>
            {
                var shifts = PuzzleInput.ToList();

                //Remove 1 problem causing polymer
                shifts.RemoveAll(c => c == testChar || c == (char)(testChar + 32));
                bool changed;
                do
                {
                    changed = false;
                    for (int i = 0; i < shifts.Count() - 1; i++)
                    {
                        if ((shifts[i] - shifts[i + 1]) % 32 == 0 && (shifts[i] != shifts[i + 1]))
                        {
                            shifts.RemoveAt(i);
                            shifts.RemoveAt(i);
                            changed = true;
                        }
                    }
                } while (changed);
                if (shifts.Count() < polymerLength)
                {
                    lock (varSafe) polymerLength = shifts.Count();
                }
            });
            return polymerLength.ToString();
        }
    }
}

