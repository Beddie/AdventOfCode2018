using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Days
{
    public class Day25 : AdventBase
    {
        public Day25()
        {
            // Test = true;
            PuzzleInput = Test ? Resources.Day25Example : Resources.Day25;
            ID = 25;
            Name = "Day 25: Four-Dimensional Adventure";
        }

        public override string[] Solution()
        {
            return new string[] { "390", "" };
        }

        public override string Part1()
        {
            //PuzzleInput = @"0,0,0,0
            //             3,0,0,0
            //             0,3,0,0
            //             0,0,3,0
            //             0,0,0,3
            //             0,0,0,6
            //             9,0,0,0
            //6,0,0,0
            //            12,0,0,0";

            //PuzzleInput = @"-1,2,2,0
            //0,0,2,-2
            //0,0,0,-2
            //-1,2,0,0
            //-2,-2,-2,2
            //3,0,2,-1
            //-1,3,2,2
            //-1,0,-1,0
            //0,2,1,-2
            //3,0,0,0";

//            PuzzleInput = @"1,-1,0,1
//            2,0,-1,0
//3,2,-1,0
//0,0,3,1
//0,0,-1,-1
//2,3,-2,0
//-2,2,0,0
//2,-2,0,-1
//1,-1,0,-1
//3,2,0,2";

            //PuzzleInput = @"1,-1,-1,-2
            //-2,-2,0,1
            //0,2,1,3
            //-2,3,-2,1
            //0,2,3,-2
            //-1,-1,1,-2
            //0,-2,-1,0
            //-2,2,3,-1
            //1,2,2,0
            //-1,-2,0,-2";

            var constellationPoints = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.Split(',')).Select((e, index) => new ConstellationPoint(index, e[0], e[1], e[2], e[3])).ToList();

            List<List<ConstellationPoint>> constellations = new List<List<ConstellationPoint>>();
            var constellationPointsScrapList = new List<ConstellationPoint>();
            constellationPointsScrapList.AddRange(constellationPoints);

            //Initialize constellation
            var constellation = new List<ConstellationPoint>();
            constellation.Add(constellationPoints[0]);

            //Remove from scraplist by ID
            constellationPointsScrapList.RemoveAll(c => constellation.Select(d => d.Index).Contains(c.Index));

            var buildConstellation = true;
            while (buildConstellation)
            {
                var constellationGrewLarger = false;
                foreach (var constellationPointFromScrapList in constellationPointsScrapList)
                {
                    var constellationPoint = constellationPoints.First(c => c.Index == constellationPointFromScrapList.Index);
                    if (constellationPoint.BelongsToConstellation(constellation))
                    {
                        constellation.Add(constellationPoint);
                        constellationGrewLarger = true;
                    }
                }

                if (constellationGrewLarger)
                {
                    constellationPointsScrapList.RemoveAll(c => constellation.Select(d => d.Index).Contains(c.Index));
                }
                else {
                    constellationPointsScrapList.RemoveAll(c => constellation.Select(d => d.Index).Contains(c.Index));
                    constellations.Add(constellation);
                    if (constellationPointsScrapList.Count() == 0) { buildConstellation = false; break; }
                    constellation = new List<ConstellationPoint>() { constellationPoints.First(c => c.Index == constellationPointsScrapList.First().Index) };
                    constellationPointsScrapList.RemoveAll(c => constellation.Select(d => d.Index).Contains(c.Index));
                }
            }

            var amountConstellations = constellations.Count();
            return "";
        }

        private class ConstellationPoint
        {
            public ConstellationPoint(int index, string x, string y, string z, string t)
            {
                Index = index;
                X = Convert.ToInt32(x);
                Y = Convert.ToInt32(y);
                Z = Convert.ToInt32(z);
                T = Convert.ToInt32(t);
            }
            public int Index { get; set; }
            public int MHDistancetoReference(ConstellationPoint cp) => Math.Abs(X - cp.X) + Math.Abs(Y - cp.Y) + Math.Abs(Z - cp.Z) + Math.Abs(T - cp.T);
            public bool BelongsToConstellation(List<ConstellationPoint> constellation) => constellation.Any(c => MHDistancetoReference(c) <= 3);

            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public int T { get; set; }
        }

        public override string Part2()
        {
            return "";
        }
    }
}

