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
    class Day3 : AdventBase, AdventInterface
    {
        public Day3()
        {
            PuzzleInput = Test? Resources.Day3Example : Resources.Day3;
        }

        public string[] Solution()
        {
            return new string[] {
                "110827",
                "116"
            };
        }

        /// <summary>
        /// Fabric class, will be used by an Elf
        /// </summary>
        private class Fabric
        {
            public int ID { get; set; }
            public int LeftMargin { get; set; }
            public int TopMargin { get; set; }
            public int FabricSideX { get; set; }
            public int FabricSideY { get; set; }
        }

        /// <summary>
        /// Square where area claimes will be made by Elves!
        /// </summary>
        private class BigFabricSquare
        {
            public BigFabricSquare(int stride)
            {
                Stride = stride;
                SquareValues = new int?[stride * stride];
            }
            public int Stride { get; set; }
            public int?[] SquareValues { get; set; }
            public HashSet<int> OverlappingIDs { get; set; } = new HashSet<int>();

            private int? GetCurrentSquareValue(int x, int y) {
                return SquareValues[x + y * Stride];
            }

            public int CountOverlappingFabricSquares() {
                return SquareValues.Where(c => c == 0).Count();
            }

            /// <summary>
            /// Claim will be made on the big square
            /// </summary>
            /// <param name="fabric"></param>
            public void Stitch(Fabric fabric) {
                for (int x = 0; x < fabric.FabricSideX; x++)
                {
                    for (int y = 0; y < fabric.FabricSideY; y++)
                    {
                        var currentSquareValue = GetCurrentSquareValue(fabric.LeftMargin + x, fabric.TopMargin + y);
                        if (currentSquareValue.HasValue)
                        {
                           if (!OverlappingIDs.Contains(fabric.ID)) OverlappingIDs.Add(fabric.ID);
                           if (!OverlappingIDs.Contains(currentSquareValue.Value)) OverlappingIDs.Add(currentSquareValue.Value);
                           currentSquareValue = 0;
                        }
                        else {
                            currentSquareValue = fabric.ID;
                        }
                        SquareValues[fabric.LeftMargin + x + ((fabric.TopMargin + y) * Stride)] = currentSquareValue;
                    }
                }
            }

            public void Print()
            {
                if (Test)
                {
                    var printstring = new StringBuilder();
                    for (int y = 0; y < Stride; y++)
                    {
                        for (int x = 0; x < Stride; x++)
                        {
                            var pixel = SquareValues[x + y * Stride];
                            printstring.Append(string.Format("{0}", !pixel.HasValue ? "  .  " : pixel.Value == 0 ? "  X  " : $"{pixel}".PadRight(5, ' ')));
                        }
                        printstring.AppendLine("");
                    }
                    printstring.AppendLine();
                    Debug.Print(printstring.ToString());
                }
            }
        }

        public string Part1()
        {
            var fabrics = new Regex("[@:#]").Replace(PuzzleInput, string.Empty).Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.Split(new[] { "x", "," }, StringSplitOptions.None).Select(f => Convert.ToInt32(f)).ToArray()).ToArray()).Select(g=> new Fabric() { ID = g[0][0], LeftMargin = g[1][0], TopMargin = g[1][1],  FabricSideX = g[2][0], FabricSideY = g[2][1]  }).ToHashSet();
            var bigFabricSquare = new BigFabricSquare(Test ? 15 : 1000);
            
            //Stitch fabric to big Square
            foreach (var fabric in fabrics) bigFabricSquare.Stitch(fabric);

            //Print big square to debug window during test
            bigFabricSquare.Print();
           return bigFabricSquare.CountOverlappingFabricSquares().ToString();
        }

        public string Part2()
        {
            var fabrics = new Regex("[@:#]").Replace(PuzzleInput, string.Empty).Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.Split(new[] { "x", "," }, StringSplitOptions.None).Select(f => Convert.ToInt32(f)).ToArray()).ToArray()).Select(g => new Fabric() { ID = g[0][0], LeftMargin = g[1][0], TopMargin = g[1][1], FabricSideX = g[2][0], FabricSideY = g[2][1] }).ToHashSet();
            var bigFabricSquare = new BigFabricSquare(Test ? 15 : 1000);

            //Stitch fabric to big Square
            foreach (var fabric in fabrics) bigFabricSquare.Stitch(fabric);

            //Print big square to debug window during test
            bigFabricSquare.Print();

            return fabrics.Select(c => c.ID).Except(bigFabricSquare.OverlappingIDs).First().ToString();
        }
    }
}

