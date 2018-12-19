using Logic.Interface;
using Logic.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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

            var grid = new int[860, gridYmax];
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
                dropsOfWater.RemoveAll(c=> c.Dead);
                //check current water position (start grid[500, 0])

                //if (tellen > 126)
                //{
                //    Print(grid, 450, 550, 180);
                //    debug = true;
                //}
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
                                FillHorizontal(grid, dropsOfWater, droppingWater);
                                break;
                            case '.':
                                droppingWater.Y++;
                                grid[droppingWater.X, droppingWater.Y] = '|';
                                break;
                            default:
                                throw new Exception("Het bestaat niet!");
                        }
                    }
                    if (droppingWater.Y == gameOver) droppingWater.Die();

                }

            }
            Print(grid);
            //Spring lives at x=500,y=0

            string part1 = CountWater(grid);


            return "";
        }

        private string CountWater(int[,] grid)
        {
            var count = 0;
            for (int y = 0; y < grid.GetLength(1); y++)
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

            grid[droppingWater.X, droppingWater.Y] = '~';
            while (goHorizontalLeft)
            {
               
                var nextLeft = grid[droppingWater.X - 1, droppingWater.Y];
                var nextLeftSurface = grid[droppingWater.X - 1, droppingWater.Y + 1];
                if ((nextLeft == '.') && nextLeftSurface != '.') {
                    grid[droppingWater.X - 1, droppingWater.Y] = '~';
                    droppingWater.X--;
                }
                else if (nextLeftSurface == '.')
                {
                    droppingWater.Die();
                    dropsOfWater.Add(new Pixel(droppingWater.X - 1, droppingWater.Y));
                    grid[droppingWater.X - 1, droppingWater.Y] = '|';
                    droppingWater.X--;
                    goHorizontalLeft = false;
                }
                else if (nextLeft == '#' || nextLeft == '|' ||  nextLeft == '~') goHorizontalLeft = false;

            }
            var goHorizontalRight = true;
            droppingWater.X = originalX;
            if (debug)
            {
                Print(grid, 450, 550, 80);
            }
            while (goHorizontalRight)
            {
                var nextRight = grid[droppingWater.X + 1, droppingWater.Y];
                var nextRightSurface = grid[droppingWater.X + 1, droppingWater.Y + 1];
                if ((nextRight == '.') && nextRightSurface != '.')
                {
                    grid[droppingWater.X + 1, droppingWater.Y] = '~';
                    droppingWater.X++;
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

            //if (nextTopStop) {
            //    //find stream (scan)
            //    var scanLeft = true;
            //    var scanRight = true;
            //    var found = false;
            //    var originalScanX = droppingWater.X;
            //    while (scanLeft)
            //    {
            //        droppingWater.X--;
            //        if (grid[droppingWater.X, droppingWater.Y - 1] == '|')
            //        {
            //            found = true;
            //            scanLeft = false;
            //        }
            //        else if (grid[droppingWater.X, droppingWater.Y - 1] == '#')
            //        {
            //            scanLeft = false;
                       
            //        }
            //    }
            //    while (scanRight && !found)
            //    {
            //        droppingWater.X++;
            //        if (grid[droppingWater.X, droppingWater.Y - 1] == '|')
            //        {
            //            found = true;
            //            scanRight = false;
            //        }
            //        else if (grid[droppingWater.X, droppingWater.Y - 1] == '#')
            //        {
            //            scanLeft = false;
            //        }
            //    }
            //}

            droppingWater.Y = originalY - 1;
        }

        private void Print(int[,] grid, int firstX = 0, int lastX = 0, int lastY = 0)
        {
                var sb = new StringBuilder();
                for (int y = 0; y < (lastY == 0 ? grid.GetLength(1): lastY); y++)
                {
                    for (int x = firstX; x < (lastX == 0 ? grid.GetLength(0): lastX); x++)
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
            return "";
        }
    }
}

