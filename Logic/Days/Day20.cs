using Logic.ExtensionMethods;
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
            var puzzleRegex = "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$";
            var parentNode = new PuzzleRoute("");
            var sb = new StringBuilder();

            var puzzleRoute = CreateChilds(puzzleRegex.Replace("^", "").Replace("$", ""));

            return "";
        }

        public PuzzleRoute GetChildBranch(string str)
        {
            PuzzleRoute mainRoutes = new PuzzleRoute();
            var options = str.GetOptions();
            if (!options.Any()) { options = new List<string> { str }; };

            var createNewPuzzleRoute = false;
          
            for (int i = 0; i < options.Count; i++)
            {
                PuzzleRoute optionalRoute = new PuzzleRoute();
                mainRoutes.Options.Add(optionalRoute);
                var opt = options[i];
                while (opt.Length > 0)
                {
                    var character = opt.First();
                    opt = opt.Remove(0, 1);
                    if (character == '(')
                    {
                        var getTotalBranch = opt.GetStringTillClosing();
                        var childBranch = GetChildBranch(getTotalBranch);
                        optionalRoute.Branches.Add(childBranch);
                        opt = opt.Remove(0, getTotalBranch.Length);
                    }
                    else if (character == ')')
                    {
                        createNewPuzzleRoute = true;
                    }
                    else
                    {
                        if (createNewPuzzleRoute)
                        {
                            optionalRoute = new PuzzleRoute();
                            mainRoutes.Options.Add(optionalRoute);
                            createNewPuzzleRoute = false;
                        }
                        optionalRoute.Puzzle.Append(character);
                    }
                }
            }
            return mainRoutes;
        }

       
        private PuzzleRoute CreateChilds(string stream)
        {
            //Get everything from first ( to  last closing )
            var subChilds = GetChildBranch(stream);
            return subChilds;
        }

        public class PuzzleRoute
        {
            public PuzzleRoute(string input)
            {
                Puzzle = new StringBuilder(input);
            }
            public PuzzleRoute()
            {
            }
            public StringBuilder Puzzle { get; set; } = new StringBuilder();
            public List<PuzzleRoute> Branches { get; set; } = new List<PuzzleRoute>();
            public List<PuzzleRoute> Options { get; set; } = new List<PuzzleRoute>();
        }

        public override string Part2()
        {
            return "";
        }
    }
}

