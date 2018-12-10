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
    public class Day6 : AdventBase
    {
        public Day6()
        {
            // Test = true;
            PuzzleInput = Test ? Resources.Day6Example : Resources.Day6;
            ID = 6;
            Name = "Day 6: Chronal Coordinates";
        }

        public override string[] Solution()
        {
            return new string[] {
                "3969",
                "42123"
            };
        }

        private HashSet<Coordinate> InfiniteCoordinates { get; set; } = new HashSet<Coordinate>();
        private int GridStride { get; set; }

        private class Coordinate
        {
            public int xVal { get; set; }
            public int yVal { get; set; }
            public char Value { get; set; }
        }

        private class GridCoordinate : Coordinate
        {
            public bool IsInfinite { get; set; }
            public int Amount { get; set; }
        }

        private object lockVar = new object();

        private Coordinate GetNearestManhattenDistanceValue(Coordinate pixel, HashSet<GridCoordinate> coordinates)
        {
            var coordinatesWithDistance = new Dictionary<Coordinate, int>();
            foreach (var coordinate in coordinates)
            {
                var distance = GetManhattenDistance(pixel, coordinate);
                coordinatesWithDistance.Add(new Coordinate() { xVal = pixel.xVal, yVal = pixel.yVal, Value = coordinate.Value }, distance);
            }

            var nearestCoordinates = coordinatesWithDistance.Where(c => c.Value == coordinatesWithDistance.Values.Min());
            if (nearestCoordinates.Count() > 1) pixel.Value = '.';
            else
            {
                var nearestCoordinate = nearestCoordinates.First();
                pixel = nearestCoordinate.Key;
                lock (lockVar)
                {
                    var coordinate = coordinates.Where(c => c.Value == nearestCoordinate.Key.Value).FirstOrDefault();
                    coordinate.Amount += 1;
                }
            }

            if (pixel.xVal == 0 || pixel.yVal == 0 || pixel.xVal == GridStride - 1 || pixel.yVal == GridStride - 1)
            {
                var coordinate = coordinates.Where(c => c.Value == pixel.Value).FirstOrDefault();
                if (coordinate != null) coordinate.IsInfinite = true;
            }
            return pixel;
        }

        private int GetManhattenDistance(Coordinate from, Coordinate to) => Math.Abs(from.xVal - to.xVal) + Math.Abs((from.yVal - to.yVal));

        public override string Part1()
        {
            var coordinates = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.Split(new[] { "," }, StringSplitOptions.None)).Select((d, e) => new GridCoordinate() { xVal = Convert.ToInt32(d[0].Trim()), yVal = Convert.ToInt32(d[1].Trim()), Value = (char)(e + 65) }).ToHashSet();
            //Create Grid
            GridStride = Test ? 10 : 400;
            var grid = new Coordinate[GridStride * GridStride];

            Parallel.For(0, GridStride, (x) =>
            {
                Parallel.For(0, GridStride, (y) =>
                {
                    var pixel = new Coordinate() { xVal = x, yVal = y };
                    pixel = GetNearestManhattenDistanceValue(pixel, coordinates);
                    grid[x + (y * GridStride)] = pixel;
                });
            });

            Print(grid);
            return coordinates.Where(x => !x.IsInfinite).Select(c => c.Amount).Max().ToString();
        }

        private void Print(Coordinate[] grid)
        {
            if (Test)
            {
                var sb = new StringBuilder();
                for (int y = 0; y < GridStride; y++)
                {
                    for (int x = 0; x < GridStride; x++)
                    {
                        sb.Append(grid[x + (y * GridStride)].Value);
                    }
                    sb.AppendLine();
                }
                Debug.WriteLine(sb.ToString());
            }
        }

        private object varSafe = new object();

        public override string Part2()
        {
            var coordinates = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.Split(new[] { "," }, StringSplitOptions.None)).Select((d, e) => new GridCoordinate() { xVal = Convert.ToInt32(d[0].Trim()), yVal = Convert.ToInt32(d[1].Trim()), Value = (char)(e + 65) }).ToHashSet();
            //Create Grid
            GridStride = Test ? 10 : 400;
            var grid = new Coordinate[GridStride * GridStride];

            Parallel.For(0, GridStride, (x) =>
            {
                Parallel.For(0, GridStride, (y) =>
                {
                    var pixel = new Coordinate() { xVal = x, yVal = y };
                    pixel = GeTotalManhattenDistanceValue(pixel, coordinates);
                    grid[x + (y * GridStride)] = pixel;
                });
            });

            Print(grid);
            return grid.Where(x => x.Value == '#').Count().ToString();
        }

        private Coordinate GeTotalManhattenDistanceValue(Coordinate pixel, HashSet<GridCoordinate> coordinates)
        {
            var totalDistanceToAllCoordinates = 0;
            foreach (var coordinate in coordinates) totalDistanceToAllCoordinates += GetManhattenDistance(pixel, coordinate);
            pixel.Value = totalDistanceToAllCoordinates < (Test ? 32 : 10000) ? '#' : '.';
            return pixel;
        }
    }
}

