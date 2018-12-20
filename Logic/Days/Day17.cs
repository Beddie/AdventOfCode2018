using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Logic.Days
{
    public class Day17 : AdventBase
    {
        public Day17()
        {
            //Test = true;
            PuzzleInput = Test ? Resources.Day17Example : Resources.Day17;
            ID = 17;
            Name = "Day 17: Reservoir Research";
        }


        public override string[] Solution()
        {
            return new string[] {
                "30635", ""
            };
        }

        public class Pixel
        {
            public Pixel(int x, int y)
            {
                X = x;
                Y = y;
            }
            public bool Dead { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            internal void Die()
            {
                Dead = true;
            }
        }


        public override string Part1()
        {
            var gridDefinition = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.Split(new[] { ", ", "=", ".." }, StringSplitOptions.None));

            //Create list of pixels
            var pixels = new List<Pixel>();
            foreach (var definition in gridDefinition)
            {
                var a = definition[0];
                var aVal = Convert.ToInt32(definition[1]);
                var b = definition[2];
                var bValFrom = Convert.ToInt32(definition[3]);
                var bValTo = Convert.ToInt32(definition[4]);

                for (int i = bValFrom; i <= bValTo; i++) pixels.Add((a == "x") ? new Pixel(aVal, i) : new Pixel(i, aVal));
            }

            var gridXmin = pixels.Min(c => c.X);
            var gridXmax = pixels.Max(c => c.X);
            var gridYmax = pixels.Max(c => c.Y) + 1;

            var grid = new int[gridXmax + 2, gridYmax];
            var pixelHashset = pixels.ToHashSet();
            for (int y = 0; y < gridYmax; y++)
            {
                for (int x = 0; x <= gridXmax; x++)
                {
                    grid[x, y] = '.';
                }
            }
            foreach (var pix in pixelHashset)
            {
                grid[pix.X, pix.Y] = '#';
            }

            grid[500, 0] = '+';
            var dropsOfWater = new List<Pixel> { new Pixel(500, 0) };
            var gameOver = gridYmax;
            var tellen = 0;
            var debug = false;
            while (dropsOfWater.Any())
            {
                tellen++;
                dropsOfWater.RemoveAll(c => c.Dead);


                var doubles = dropsOfWater.GroupBy(c => new { c.X, c.Y }).Where(c => c.Count() > 1);
                foreach (var db in doubles)
                {
                    dropsOfWater.Remove(dropsOfWater.Where(c => c.X == db.Key.X && c.Y == db.Key.Y).First());
                }



                for (int i = 0; i < dropsOfWater.Count; i++)
                {
                    var droppingWater = dropsOfWater[i];

                    if (droppingWater.Y + 1 == gameOver) droppingWater.Die();

                    if (!droppingWater.Dead)
                    {
                        //check position below
                        var nextPixelAfterGravity = grid[droppingWater.X, droppingWater.Y + 1];

                        switch (nextPixelAfterGravity)
                        {
                            case '#':
                                FillHorizontal(grid, dropsOfWater, droppingWater, debug);
                                break;
                            case '~':
                                if (!IsStandingWater(grid, dropsOfWater, droppingWater))
                                {
                                    droppingWater.Die();
                                }
                                else FillHorizontal(grid, dropsOfWater, droppingWater);
                                break;
                            case '.':
                                droppingWater.Y++;
                                grid[droppingWater.X, droppingWater.Y] = '|';
                                break;
                            case '|':
                                droppingWater.Die();
                                //grid[droppingWater.X, droppingWater.Y] = '|';
                                break;
                            default:
                                throw new Exception("Het bestaat niet!");
                        }
                    }
                    if (droppingWater.Y == gameOver) droppingWater.Die();

                }


            }
            Print(grid); //589
            //Spring lives at x=500,y=0
            return CountWater(grid);
        }

        private bool IsStandingWater(int[,] grid, List<Pixel> dropsOfWater, Pixel droppingWater)
        {
            //scan line below
            //Dermine all X values to scan
            int? scanLeftX = null;
            int? scanRightX = null;
            var standingLeft = false;
            var standingRight = false;
            var originalScanX = droppingWater.X;
            var originalScanY = droppingWater.Y;

            droppingWater.Y++;

            while (!scanLeftX.HasValue)
            {
                if (grid[droppingWater.X - 1, droppingWater.Y] == '~')
                {
                }
                else if (grid[droppingWater.X - 1, droppingWater.Y] == '#')
                {
                    standingLeft = true;
                }
                else
                {
                    scanLeftX = droppingWater.X;
                }

                droppingWater.X--;
            }
            droppingWater.X = originalScanX;
            while (!scanRightX.HasValue)
            {
                if (grid[droppingWater.X + 1, droppingWater.Y] == '~')
                {
                }
                else if (grid[droppingWater.X + 1, droppingWater.Y] == '#')
                {
                    standingRight = true;
                }
                else
                {
                    scanRightX = droppingWater.X;
                }

                droppingWater.X++;
            }
            droppingWater.X = originalScanX;
            droppingWater.Y = originalScanY;

            return standingLeft && standingRight;
        }

        private string CountWater(int[,] grid)
        {
            var count = 0;

            //Start counting from first Y with #
            //Start counting from first Y with #
            var firstY = (int?)null;
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    var gridVal = grid[x, y];
                    if (gridVal != 0)
                    {
                        if (!firstY.HasValue && gridVal == '#') firstY = y;
                    }

                }
            }

            for (int y = firstY.Value; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    var gridVal = grid[x, y];
                    if (gridVal != 0)
                    {
                        if (gridVal == '~' || gridVal == '|')
                        {
                            count++;
                        }
                    }
                }
            }
            return count.ToString();
        }

        public void FillHorizontal(int[,] grid, List<Pixel> dropsOfWater, Pixel droppingWater, bool debug = false)
        {
            var goHorizontalLeft = true;
            var originalX = droppingWater.X;
            var originalY = droppingWater.Y;

            //Check if standing water is old standing water.
            grid[droppingWater.X, droppingWater.Y] = '~';
            while (goHorizontalLeft)
            {

                var nextLeft = grid[droppingWater.X - 1, droppingWater.Y];
                var nextLeftSurface = grid[droppingWater.X - 1, droppingWater.Y + 1];
                if ((nextLeft == '.') && nextLeftSurface != '.')
                {
                    grid[droppingWater.X - 1, droppingWater.Y] = '~';
                    droppingWater.X--;
                }
                else if ((nextLeft == '|') && nextLeftSurface != '.')
                {
                    droppingWater.X--;
                    if (!dropsOfWater.Any(c => c != droppingWater && c.X == droppingWater.X && c.Y == droppingWater.Y))
                    {
                        grid[droppingWater.X, droppingWater.Y] = '~';
                    }
                }
                else if (nextLeftSurface == '.')
                {
                    droppingWater.Die();
                    dropsOfWater.Add(new Pixel(droppingWater.X - 1, droppingWater.Y));
                    grid[droppingWater.X - 1, droppingWater.Y] = '|';
                    droppingWater.X--;
                    goHorizontalLeft = false;
                }
                else if (nextLeft == '#' || nextLeft == '|' || nextLeft == '~') goHorizontalLeft = false;

            }
            var goHorizontalRight = true;
            droppingWater.X = originalX;

            while (goHorizontalRight)
            {
                var nextRight = grid[droppingWater.X + 1, droppingWater.Y];
                var nextRightSurface = grid[droppingWater.X + 1, droppingWater.Y + 1];
                if ((nextRight == '.') && nextRightSurface != '.')
                {
                    grid[droppingWater.X + 1, droppingWater.Y] = '~';
                    droppingWater.X++;
                }
                else if ((nextRight == '|') && nextRightSurface != '.')
                {
                    droppingWater.X++;
                    if (!dropsOfWater.Any(c => c != droppingWater && c.X == droppingWater.X && c.Y == droppingWater.Y))
                    {
                        grid[droppingWater.X, droppingWater.Y] = '~';
                    }
                }
                else if (nextRightSurface == '.')
                {
                    droppingWater.Die();
                    dropsOfWater.Add(new Pixel(droppingWater.X + 1, droppingWater.Y));
                    grid[droppingWater.X + 1, droppingWater.Y] = '|';
                    droppingWater.X++;
                    goHorizontalRight = false;
                }
                else if (nextRight == '#' || nextRight == '|' || nextRight == '~') goHorizontalRight = false;
            }
            droppingWater.X = originalX;

            var nextTopStop = grid[droppingWater.X, droppingWater.Y - 1] == '.';
            if (grid[droppingWater.X, droppingWater.Y - 1] == '~')
            {
                droppingWater.Die();
            }

            if (!nextTopStop) droppingWater.Y = originalY - 1;
            else
            {
                if (!droppingWater.Dead)
                {
                    //Dermine all X values to scan
                    int? scanLeftX = null;
                    int? scanRightX = null;
                    var found = false;
                    var originalScanX = droppingWater.X;
                    while (!scanLeftX.HasValue)
                    {

                        if (grid[droppingWater.X - 1, droppingWater.Y] == '#')
                        {
                            //found = true;
                            scanLeftX = droppingWater.X;

                        }
                        droppingWater.X--;
                    }
                    droppingWater.X = originalScanX;
                    while (!scanRightX.HasValue)
                    {
                        if (grid[droppingWater.X + 1, droppingWater.Y] == '#')
                        {
                            //found = true;
                            scanRightX = droppingWater.X;

                        }
                        droppingWater.X++;
                    }

                    for (int scanx = scanLeftX.Value; scanx <= scanRightX; scanx++)
                    {
                        if (grid[scanx, droppingWater.Y - 1] == '|')
                        {
                            found = true;
                            droppingWater.X = scanx;
                            droppingWater.Y--;
                            break;
                        }
                    }

                    if (found)
                    {

                        for (int scanx = scanLeftX.Value; scanx <= scanRightX; scanx++)
                        {
                            if (dropsOfWater.Any(c => c != droppingWater && c.X == droppingWater.X && c.Y == droppingWater.Y + 1))
                            {
                                dropsOfWater.Where(c => c != droppingWater && c.X == droppingWater.X && c.Y == droppingWater.Y + 1).ToList().ForEach(c => c.Die());
                            }
                        }
                    }
                }
            };
        }

        private void Print(int[,] grid, int firstX = 0, int lastX = 0, int firstY = 0, int lastY = 0)
        {
            var sb = new StringBuilder();
            for (int y = firstY; y < (lastY == 0 ? grid.GetLength(1) : lastY); y++)
            {
                for (int x = firstX; x < (lastX == 0 ? grid.GetLength(0) : lastX); x++)
                {
                    if (grid[x, y] != 0)
                    {
                        sb.Append((char)grid[x, y]);
                    }
                }
                sb.AppendLine();
            }
            Debug.WriteLine(sb.ToString());
        }

        public override string Part2()
        {
            var gridDefinition = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.Split(new[] { ", ", "=", ".." }, StringSplitOptions.None));

            //Create list of pixels
            var pixels = new List<Pixel>();
            foreach (var definition in gridDefinition)
            {
                var a = definition[0];
                var aVal = Convert.ToInt32(definition[1]);
                var b = definition[2];
                var bValFrom = Convert.ToInt32(definition[3]);
                var bValTo = Convert.ToInt32(definition[4]);

                for (int i = bValFrom; i <= bValTo; i++) pixels.Add((a == "x") ? new Pixel(aVal, i) : new Pixel(i, aVal));
            }

            var gridXmin = pixels.Min(c => c.X);
            var gridXmax = pixels.Max(c => c.X);
            var gridYmax = pixels.Max(c => c.Y) + 1;

            var grid = new int[gridXmax + 2, gridYmax];
            var pixelHashset = pixels.ToHashSet();
            for (int y = 0; y < gridYmax; y++)
            {
                for (int x = 0; x <= gridXmax; x++)
                {
                    grid[x, y] = '.';
                }
            }
            foreach (var pix in pixelHashset)
            {
                grid[pix.X, pix.Y] = '#';
            }

            grid[500, 0] = '+';
            var dropsOfWater = new List<Pixel> { new Pixel(500, 0) };
            var gameOver = gridYmax;
            var tellen = 0;
            var debug = false;
            while (dropsOfWater.Any())
            {
                tellen++;
                dropsOfWater.RemoveAll(c => c.Dead);


                var doubles = dropsOfWater.GroupBy(c => new { c.X, c.Y }).Where(c => c.Count() > 1);
                foreach (var db in doubles)
                {
                    dropsOfWater.Remove(dropsOfWater.Where(c => c.X == db.Key.X && c.Y == db.Key.Y).First());
                }

                for (int i = 0; i < dropsOfWater.Count; i++)
                {
                    var droppingWater = dropsOfWater[i];

                    if (droppingWater.Y + 1 == gameOver) droppingWater.Die();

                    if (!droppingWater.Dead)
                    {
                        //check position below
                        var nextPixelAfterGravity = grid[droppingWater.X, droppingWater.Y + 1];

                        switch (nextPixelAfterGravity)
                        {
                            case '#':
                                FillHorizontal(grid, dropsOfWater, droppingWater, debug);
                                break;
                            case '~':
                                if (!IsStandingWater(grid, dropsOfWater, droppingWater))
                                {
                                    droppingWater.Die();
                                }
                                else FillHorizontal(grid, dropsOfWater, droppingWater);
                                break;
                            case '.':
                                droppingWater.Y++;
                                grid[droppingWater.X, droppingWater.Y] = '|';
                                break;
                            case '|':
                                droppingWater.Die();
                                break;
                            default:
                                throw new Exception("Het bestaat niet!");
                        }
                    }
                    if (droppingWater.Y == gameOver) droppingWater.Die();

                }


            }
            Print(grid); //589
            //Spring lives at x=500,y=0
            return CountWater(grid);
        }
    }
}

