using Logic.Interface;
using Logic.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day2 : AdventBase, AdventInterface
    {
        public Day2()
        {
            PuzzleInput = Resources.Day2;
        }

        public string[] Solution()
        {
            return new string[] {
                "8610",
                "iosnxmfkpabcjpdywvrtahluy"
            };
        }

        public string Part1()
        {
            var boxIDs = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.ToCharArray()).ToHashSet();
            var countMultiples = new int[2];
            foreach (var box in boxIDs)
            {
                (bool, bool) multiple = (false, false);
                foreach (var letter in box.Distinct())
                {
                    var countChar = box.Count(c => c == letter);
                    if (countChar == 3) multiple.Item2 = true;
                    else if (countChar == 2) multiple.Item1 = true;

                    if (multiple.Item1 && multiple.Item2) break;
                }
                if (multiple.Item1) countMultiples[0] += 1;
                if (multiple.Item2) countMultiples[1] += 1;
            }
            return (countMultiples[0] * countMultiples[1]).ToString();
        }

        private object lockLoop = new object();

        public string Part2()
        {
            var boxIDs = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.ToCharArray()).ToHashSet();
            var numberOfMatchingChars = boxIDs.First().Count() - 1;

            var returnstring = string.Empty;
            Parallel.ForEach(boxIDs, (box, loopState) => Parallel.ForEach(boxIDs, (matchBox, loopState2) =>
                {
                    var boxIDBuilder = new StringBuilder();
                    if (matchBox != box)
                    {
                        for (int i = 0; i < box.Length; i++) if (box[i] == matchBox[i]) boxIDBuilder.Append(box[i]);
                        if (boxIDBuilder.Length == numberOfMatchingChars)
                        {
                            lock (lockLoop)
                            {
                                returnstring = boxIDBuilder.ToString();
                                loopState.Stop();
                                loopState2.Stop();
                            }
                        }
                    }
                    boxIDBuilder.Clear();
                }));
            return returnstring;
        }
        public string GetListName()
        {
            return "Day 2: Inventory Management System";
        }
        public int GetID()
        {
            return 2;
        }
    }

}

