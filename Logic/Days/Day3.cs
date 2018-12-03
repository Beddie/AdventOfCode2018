using Logic.Interface;
using Logic.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        private class Fabric
        {
            public int ID { get; set; }
            public int LeftMargin { get; set; }
            public int TopMargin { get; set; }
            public int FabricSideX { get; set; }
            public int FabricSideY { get; set; }
            public bool HasOverlap { get; set; }
        }

        private class FabricSquare
        {
            public int Stride { get; set; }
            public int[] GridValues { get; set; }
            private int GridValue(int x, int y) {
                return GridValues[x + y * Stride];
            }
            private int SquareGridValue(int x, int y) {
                return GridValue(x, y);
            }

            public int CountOverlappingFabricSquares() {
                return GridValues.Where(c => c == -1).Count();
            }

            public List<int> overlappingIDs { get; set; } = new List<int>();
            public void Stitch(Fabric fabric) {
                for (int x = 0; x < fabric.FabricSideX; x++)
                {
                    for (int y = 0; y < fabric.FabricSideY; y++)
                    {
                        var currentSquareValue = SquareGridValue(fabric.LeftMargin + x, fabric.TopMargin + y);
                        if (currentSquareValue != 0)
                        {
                            overlappingIDs.Add(fabric.ID);
                            overlappingIDs.Add(currentSquareValue);
                            currentSquareValue = -1;
                        }
                        else {
                            currentSquareValue = fabric.ID;
                        }
                        GridValues[fabric.LeftMargin + x + ((fabric.TopMargin + y) * Stride)] = currentSquareValue;
                    }
                }
            }

            public void Print()
            {
                var printstring = new StringBuilder();
                for (int y = 0; y < Stride; y++)
                {
                    for (int x = 0; x < Stride; x++)
                    {
                        var pixel = GridValues[x + y * Stride];
                        printstring.Append(string.Format("{0}", pixel == 0 ? "  .  " : pixel == -1 ? "  X  " : $"{pixel}".PadRight(5, ' ')));
                    }
                    printstring.AppendLine("");
                }
                printstring.AppendLine();
                Debug.Print(printstring.ToString());
            }

        }

        public string Part1()
        {
            var stride = Test ? 15 : 1000;
            //var fabricSquare = new int[stride * stride];
            var fabricSquare = new FabricSquare() { Stride = stride, GridValues = new int[stride * stride] };

            var fabrics = PuzzleInput.Replace("@", "").Replace(":", "").Replace("#", "").Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.Split(new[] { " " }, StringSplitOptions.None).Where(e => !string.IsNullOrEmpty(e)).ToArray().Select(d => d.Split(new[] { "x", "," }, StringSplitOptions.None).ToArray().Select(f => Convert.ToInt32(f)).ToArray()).ToArray()).Select(g=> new Fabric() { ID = g[0][0], LeftMargin = g[1][0], TopMargin = g[1][1],  FabricSideX = g[2][0], FabricSideY = g[2][1]  }).ToHashSet();

            foreach (var fabric in fabrics)
            {
                fabricSquare.Stitch(fabric);
            }
            fabricSquare.Print();

            return fabricSquare.CountOverlappingFabricSquares().ToString();
        }

        public string Part2()
        {
            var stride = Test ? 15 : 1000;
            //var fabricSquare = new int[stride * stride];
            var fabricSquare = new FabricSquare() { Stride = stride, GridValues = new int[stride * stride] };

            var fabrics = PuzzleInput.Replace("@", "").Replace(":", "").Replace("#", "").Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.Split(new[] { " " }, StringSplitOptions.None).Where(e => !string.IsNullOrEmpty(e)).ToArray().Select(d => d.Split(new[] { "x", "," }, StringSplitOptions.None).ToArray().Select(f => Convert.ToInt32(f)).ToArray()).ToArray()).Select(g => new Fabric() { ID = g[0][0], LeftMargin = g[1][0], TopMargin = g[1][1], FabricSideX = g[2][0], FabricSideY = g[2][1] }).ToHashSet();

            foreach (var fabric in fabrics)
            {
                fabricSquare.Stitch(fabric);
            }
            fabricSquare.Print();

            return fabrics.Select(c => c.ID).Except(fabricSquare.overlappingIDs).First().ToString();
        }
    }
}

