using Logic.Properties;
using System;
using System.Linq;
using System.Text;

namespace Logic.Days
{
    public class Day20: AdventBase
    {
        public Day20()
        {
            // Test = true;
            PuzzleInput = Test ? Resources.Day20Example : Resources.Day20;
            ID = 20;
            Name = "Day 20: A Regular Map";
        }

        public override string[] Solution()
        {
            return new string[] {
            };
        }

        public override string Part1()
        {
            //var puzzleRegex = "^ENWWW(NEEE|SSE(EE|N))$";
            //var bb = puzzleRegex.Split(new[] { "(", ")" },StringSplitOptions.None).Select(s => s.Split('|'));

            //var startNode = new HierachyPuzzle("");
            //var sb = new StringBuilder();

            //foreach (var character in puzzleRegex)
            //{
            //    if (character == '(') {
            //        var child = new HierachyPuzzle();
            //        startNode.Child = child;
            //        if (character == '|')
            //        {
            //            var childchild = new HierachyPuzzle();

            //        }
            //    else { sb.Append(character); }
                
                
            //}


            return "";
        }

        public class HierachyPuzzle
        {
            public HierachyPuzzle(string input) {
                Puzzle = input;
            }
            public string Puzzle { get; set; }
            public HierachyPuzzle Child { get; set; }
        }

        public override string Part2()
        {
            return "";
        }
    }
}

