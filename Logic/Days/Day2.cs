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
    class Day2 : AdventBase, AdventInterface
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
            var numberOfChar = boxIDs.First().Count();

            var returnstring = string.Empty;
            Parallel.ForEach(boxIDs, (box, loopState) =>
            {
                var matchList = new List<char[]>();
                //Build matchlist positionbased on equal chars
                for (int i = 0; i < numberOfChar; i++) matchList.AddRange(boxIDs.Where(c => c != box && c[i] == box[i]));

                //Count number of occurences, and is matched when equal chars = 25
                var match = matchList.Where(c => c != box).GroupBy(c => c).Select(c => new { count = c.Count(), arr = c.First() }).Where(d => d.count == numberOfChar - 1).FirstOrDefault();
                if (match != null)
                {
                    lock (lockLoop)
                    {
                        var boxIDBuilder = new StringBuilder();
                        for (int i = 0; i < box.Length; i++) if (box[i] == match.arr[i]) boxIDBuilder.Append(box[i]);
                        returnstring = boxIDBuilder.ToString();
                        loopState.Break();
                    }
                }
            });
            return returnstring;
        }
    }
}

