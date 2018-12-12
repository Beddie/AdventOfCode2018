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
    public class Day12 : AdventBase
    {
        public Day12()
        {
            //  Test = true;
            PuzzleInput = Test ? Resources.Day12Example : Resources.Day12;
            ID = 12;
            Name = "Day 12: Subterranean Sustainability";
        }

        public override string[] Solution()
        {
            return new string[] {
                "2995"
                ,"3650000000377"
            };
        }

        public override string Part1()
        {
            var potState = new StringBuilder();
            var potStateZeroIndex = 5;
            if (Test) { potState.Append(".....#..#.#..##......###...###....."); } else { potState.Append(".....#.#..#..###.###.#..###.#####...########.#...#####...##.#....#.####.#.#..#..#.#..###...#..#.#....##....."); };
            var splitRegex = new Regex("[=>]");
            var combinations = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Split("=>")).ToDictionary(a => a[0].Trim(), b => b[1].Trim().First()); //new Dictionary<char[], char>() { 
            var lookMargin = 2;

            for (int generation = 0; generation < 20; generation++)
            {
                Print(potState, potStateZeroIndex, generation);
                var referenceState = potState.ToString();
                potState.Clear();
                potState.Append("...");

                for (int potIndex = lookMargin; potIndex < referenceState.Length - lookMargin; potIndex++)
                {
                    var potPattern = referenceState.Substring(potIndex - lookMargin, 5);

                    if (combinations.TryGetValue(potPattern, out char check))
                    {
                        potState.Append(check);
                    }
                    else
                    {
                        potState.Append('.');
                    }
                }
                potState.Append("...");
                potStateZeroIndex++;
            }

            var count = 0;
            for (int potIndex = 0; potIndex < potState.Length; potIndex++)
            {
                count += potState[potIndex] == '#' ? potIndex - potStateZeroIndex : 0;
            }
            return count.ToString();
        }

        private void Print(StringBuilder potState, int potStateZeroIndex, int? generation = null)
        {
            if (Test)
            {
                var printBuilder = new StringBuilder();
                printBuilder.Append(potState.ToString());
                if (generation.HasValue)
                {
                    printBuilder[potStateZeroIndex] = 'X';
                    Debug.WriteLine($"{generation}: {printBuilder}");
                }
                else
                {
                    Debug.WriteLine($"{potStateZeroIndex}: {printBuilder}");
                }
            }
        }

        public override string Part2()
        {
            var potState = new StringBuilder();
            var potStateZeroIndex = 5;
            if (Test) { potState.Append(".....#..#.#..##......###...###....."); } else { potState.Append(".....#.#..#..###.###.#..###.#####...########.#...#####...##.#....#.####.#.#..#..#.#..###...#..#.#....##....."); };
            var combinations = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Split("=>")).ToDictionary(a => a[0].Trim(), b => b[1].Trim().First());
            var potMargin = 2;
            var refGenerationPattern = string.Empty;
            var generationPatternIsdifferent = true;
            var sumPotValues = (long)0;
            string margin = ".....";

            for (int generation = 1; generationPatternIsdifferent; generation++)
            {
                var referenceState = potState.ToString();
                potState.Clear();
                potState.Append(new string('.', potMargin));

                //Keep left-margin of 5 (based on max growth of 1)
                if (referenceState.Substring(0, 5) != margin)
                {
                    potState.Append(".");
                    //Zero index shifts +1
                    potStateZeroIndex++;
                }

                for (int potIndex = potMargin; potIndex < referenceState.Length - potMargin; potIndex++)
                {
                    var potPattern = referenceState.Substring(potIndex - potMargin, 5);
                    if (combinations.TryGetValue(potPattern, out char check))
                    {
                        potState.Append(check);
                    }
                    else
                    {
                        potState.Append('.');
                    }
                }

                potState.Append(new string('.', potMargin));

                //Keep rigth-margin of 5 (based on max growth of 1)
                if (referenceState.Substring(referenceState.Length - 5, 5) != margin) potState.Append(".");

                var potStateEvalute = potState.ToString();
                var squarePattern = potStateEvalute.Substring(potStateEvalute.IndexOf('#'), potStateEvalute.LastIndexOf('#') - potStateEvalute.IndexOf('#'));

                if (squarePattern != refGenerationPattern) refGenerationPattern = squarePattern;
                else
                {
                    generationPatternIsdifferent = false;
                    Print(potState, potStateZeroIndex, Convert.ToInt32(generation));
                    var countSum = (long)0;
                    for (int potIndex = 0; potIndex < potState.Length; potIndex++)
                    {
                        countSum += potState[potIndex] == '#' ? potIndex - potStateZeroIndex : 0;
                    }
                    sumPotValues = countSum + ((50000000000 - generation) * potStateEvalute.Where(c => c == '#').Count());
                }
            }
            return sumPotValues.ToString();
        }
    }
}

