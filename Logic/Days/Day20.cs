using Logic.ExtensionMethods;
using Logic.Properties;
using Logic.Service.Pathfinder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
            //puzzleRegex = PuzzleInput;
            //puzzleRegex = "^WNE$";
            //puzzleRegex = "^ENWWW(NEEE|SSE(EE|N))$";
            //puzzleRegex = "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$";

            //puzzleRegex = "^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$";
            var parentNode = new PuzzleRoute("");
            var sb = new StringBuilder();

            var puzzleRoute = CreateChilds(puzzleRegex.Replace("^", "").Replace("$", ""));

            var coordinates = new List<Pixel>();

            var indexPixel = new Pixel(0, 0);
            foreach (var mainRoute in puzzleRoute.Options)
            {
                AddBranchPixel(mainRoute, indexPixel);
            }

            var grid = CreateGrid(puzzleRoute);
            var rooms = FindRooms(grid);
            //Print(grid.Item2);

            
            var longestPath = 0;
            Print(grid.Item2);
            var longPath = new List<PathFinderNodeDay20>();
            for (int i = 0; i < rooms.Count; i++)
            {
                var pathfinder = new PathFinderDay20(grid.Item2);

                //if (grid.Item1.X == rooms[i].X && grid.Item1.Y == rooms[i].Y) {
                //    var bv = 1;
                //}
                var countPath = pathfinder.FindPath(new Point(grid.Item1.X, grid.Item1.Y), new Point(rooms[i].X, rooms[i].Y));
                if (countPath != null)
                {
                    if (countPath.Count > longestPath)
                    {
                        longestPath = countPath.Count;
                        longPath = countPath;
                    }
                }
            }


            foreach (var step in longPath)
            {
                grid.Item2[step.X, step.Y] = 'S';
            }
            grid.Item2[longPath.Last().X, longPath.Last().Y] = 'E';
            grid.Item2[longPath.First().X, longPath.First().Y] = 'F';
            Print(grid.Item2);
            var bb = ((longestPath - 1) / 2).ToString();
            return bb;
            //749 TO LOW
        }

        //public static bool IsAdjacent(Pixel fromindex, Pixel relatedIndex) =>
        //           (relatedIndex.X == fromindex.X && relatedIndex.Y == fromindex.Y + 1) ||
        //           (relatedIndex.X == fromindex.X && relatedIndex.Y == fromindex.Y - 1) ||
        //           (relatedIndex.X == fromindex.X + 1 && relatedIndex.Y == fromindex.Y) ||
        //           (relatedIndex.X == fromindex.X - 1 && relatedIndex.Y == fromindex.Y);

        private List<Pixel> FindRooms((Pixel,int[,]) grid)
        {
            var rooms = new List<Pixel>();

            for (int y = grid.Item2.GetUpperBound(1) - 1; y >= 1; y--)
            {
                for (int x = 1; x <= grid.Item2.GetUpperBound(0) - 1; x++)
                {
                    if (grid.Item2[x, y] == '.')
                    {
                        var left = grid.Item2[x - 1, y];
                        var right = grid.Item2[x + 1, y];
                        var up = grid.Item2[x, y + 1];
                        var down = grid.Item2[x, y - 1];
                        if (left + right + up + down == '-' || left + right + up + down == '|')
                        {
                            //if (grid.Item1.X != x && grid.Item1.Y != y)
                                rooms.Add(new Pixel(x, y));
                        }
                    }
                }
            }
            return rooms;
        }

        private (Pixel, int[,]) CreateGrid(PuzzleRoute puzzleRoute)
        {
            var mainPixel = new Pixel(0, 0); // new Pixel(indexPixel.X, indexPixel.Y);
            var minX = Coordinates.Min(c => c.X);
            var minY = Coordinates.Min(c => c.Y);

            var shiftX = 0 - minX + 1;
            var shiftY = 0 - minY + 1;
            var maxX = Coordinates.Max(c => c.X);
            var maxY = Coordinates.Max(c => c.Y);
            var strideX = (maxX + shiftX) + 1;
            var strideY = (maxY + shiftY) + 1;

            var grid = new int[strideX + 2, strideY + 2];
            // WalkGrid(puzzleRoute, mainPixel);
            foreach (var coordinate in Coordinates.Where(c => (Math.Abs(c.X) % 2 == 0 && Math.Abs(c.Y) % 2 == 0)))
            {
                grid[coordinate.X + shiftX, coordinate.Y + shiftY] = '.';
            }
            foreach (var coordinate in Coordinates.Where(c => Math.Abs(c.X) % 2 == 1 || Math.Abs(c.X) == 1))
            {
                grid[coordinate.X + shiftX, coordinate.Y + shiftY] = '|';
            }

            foreach (var coordinate in Coordinates.Where(c => Math.Abs(c.Y) % 2 == 1 || Math.Abs(c.Y) == 1))
            {
                grid[coordinate.X + shiftX, coordinate.Y + shiftY] = '-';
            }

            grid[0 + shiftX, 0 + shiftY] = 'O';


            var centrePixel = new Pixel(shiftX, shiftY);
            return (centrePixel, grid);
        }

        private void Print(int[,] grid)
        {
            var sb = new StringBuilder();
            for (int y = grid.GetUpperBound(1); y >= 0; y--)
            {
                for (int x = 0; x <= grid.GetUpperBound(0); x++)
                {
                    if (y == 0 || x == 0 || y == grid.GetUpperBound(1) || x == grid.GetUpperBound(0))
                    {
                        sb.Append("#");
                    }
                    else
                    {
                        var val = grid[x, y];
                        if (val == 0 && ((x % 2 == 1 && y % 2 == 0) || (y % 2 == 1 && x % 2 == 0))) val = '#';
                        sb.Append($"{ (val == 0 ? '#' : (char)val) }");
                    }
                }
                sb.AppendLine();
            }
            Debug.WriteLine(sb.ToString());
        }

        private List<Pixel> Coordinates = new List<Pixel>();

        private void AddBranchPixel(PuzzleRoute puzzleRoute, Pixel indexPixel)
        {
            var mainPixel = indexPixel;
            var mainRoutes = puzzleRoute.Puzzle.ToString().Split(".");

            for (int i = 0; i < mainRoutes.Length; i++)
            {
                var mainRouteValue = mainRoutes[i];

                if (!string.IsNullOrEmpty(mainRouteValue))
                {
                    AddMainPixels(puzzleRoute, mainPixel, mainRouteValue);
                    for (int branchi = i; branchi < puzzleRoute.Branches.Count; branchi++)
                    {
                        //  foreach (var branch in puzzleRoute.Branches)
                        //{
                        foreach (var optionalRoute in puzzleRoute.Branches[i].Options)
                        {
                            var branchPixel = new Pixel(mainPixel.X, mainPixel.Y);
                            AddBranchPixel(optionalRoute, branchPixel);
                        }
                    }
                }
            }
        }

        private void AddMainPixels(PuzzleRoute puzzleRoute, Pixel indexPixel)
        {
            foreach (var direction in puzzleRoute.Puzzle.ToString())
            {
                switch (direction)
                {
                    case 'N':
                        puzzleRoute.Coordinates.Add(new Pixel(indexPixel.X, ++indexPixel.Y));
                        puzzleRoute.Coordinates.Add(new Pixel(indexPixel.X, ++indexPixel.Y));
                        break;
                    case 'E':
                        puzzleRoute.Coordinates.Add(new Pixel(++indexPixel.X, indexPixel.Y));
                        puzzleRoute.Coordinates.Add(new Pixel(++indexPixel.X, indexPixel.Y));
                        break;
                    case 'W':
                        puzzleRoute.Coordinates.Add(new Pixel(--indexPixel.X, indexPixel.Y));
                        puzzleRoute.Coordinates.Add(new Pixel(--indexPixel.X, indexPixel.Y));
                        break;
                    case 'S':
                        puzzleRoute.Coordinates.Add(new Pixel(indexPixel.X, --indexPixel.Y));
                        puzzleRoute.Coordinates.Add(new Pixel(indexPixel.X, --indexPixel.Y));
                        break;
                    default:
                        break;
                }
            }
            Coordinates.AddRange(puzzleRoute.Coordinates);
        }


        private void AddMainPixels(PuzzleRoute puzzleRoute, Pixel indexPixel, string puzzleRouteString)
        {
            foreach (var direction in puzzleRouteString)
            {
                switch (direction)
                {
                    case 'N':
                        puzzleRoute.Coordinates.Add(new Pixel(indexPixel.X, ++indexPixel.Y));
                        puzzleRoute.Coordinates.Add(new Pixel(indexPixel.X, ++indexPixel.Y));
                        break;
                    case 'E':
                        puzzleRoute.Coordinates.Add(new Pixel(++indexPixel.X, indexPixel.Y));
                        puzzleRoute.Coordinates.Add(new Pixel(++indexPixel.X, indexPixel.Y));
                        break;
                    case 'W':
                        puzzleRoute.Coordinates.Add(new Pixel(--indexPixel.X, indexPixel.Y));
                        puzzleRoute.Coordinates.Add(new Pixel(--indexPixel.X, indexPixel.Y));
                        break;
                    case 'S':
                        puzzleRoute.Coordinates.Add(new Pixel(indexPixel.X, --indexPixel.Y));
                        puzzleRoute.Coordinates.Add(new Pixel(indexPixel.X, --indexPixel.Y));
                        break;
                    default:
                        break;
                }
            }
            Coordinates.AddRange(puzzleRoute.Coordinates);
        }

        public class Pixel
        {
            public Pixel(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
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
                        optionalRoute.Puzzle.Append('.');
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
                        //if (createNewPuzzleRoute)
                        //{
                        //    optionalRoute = new PuzzleRoute();
                        //    mainRoutes.Options.Add(optionalRoute);
                        //    createNewPuzzleRoute = false;
                        //}
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
            public List<Pixel> Coordinates { get; set; } = new List<Pixel>();
        }

        public override string Part2()
        {
            return "";
        }
    }
}

