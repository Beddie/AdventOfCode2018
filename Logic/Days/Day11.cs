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
            Test = true;
            //PuzzleInput = Test ? 300 : 3628;

            ID = 11;
            Name = "Day 11: Chronal Charge";
        }

        public override string[] Solution()
        {
            return new string[] {
            };
        }

        public override string Part1()
        {
            var gridSerialNumber = Test ? 18 : 3628;
            var stride = Test ? 300 : 300;
            var grid = new int[stride * stride];

            for (int y = 1; y < stride; y++)
            {
                for (int x = 1; x < stride; x++)
                {
                    var rackID = x + 10;
                    int? powerLevel = (rackID * y) + gridSerialNumber;
                    powerLevel = powerLevel * rackID;
                    powerLevel = (powerLevel.Value / 100) % 10; // powerLevel.ToString().Reverse().Skip(2).FirstOrDefault();
                    if (powerLevel.HasValue)
                    {
                        powerLevel -= 5;
                    }
                    else
                    {
                        powerLevel = 0;
                    }
                    grid[x + (y * stride)] = powerLevel.Value;
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
            for (int y = 1; y < stride - 2; y++)
            {
                for (int x = 1; x < stride - 2; x++)
                {
                    var total = 0;

                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            total += grid[(x + i) + ((y+ j) * stride)];
                        }
                    }
                    //total += grid[x + (y * stride)];
                    //total += grid[x + 1 + (y * stride)];
                    //total += grid[x + 2 + (y * stride)];
                    //total += grid[x + ((y + 1) * stride)];
                    //total += grid[x + 1 + ((y + 1) * stride)];
                    //total += grid[x + 2 + ((y + 1) * stride)];
                    //total += grid[x + ((y + 2) * stride)];
                    //total += grid[x + 1 + ((y + 2) * stride)];
                    //total += grid[x + 2 + ((y + 2) * stride)];

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
                    int? powerLevel = (rackID * y) + gridSerialNumber;
                    powerLevel = powerLevel * rackID;
                    powerLevel = (powerLevel.Value / 100) % 10; // powerLevel.ToString().Reverse().Skip(2).FirstOrDefault();
                    if (powerLevel.HasValue)
                    {
                        powerLevel -= 5;
                    }
                    else
                    {
                        powerLevel = 0;
                    }
                    grid[x + (y * stride)] = powerLevel.Value;
                }
            }

            Print(grid, stride);
            var XY = GetMaxSquare(grid, stride);
            return XY;
        }

        private string GetMaxSquare(int[] grid, int stride)
        {
            var maxValue = 0;
            var TopLeftx = 0;
            var TopLefty = 0;

            for (int size = 1; size < stride; size++)
            {
                for (int y = 1; y < stride - (size -1); y++)
                {
                    for (int x = 1; x < stride - (size -1); x++)
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
                        //total += grid[x + (y * stride)];
                        //total += grid[x + 1 + (y * stride)];
                        //total += grid[x + 2 + (y * stride)];
                        //total += grid[x + ((y + 1) * stride)];
                        //total += grid[x + 1 + ((y + 1) * stride)];
                        //total += grid[x + 2 + ((y + 1) * stride)];
                        //total += grid[x + ((y + 2) * stride)];
                        //total += grid[x + 1 + ((y + 2) * stride)];
                        //total += grid[x + 2 + ((y + 2) * stride)];


                    }
                }
            }
            return $"{TopLeftx},{TopLefty}";

        }
    }
}

