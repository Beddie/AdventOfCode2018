using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Days
{
    public class Day20 : AdventBase
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
            var puzzleRegex = "^ENWWW(NEEE|SSE)$"; //^ENWWW(NEEE|SSE(EE|N))$
            //var bb = puzzleRegex.Split(new[] { "(", ")" }, StringSplitOptions.None).Select(s => s.Split('|'));

            var parentNode = new HierachyPuzzle("");
            var childNode = new HierachyPuzzle("");
            var sb = new StringBuilder();

            for (int i = 0; i < puzzleRegex.Length; i++)
            {
                var character = puzzleRegex[i];
                switch (character)
                {
                    //create childnode and save parentnode
                    case '(':
                        parentNode.Puzzle = sb.ToString();
                        childNode = new HierachyPuzzle();
                        //parentNode.Children.Add(childNode);

                        sb.Clear();
                        break;
                    //add childnode
                    case ')':
                        childNode.Puzzle = sb.ToString();
                        //childNode = new HierachyPuzzle();
                        sb.Clear();
                        break;
                    //create another childnode
                    case '|':
                        parentNode.Children.Add(childNode);
                        childNode = new HierachyPuzzle();
                        sb.Clear();
                        break;
                    default:
                        sb.Append(character);
                        break;
                }
            }
            foreach (var character in puzzleRegex)
            {
                switch (character)
                {
                    //create childnode and save parentnode
                    case '(':
                        parentNode.Puzzle = sb.ToString();
                        childNode = new HierachyPuzzle();
                        //parentNode.Children.Add(childNode);

                        sb.Clear();
                        break;
                    //add childnode
                    case ')':
                        childNode.Puzzle = sb.ToString();
                        //childNode = new HierachyPuzzle();
                        sb.Clear();
                        break;
                    //create another childnode
                    case '|':
                        parentNode.Children.Add(childNode);
                        childNode = new HierachyPuzzle();
                        sb.Clear();
                        break;
                    default:
                        sb.Append(character);
                        break;
                }
            }
            return "";
        }

        private void AddNode(HierachyPuzzle parent) {


        }

        public class HierachyPuzzle
        {
            public HierachyPuzzle(string input)
            {
                Puzzle = input;
            }
            public HierachyPuzzle()
            {
            }
            public string Puzzle { get; set; }
            public List<HierachyPuzzle> Children { get; set; } = new List<HierachyPuzzle>();
        }

        public override string Part2()
        {
            return "";
        }
    }
}

