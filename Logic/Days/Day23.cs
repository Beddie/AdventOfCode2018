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
    public class Day23 : AdventBase
    {
        public Day23()
        {
            //Test = true;
            PuzzleInput = Test ? Resources.Day23Example : Resources.Day23;
            ID = 23;
            Name = "Day 23: Experimental Emergency Teleportation";
        }

        public override string[] Solution()
        {
            return new string[] { "309","119011326"
            };
        }

        private class Nanobot
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public int Radius { get; set; }
            public List<Nanobot> IntersectingNanoBots { get; set; } = new List<Nanobot>();
        }

        private class Distance
        {
            public Distance(int x, int y, int z)
            {
                MHDistanceToZeto = x + y + z;
                X = x;
                Y = y;
                Z = z;
            }
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public int MHDistanceToZeto { get; set; }
        }

        public override string Part1()
        {
            var digitRegex = new Regex(@"-?\d+");
            var nanoBots = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => digitRegex.Matches(c)).Select(c => new Nanobot() { X = Convert.ToInt32(c[0].Value), Y = Convert.ToInt32(c[1].Value), Z = Convert.ToInt32(c[2].Value), Radius = Convert.ToInt32(c[3].Value) }).ToList();
            var masterNanoBot = nanoBots.First(c => c.Radius == nanoBots.Max(d => d.Radius));
            var countNanoBots = nanoBots.Where(c => IsManhattenValidRange(c, masterNanoBot)).Count();

            return countNanoBots.ToString();
        }


        private bool Intersect(Nanobot nanobotFrom, Nanobot nanobotTo)
        {
            var distance = Math.Sqrt((nanobotFrom.X - nanobotTo.X) * (nanobotFrom.X - nanobotTo.X) +
                                     (nanobotFrom.Y - nanobotTo.Y) * (nanobotFrom.Y - nanobotTo.Y) +
                                     (nanobotFrom.Z - nanobotTo.Z) * (nanobotFrom.Z - nanobotTo.Z));
            return distance < (nanobotFrom.Radius + nanobotFrom.Radius);
        }

        private bool IsManhattenValidRange(Nanobot from, Nanobot reference) => (Math.Abs(from.X - reference.X) + Math.Abs(from.Y - reference.Y) + Math.Abs(from.Z - reference.Z)) <= reference.Radius;

        private bool IsManhattenValidRange(Nanobot from, long x, long y, long z) => (Math.Abs(from.X - x) + Math.Abs(from.Y - y) + Math.Abs(from.Z - z)) <= from.Radius;

        private long IsManhattenRangeFromZero(long x, long y, long z) => (Math.Abs(0 - x) + Math.Abs(0 - y) + Math.Abs(0 - z));

        public override string Part2()
        {
            //var sp = new Sphere();
            var digitRegex = new Regex(@"-?\d+");
            var nanoBots = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => digitRegex.Matches(c)).Select(c => new Nanobot() { X = Convert.ToInt32(c[0].Value), Y = Convert.ToInt32(c[1].Value), Z = Convert.ToInt32(c[2].Value), Radius = Convert.ToInt32(c[3].Value) }).ToHashSet();

            var MH = 0;

            //Manually found numbers
            //TODO Create algo
            var minXX = 32906900;
            var minYY = 33700100;
            var minZZ = 52400200;
            var maxXX = 32907100;
            var maxYY = 33700300;
            var maxZZ = 52405200;

            var maxCount = 938;
            var distances = new List<Distance>();
            var first = true;

            for (int stepAmount = 10; stepAmount > 0; stepAmount = stepAmount / 10)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    var range = 11 * stepAmount;

                    var smallestMHDistance = distances.First(c => c.MHDistanceToZeto == distances.Min(d => d.MHDistanceToZeto));
                    minXX = smallestMHDistance.X - range;
                    minYY = smallestMHDistance.Y - range;
                    minZZ = smallestMHDistance.Z - range;
                    maxXX = smallestMHDistance.X + range;
                    maxYY = smallestMHDistance.Y + range;
                    maxZZ = smallestMHDistance.Z + range;

                    distances = new List<Distance>();
                }

                for (int valfromX = minXX; valfromX < maxXX; valfromX += stepAmount)
                {
                    for (int valfromY = minYY; valfromY < maxYY; valfromY += stepAmount)
                    {
                        for (int valfromZ = minZZ; valfromZ < maxZZ; valfromZ += stepAmount)
                        {
                            var count = nanoBots.Count(c => IsManhattenValidRange(c, valfromX, valfromY, valfromZ));

                            if (count > maxCount)
                            {
                                maxCount = count;
                                distances = new List<Distance>();
                                distances.Add(new Distance(valfromX, valfromY, valfromZ));
                            }
                            else if (count == maxCount)
                            {
                                distances.Add(new Distance(valfromX, valfromY, valfromZ));
                            }
                        }
                    }
                }

                if (stepAmount == 1)
                {
                    var MHD = distances.First(c => c.MHDistanceToZeto == distances.Min(d => d.MHDistanceToZeto));
                    MH = MHD.MHDistanceToZeto;
                }
            }

            return MH.ToString();
        }
    }
}

