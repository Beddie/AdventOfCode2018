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
    public class Day11 : AdventBase
    {
        public Day11()
        {
            //Test = true;
            ID = 11;
            Name = "Day 11: Chronal Charge";
        }

        public override string[] Solution()
        {
            return new string[] {
                "216,12"
                ,"236,175,11"
            };
        }

        public override string Part1()
        {
            var gridSerialNumber = Test ? 18 : 3628;
            var stride = 300;
            var grid = new int[stride * stride];

            for (int y = 1; y < stride; y++)
            {
                for (int x = 1; x < stride; x++)
                {
                    var rackID = x + 10;
                    var powerLevel = ((rackID * y) + gridSerialNumber) * rackID;
                    grid[x + (y * stride)] = (powerLevel > 100) ? ((powerLevel / 100) % 10) - 5 : 0;
                }
            }

            var XY = GetMax33Square(grid, stride);
            Print(grid, stride);
            return XY;
        }

        private string GetMax33Square(int[] grid, int stride)
        {
            var maxValue = 0;
            var TopLeftx = 0;
            var TopLefty = 0;
            var size = 3;
            for (int y = 1; y < stride - (size - 1); y++)
            {
                for (int x = 1; x < stride - (size - 1); x++)
                {
                    var total = 0;
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            total += grid[(x + i) + ((y + j) * stride)];
                        }
                    }
                    if (total > maxValue)
                    {
                        maxValue = total;
                        TopLeftx = x;
                        TopLefty = y;
                    }
                }
            }
            return $"{TopLeftx},{TopLefty}";
        }

        public override string Part2()
        {
            var gridSerialNumber = Test ? 18 : 3628;
            var stride = Test ? 300 : 300;
            var grid = new int[stride * stride];

            for (int y = 1; y < stride; y++)
            {
                for (int x = 1; x < stride; x++)
                {
                    var rackID = x + 10;
                    var powerLevel = ((rackID * y) + gridSerialNumber) * rackID;
                    grid[x + (y * stride)] = (powerLevel > 100) ? ((powerLevel / 100) % 10) - 5 : 0;
                }
            }
            Print(grid, stride);
            return GetMaxSquare(grid, stride);
        }

        private object lockVar = new object();

        private string GetMaxSquare(int[] grid, int stride)
        {
            var maxValue = 0;
            var TopLeftx = 0;
            var TopLefty = 0;
            var gridSize = 0;

            Parallel.For(1, stride, (y) =>
            {
                Parallel.For(1, stride, (x) =>
                {
                    var maxXY = x > y ? x : y;
                    var total = 0;
                    for (int size = 0; size < stride - maxXY; size++)
                    {
                        for (int edgeY = 0; edgeY < size + 1; edgeY++)
                        {
                            total += grid[(x + size) + ((y + edgeY) * stride)];
                        };
                        for (int edgeX = 0; edgeX < size; edgeX++)
                        {
                            total += grid[(x + edgeX) + ((y + size) * stride)];
                        };

                        if (total > 0)
                        {
                            lock (lockVar)
                            {
                                if (total > maxValue)
                                {
                                    maxValue = total;
                                    TopLeftx = x;
                                    TopLefty = y;
                                    gridSize = size + 1;
                                }
                            }
                        }
                    };
                });
            });
            return $"{TopLeftx},{TopLefty},{gridSize}";

        }

        private void Print(int[] grid, int stride)
        {
            if (Test)
            {
                var sb = new StringBuilder();
                for (int y = 0; y < stride; y++)
                {
                    for (int x = 0; x < stride; x++)
                    {
                        sb.Append($"  { grid[x + (y * stride)]}  ");
                    }
                    sb.AppendLine();
                }
                Debug.WriteLine(sb.ToString());
            }
        }
    }
}

