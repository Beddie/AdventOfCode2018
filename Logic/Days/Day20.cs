using Logic.ExtensionMethods;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Logic.Days
{
    //TODO refactor
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
            return new string[] { "3046", "8545"
            };
        }

        public override string Part1()
        {
            var puzzleRegex = PuzzleInput;
            var parentNode = new PuzzleRoute("");
            var sb = new StringBuilder();

            var puzzleRoute = GetChildBranch(puzzleRegex.Replace("^", "").Replace("$", ""));

            var indexPixel = new Pixel(0, 0);
            foreach (var mainRoute in puzzleRoute.Options)
            {
                AddBranchPixel(mainRoute, indexPixel);
            }

            var grid = CreateGrid(puzzleRoute);
            var shiftPixel = grid.Item3;

            if (Test)
            {
                var newGrid = PrepareGrid(grid.Item2);
                PrintGRID(newGrid);
            }
            var atLeast = Test ? 31 : 1000;

            var coordinates = new List<Pixel>();
            coordinates.AddRange(Coordinates.Where(c => Math.Abs(c.X) % 2 == 0 && Math.Abs(c.Y) % 2 == 0).ToList());

            var distinctCoordinates = new List<Pixel>();
            for (var k = 0; k < coordinates.Count; k++)
            {
                if (!distinctCoordinates.Any(c => c.X == coordinates[k].X + shiftPixel.X && c.Y == coordinates[k].Y + shiftPixel.Y))
                {
                    distinctCoordinates.Add(new Pixel(coordinates[k].X + shiftPixel.X, coordinates[k].Y + shiftPixel.Y));
                }
            }

            coordinates = distinctCoordinates;

            var pixels = new List<Pixel>();
            DFS(grid.Item2, coordinates, grid.Item1, c => pixels.Add(c));

            var cc = pixels.Max(c => c.Distance);

            if (Test)
            {
                foreach (var pix in pixels)
                {
                    grid.Item2[pix.X, pix.Y] = (char)(pix.Distance.ToString().Last());
                }

                Print(grid.Item2);
            }
            return cc.ToString();
        }

        private (Pixel, int[,], Pixel) CreateGrid(PuzzleRoute puzzleRoute)
        {
            var mainPixel = new Pixel(0, 0);
            var minX = Coordinates.Min(c => c.X);
            var minY = Coordinates.Min(c => c.Y);

            var shiftX = 0 - minX + 1;
            var shiftY = 0 - minY + 1;
            var maxX = Coordinates.Max(c => c.X);
            var maxY = Coordinates.Max(c => c.Y);
            var strideX = (maxX + shiftX) + 1;
            var strideY = (maxY + shiftY) + 1;
            var shoftPixel = new Pixel(shiftX, shiftY);
            var grid = new int[strideX + 2, strideY + 2];

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
            return (centrePixel, grid, shoftPixel);
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

        private void PrintGRID(int[,] grid)
        {
            var sb = new StringBuilder();
            for (int y = grid.GetUpperBound(1); y >= 0; y--)
            {
                for (int x = 0; x <= grid.GetUpperBound(0); x++)
                {
                    var val = grid[x, y];
                    sb.Append($"{ (val == 0 ? '#' : (char)val) }");
                }
                sb.AppendLine();
            }
            Debug.WriteLine(sb.ToString());
        }



        private int[,] PrepareGrid(int[,] grid)
        {
            var returnGrid = new int[grid.GetUpperBound(0) + 1, grid.GetUpperBound(1) + 1];

            for (int y = grid.GetUpperBound(1); y >= 0; y--)
            {
                for (int x = 0; x <= grid.GetUpperBound(0); x++)
                {
                    if (y == 0 || x == 0 || y == grid.GetUpperBound(1) || x == grid.GetUpperBound(0))
                    {
                    }
                    else
                    {
                        var val = grid[x, y];
                        returnGrid[x, y] = val > 0 ? '.' : 0;

                    }
                }
            }
            return returnGrid;
        }

        public List<Pixel> Coordinates { get; set; } = new List<Pixel>();

        private void AddBranchPixel(PuzzleRoute puzzleRoute, Pixel indexPixel)
        {
            var mainRoutes = puzzleRoute.Puzzle.ToString().Split(".");

            for (int i = 0; i < mainRoutes.Length; i++)
            {
                var mainRouteValue = mainRoutes[i];

                AddMainPixels(puzzleRoute, indexPixel, mainRouteValue);
                for (int branchi = i; branchi < puzzleRoute.Branches.Count; branchi++)
                {
                    foreach (var optionalRoute in puzzleRoute.Branches[i].Options)
                    {
                        var branchPixel = new Pixel(indexPixel.X, indexPixel.Y);
                        AddBranchPixel(optionalRoute, branchPixel);
                    }
                }
            }
        }

        private void AddMainPixels(PuzzleRoute puzzleRoute, Pixel indexPixel, string puzzleRouteString)
        {
            foreach (var direction in puzzleRouteString)
            {
                switch (direction)
                {
                    case 'N':
                        Coordinates.Add(new Pixel(indexPixel.X, ++indexPixel.Y));
                        Coordinates.Add(new Pixel(indexPixel.X, ++indexPixel.Y));
                        break;
                    case 'E':
                        Coordinates.Add(new Pixel(++indexPixel.X, indexPixel.Y));
                        Coordinates.Add(new Pixel(++indexPixel.X, indexPixel.Y));
                        break;
                    case 'W':
                        Coordinates.Add(new Pixel(--indexPixel.X, indexPixel.Y));
                        Coordinates.Add(new Pixel(--indexPixel.X, indexPixel.Y));
                        break;
                    case 'S':

                        Coordinates.Add(new Pixel(indexPixel.X, --indexPixel.Y));
                        Coordinates.Add(new Pixel(indexPixel.X, --indexPixel.Y));
                        break;
                    default:
                        break;
                }
            }
        }

        public class Pixel
        {

            public int X { get; set; }
            public int Y { get; set; }
            public int Distance { get; set; }

            public Pixel(int x, int y)
            {
                X = x;
                Y = y;
            }

            public List<Pixel> Neighbours(List<Pixel> coords, int[,] grid)
            {
                var adjacentRooms = coords.Where(c =>
                (c.X == X - 2 && c.Y == Y)
                || (c.X == X + 2 && c.Y == Y)
                 || (c.Y == Y + 2 && c.X == X)
                 || (c.Y == Y - 2 && c.X == X)
                ).ToList();

                var retPixels = new List<Pixel>();
                for (int i = 0; i < adjacentRooms.Count; i++)
                {
                    var room = adjacentRooms[i];
                    var shiftPixel = new Pixel((X - room.X) / 2, (Y - room.Y) / 2);
                    var structure = grid[room.X + shiftPixel.X, room.Y + shiftPixel.Y];
                    if (structure == '|' || structure == '-')
                    {
                        retPixels.Add(room);
                    }
                };
                return retPixels;
            }
        }

        public PuzzleRoute GetChildBranch(string str)
        {
            PuzzleRoute mainRoutes = new PuzzleRoute();
            var options = str.GetOptions();
            if (!options.Any()) { options = new List<string> { str }; };

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
                    }
                    else
                    {
                        optionalRoute.Puzzle.Append(character);
                    }
                }
            }
            return mainRoutes;
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
            var puzzleRegex = PuzzleInput;
            var parentNode = new PuzzleRoute("");
            var sb = new StringBuilder();

            var puzzleRoute = GetChildBranch(puzzleRegex.Replace("^", "").Replace("$", ""));

            var indexPixel = new Pixel(0, 0);
            foreach (var mainRoute in puzzleRoute.Options)
            {
                AddBranchPixel(mainRoute, indexPixel);
            }

            var grid = CreateGrid(puzzleRoute);
            var shiftPixel = grid.Item3;

            if (Test)
            {
                var newGrid = PrepareGrid(grid.Item2);
                PrintGRID(newGrid);
            }
            var atLeast = Test ? 31 : 1000;

            var coordinates = new List<Pixel>();
            coordinates.AddRange(Coordinates.Where(c => Math.Abs(c.X) % 2 == 0 && Math.Abs(c.Y) % 2 == 0).ToList());

            var distinctCoordinates = new List<Pixel>();
            for (var k = 0; k < coordinates.Count; k++)
            {
                if (!distinctCoordinates.Any(c => c.X == coordinates[k].X + shiftPixel.X && c.Y == coordinates[k].Y + shiftPixel.Y))
                {
                    distinctCoordinates.Add(new Pixel(coordinates[k].X + shiftPixel.X, coordinates[k].Y + shiftPixel.Y));
                }
            }

            coordinates = distinctCoordinates;

            var pixels = new List<Pixel>();
            DFS(grid.Item2, coordinates, grid.Item1, c => pixels.Add(c));

            var part2 = pixels.Where(c => c.Distance >= (Test ? 31 : 1000)).Count();

            if (Test)
            {
                foreach (var pix in pixels)
                {
                    grid.Item2[pix.X, pix.Y] = (char)(pix.Distance.ToString().Last());
                }
                Print(grid.Item2);
            }

            return part2.ToString();
        }

        public HashSet<Pixel> DFS(int[,] grid, List<Pixel> coordinates, Pixel start, Action<Pixel> preVisit = null)
        {
            var visited = new HashSet<Pixel>();

            var stack = new Stack<Pixel>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var vertex = stack.Pop();

                if (visited.Any(c => c.X == vertex.X && c.Y == vertex.Y))
                    continue;

                if (preVisit != null)
                    preVisit(vertex);

                visited.Add(vertex);

                //find all neighbours
                foreach (var neighbor in vertex.Neighbours(coordinates, grid))
                    if (!visited.Any(c => c.X == neighbor.X && c.Y == neighbor.Y))
                    {
                        neighbor.Distance = vertex.Distance + 1;
                        stack.Push(neighbor);
                    }
            }
            return visited;
        }
    }
}

