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
            return new string[] { "309",
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
        public override string Part1()
        {
            var digitRegex = new Regex(@"-?\d+");
            var nanoBots = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => digitRegex.Matches(c)).Select(c => new Nanobot() { X = Convert.ToInt32(c[0].Value), Y = Convert.ToInt32(c[1].Value), Z = Convert.ToInt32(c[2].Value), Radius = Convert.ToInt32(c[3].Value) }).ToList();


            var masterNanoBot = nanoBots.First(c => c.Radius == nanoBots.Max(d => d.Radius));

            // var bb = nanoBots.Count(c => c.Z < 0);
            var countNanoBots = nanoBots.Where(c => IsManhattenValidRange(c, masterNanoBot)).Count();

            return countNanoBots.ToString();
        }


        private bool Intersect(Nanobot nanobotFrom, Nanobot nanobotTo)
        {
            // we are using multiplications because it's faster than calling Math.pow
            var distance = Math.Sqrt((nanobotFrom.X - nanobotTo.X) * (nanobotFrom.X - nanobotTo.X) +
                                     (nanobotFrom.Y - nanobotTo.Y) * (nanobotFrom.Y - nanobotTo.Y) +
                                     (nanobotFrom.Z - nanobotTo.Z) * (nanobotFrom.Z - nanobotTo.Z));
            return distance < (nanobotFrom.Radius + nanobotFrom.Radius);
        }

        private bool IsManhattenValidRange(Nanobot from, Nanobot reference) => (Math.Abs(from.X - reference.X) + Math.Abs(from.Y - reference.Y) + Math.Abs(from.Z - reference.Z)) <= reference.Radius;

        private bool IsManhattenValidRange(Nanobot from, long x, long y, long z) => (Math.Abs(from.X - x) + Math.Abs(from.Y - y) + Math.Abs(from.Z - z)) <= from.Radius;

        private long IsManhattenRangeFromZero(long x, long y, long z) => (Math.Abs(0 - x) + Math.Abs(0 - y) + Math.Abs(0 - z));

        private object varLock = new object();
        public override string Part2()
        {
            //var sp = new Sphere();
            var digitRegex = new Regex(@"-?\d+");
            var nanoBots = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => digitRegex.Matches(c)).Select(c => new Nanobot() { X = Convert.ToInt32(c[0].Value), Y = Convert.ToInt32(c[1].Value), Z = Convert.ToInt32(c[2].Value), Radius = Convert.ToInt32(c[3].Value) }).ToHashSet();

            var stride = Test ? (int)200 : 200; // (int)100000000;
            var maxCOunt = 0;
            var valX = 0;
            var valY = 0;
            var valZ = 0;


            var maxList = new HashSet<(long, int, int, int)>();
            var jumpAmount = 1000;

            //var minOctaHedron = 49505500;
            //var maxOctaHedron = 49506400;
            var vX = 49506210;


            var minOctaHedron = 49500000;
            var maxOctaHedron = 49510000;
            //var vX = 49506210;


            //var middleOctaHedron = 49508000;

            //for (int jump = jumpAmount * jumpAmount; jump < 100 * jumpAmount * jumpAmount; jump = jump + 10000)

            var fromValue = 100000000;
            var minX = 0;
            var minY = 0;
            var minZ = 0;

            var minXX = 0;
            var minYY = 0;
            var minZZ = 0;

            var maxCount = 630;

            for (int shrimpValue = 1; shrimpValue < 10; shrimpValue++)
            {
                for (int valfromX = 0; valfromX < fromValue; valfromX += 1000000 / shrimpValue)
                {
                    for (int valfromY = 0; valfromY < fromValue; valfromY += 1000000 / shrimpValue)
                    {
                        for (int valfromZ = 0; valfromZ < fromValue; valfromZ += 1000000 / shrimpValue)
                        {
                            var count = 0;
                            foreach (var bot in nanoBots) count += IsManhattenValidRange(bot, valfromX, valfromY, valfromZ) ? 1 : 0;

                            if (count > maxCount)
                            {
                                maxCount = count;

                            }
                            else if (count == maxCount)
                            {
                                if (Math.Abs(valfromX) < minX) minX = valfromX;
                                if (Math.Abs(valfromY) < minY) minY = valfromY;
                                if (Math.Abs(valfromZ) < minZ) minZ = valfromZ;

                            }
                        }
                    }
                }
            }
            var stop = 1;
            //for (int jump = minOctaHedron; jump < maxOctaHedron; jump += 10)
            //{
            //    var count = 0;
            //    foreach (var bot in nanoBots)
            //    {
            //        count += IsManhattenValidRange(bot, vX, jump, jump) ? 1 : 0;
            //    }
            //    // var manhattenDistance = IsManhattenRangeFromZero(jump, jump, jump);
            //    maxList.Add((count, vX, jump, jump));
            //}
            //var meanXmax = nanoBots.Average(c=> c.X);
            //var meanYmax = nanoBots.Average(c => c.Y);
            //var meanZmax = maxList.OrderByDescending(c => c.Item1);

            //foreach (var nanoBot in nanoBots)
            //{
            //    nanoBot.IntersectingNanoBots = nanoBots.Where(c => c != nanoBot && Intersect(nanoBot, c)).ToList();
            //}

            //var most = nanoBots.Max(c => c.IntersectingNanoBots.Count());

            // var maxList = new HashSet<(int, int, int)>();

            var smallestManhattenDistance = (long)99999999;
            Parallel.For(-stride, stride, (y) =>
            {
                //for (int y = -stride; y < stride; y++)
                // {
                Parallel.For(-stride, stride, (x) =>
             {
                 // for (int x = -stride; x < stride; x++)
                 //{
                 // Parallel.For(-stride, stride, (z) =>
                 //{
                 for (int z = -stride; z < stride; z++)
                 {

                     var count = 0;

                     // Parallel.ForEach(nanoBots, (nanoBot) =>
                     // {
                     foreach (var nanoBot in nanoBots)
                     {
                         count += IsManhattenValidRange(nanoBot, x, y, z) ? 1 : 0;


                     };
                     lock (varLock)
                     {


                         if (count > maxCOunt)
                         {
                             maxCOunt = count;
                             smallestManhattenDistance = IsManhattenRangeFromZero(x, y, z);
                             valX = x;
                             valY = y;
                             valZ = z;
                             //maxList.Add((x, y, z));
                         }
                         else if (count == maxCOunt && smallestManhattenDistance > IsManhattenRangeFromZero(x, y, z))
                         {
                             smallestManhattenDistance = IsManhattenRangeFromZero(x, y, z);
                             valX = x;
                             valY = y;
                             valZ = z;
                             //maxList.Add((x, y, z));
                         }
                     }
                 };
             });

            });
            return maxCOunt.ToString();
        }
    }
}

