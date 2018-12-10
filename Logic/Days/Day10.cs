using Logic.Interface;
using Logic.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day10 : AdventBase, AdventInterface
    {
        public Day10()
        {
            // Test = true;
            PuzzleInput = Test ? Resources.Day10Example : Resources.Day10;
        }

        public string[] Solution()
        {
            return new string[] {
                @"
#....#..#####...#....#..######..######..#....#...####......###......................................
##...#..#....#..#....#..#............#..#....#..#....#......#.......................................
##...#..#....#..#....#..#............#..#....#..#...........#.......................................
#.#..#..#....#..#....#..#...........#...#....#..#...........#.......................................
#.#..#..#####...######..#####......#....######..#...........#.......................................
#..#.#..#....#..#....#..#.........#.....#....#..#...........#.......................................
#..#.#..#....#..#....#..#........#......#....#..#...........#.......................................
#...##..#....#..#....#..#.......#.......#....#..#.......#...#.......................................
#...##..#....#..#....#..#.......#.......#....#..#....#..#...#.......................................
#....#..#####...#....#..######..######..#....#...####....###........................................
",
                "10558"
            };
        }

        public string Part1()
        {
            var regex = new Regex(@"\<(.*?)\>");
            var points = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).AsParallel()
                .Select(c => regex.Matches(c)).Select((puzzle) => new Point(puzzle)).AsParallel().ToHashSet();

            var returnVal = string.Empty;
            while (true)
            {
                points.AsParallel().ForAll(point => point.Move());
                returnVal = GetGridResult(points, true);
                if (returnVal != null) break;
            }
            //TODO Use OCR for .NET CORE when available
            return returnVal;
        }

        private string GetGridResult(HashSet<Point> points, bool getText = false)
        {
            //Check if points are on 1 line
            var YDifference = points.Max(c => c.PositionY) - points.Min(c => c.PositionY);

            if (YDifference < (Test ? 8 : 11))
            {
                if (!getText) return "DONE";

                var strideX = Test ? 30 : 100;
                var strideY = Test ? 10 : 10;
                var grid = new byte[strideX * strideY];

                var upperLeftY = points.Min(c => c.PositionY);
                var upperLeftX = points.Min(c => c.PositionX);

                //Fill grid with values
                foreach (var item in points)
                {
                    grid[Math.Abs(item.PositionX) - upperLeftX + ((Math.Abs(item.PositionY) - upperLeftY) * strideX)] = 1;
                }

                var sb = new StringBuilder();
                sb.AppendLine();
                for (int y = 0; y < strideY; y++)
                {
                    for (int x = 0; x < strideX; x++)
                    {
                        var position = x + (y * strideX);
                        sb.Append((grid[position] == 1) ? "#" : ".");
                    }
                    sb.AppendLine();
                }

                if (Test) Debug.WriteLine(sb.ToString());
                return sb.ToString();
            }
            return null;
        }

        public string Part2()
        {
            var regex = new Regex(@"\<(.*?)\>");
            var points = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).AsParallel()
                .Select(c => regex.Matches(c)).Select((puzzle) => new Point(puzzle)).AsParallel().ToHashSet();

            var seconds = 0;
            while (true)
            {
                seconds++;
                points.AsParallel().ForAll(point => point.Move());
                if (!string.IsNullOrEmpty(GetGridResult(points))) break;
            }
            return seconds.ToString();
        }

        private class Point
        {
            public Point(MatchCollection matchCollection)
            {
                PositionX = Convert.ToInt32(matchCollection[0].Groups[1].Value.Split(',')[0]);
                PositionY = Convert.ToInt32(matchCollection[0].Groups[1].Value.Split(',')[1]);
                VelocityX = Convert.ToInt32(matchCollection[1].Groups[1].Value.Split(',')[0]);
                VelocityY = Convert.ToInt32(matchCollection[1].Groups[1].Value.Split(',')[1]);
            }

            public int PositionX { get; set; }
            public int PositionY { get; set; }
            public int VelocityX { get; set; }
            public int VelocityY { get; set; }

            public void Move()
            {
                PositionX += VelocityX;
                PositionY += VelocityY;
            }
        }

        public string GetListName()
        {
            return "Day 10: The Stars Align";
        }

        public int GetID()
        {
            return 10;
        }
    }
}

