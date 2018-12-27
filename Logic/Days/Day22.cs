using Logic.Service.Pathfinder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Logic.Days
{
    public class Day22 : AdventBase
    {
        public Day22()
        {
            Test = true;
            // PuzzleInput = Test ? Resources.Day22Example : Resources.Day22;
            ID = 22;
            Name = "Day 22: Mode Maze";
        }

        public override string[] Solution()
        {
            return new string[] {
            };
        }

        public override string Part1()
        {
            var puzzle = (Test) ? new int[] { 510, 10, 10 } : new int[] { 11820, 7, 782 };
            var depth = puzzle[0];
            var targetX = puzzle[1];
            var targetY = puzzle[2];

            var maxStride = (targetX > targetY) ? targetX + 2: targetY + 2;
            var strideX = maxStride; //  targetX + 10;
            var strideY = maxStride; // targetY + 10;

            var geologicGrid = new long[strideX, strideY];
            var erosionGrid = new long[strideX, strideY];
            var finalGrid = new long[strideX , strideY];


            for (int squareMaxIndex = 0; squareMaxIndex < strideX; squareMaxIndex++)
            {
                long val;
                //fill all X
                for (int x = 0; x < squareMaxIndex; x++)
                {
                    val = GetGeologicIndex(geologicGrid, x, squareMaxIndex, erosionGrid, targetX, targetY);
                    erosionGrid[x, squareMaxIndex] = (val + depth) % 20183;
                    geologicGrid[x, squareMaxIndex] = val;
                }
                for (int y = 0; y < squareMaxIndex; y++)
                {
                    val = GetGeologicIndex(geologicGrid, squareMaxIndex, y, erosionGrid, targetX, targetY);
                    erosionGrid[squareMaxIndex, y] = (val + depth) % 20183;
                    geologicGrid[squareMaxIndex, y] = val;
                }

                val = GetGeologicIndex(geologicGrid, squareMaxIndex, squareMaxIndex, erosionGrid, targetX, targetY);
                erosionGrid[squareMaxIndex, squareMaxIndex] = (val + depth) % 20183;
                geologicGrid[squareMaxIndex, squareMaxIndex] = val;
            };

            for (int y = 0; y < erosionGrid.GetUpperBound(1); y++)
            {
                for (int x = 0; x < erosionGrid.GetUpperBound(0); x++)
                {
                    var erosionLevel = erosionGrid[x, y];
                    finalGrid[x, y] = GetErosionType(erosionLevel);
                    //grid[x,y] = GetErosionLevel(geologicalIndex, depth);

                }
            }

            Print(finalGrid);


            //           Before you go in, you should determine the risk level of the area.For the the rectangle that 
            //has a top-left corner of region 0,0 and a bottom - right corner of the region containing the target, 
            //add up the risk level of each individual region: 0 for rocky regions, 1 for wet regions, and 2 for narrow regions.
            //In the cave system above, because the mouth is at 0, 0 and the target is at 10, 10, 
            //adding up the risk level of all regions with an X coordinate from 0 to 10 and a Y coordinate from 0 to 10, this total is 114.
            var total = 0;
            for (int y = 0; y <= targetY; y++)
            {
                for (int x = 0; x <= targetX; x++)
                {
                    var val = finalGrid[x, y];
                    switch (val)
                    {
                        case '.':
                            break;
                        case '=':
                            total++;
                            break;
                        case '|':
                            total += 2;
                            break;
                        default:
                            break;
                            //throw new Exception("non existing");
                    }

                }
            }
            return total.ToString();
        }

        private long GetErosionType(long erosionLevel)
        {
            switch (erosionLevel % 3)
            {
                case 0:
                    return '.';
                case 1:
                    return '=';
                case 2:
                    return '|';
                default:
                    return 0;
            }
        }

        private long GetGeologicIndex(long[,] geologicGrid, int x, int y, long[,] erosionGrid, int targetX, int targetY)
        {
            if (x == 0 && y == 0)
            {
                return 0;
            }
            else if (x == targetX && y == targetY) {
                return 0;
            }
            else if (y == 0)
            {
                return (long)(x * 16807);
            }
            else if (x == 0)
            {
                return (long)(y * 48271);
            }
            else
            {
                var a = erosionGrid[x - 1, y];// GetGeologicIndex(grid, x - 1, y);
                var b = erosionGrid[x, y - 1]; //  GetGeologicIndex(grid, x, y - 1);

                if (a * b == 0)
                {
                    var t = 2;
                }
                return a * b;

            }
            //The region at 0,0(the mouth of the cave) has a geologic index of 0.
            //The region at the coordinates of the target has a geologic index of 0.
            //If the region's Y coordinate is 0, the geologic index is its X coordinate times 16807.
            //If the region's X coordinate is 0, the geologic index is its Y coordinate times 48271.
            //Otherwise, the region's geologic index is the result of multiplying the erosion levels of the regions at X-1,Y and X,Y-1.
        }

        private void Print(long[,] grid, int firstX = 0, int lastX = 0, int firstY = 0, int lastY = 0)
        {
            var sb = new StringBuilder();
            for (int y = firstY; y < (lastY == 0 ? grid.GetLength(1) : lastY); y++)
            {
                for (int x = firstX; x < (lastX == 0 ? grid.GetLength(0) : lastX); x++)
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

            //int[,] graph =  {
            //             { 0, 6, 0, 0, 0, 0, 0, 9, 0 },
            //             { 6, 0, 9, 0, 0, 0, 0, 11, 0 },
            //             { 0, 9, 0, 5, 0, 6, 0, 0, 2 },
            //             { 0, 0, 5, 0, 9, 16, 0, 0, 0 },
            //             { 0, 0, 0, 9, 0, 10, 0, 0, 0 },
            //             { 0, 0, 6, 0, 10, 0, 2, 0, 0 },
            //             { 0, 0, 0, 16, 0, 2, 0, 1, 6 },
            //             { 9, 11, 0, 0, 0, 0, 1, 0, 5 },
            //             { 0, 0, 2, 0, 0, 0, 6, 5, 0 }
            //                };

            //DijkstraAlgo(graph, 0, 9);


            var puzzle = (Test) ? new int[] { 510, 10, 10 } : new int[] { 11820, 7, 782 };
            var depth = puzzle[0];
            var targetX = puzzle[1];
            var targetY = puzzle[2];

            var maxStride = (targetX > targetY) ? targetX + 10 : targetY + 10;
            var strideX = maxStride; //  targetX + 10;
            var strideY = maxStride; // targetY + 10;

            var geologicGrid = new long[strideX, strideY];
            var erosionGrid = new long[strideX, strideY];
            var finalGrid = new long[strideX, strideY];


            for (int squareMaxIndex = 0; squareMaxIndex < strideX; squareMaxIndex++)
            {
                long val;
                //fill all X
                for (int x = 0; x < squareMaxIndex; x++)
                {
                    val = GetGeologicIndex(geologicGrid, x, squareMaxIndex, erosionGrid, targetX, targetY);
                    erosionGrid[x, squareMaxIndex] = (val + depth) % 20183;
                    geologicGrid[x, squareMaxIndex] = val;
                }
                for (int y = 0; y < squareMaxIndex; y++)
                {
                    val = GetGeologicIndex(geologicGrid, squareMaxIndex, y, erosionGrid, targetX, targetY);
                    erosionGrid[squareMaxIndex, y] = (val + depth) % 20183;
                    geologicGrid[squareMaxIndex, y] = val;
                }

                val = GetGeologicIndex(geologicGrid, squareMaxIndex, squareMaxIndex, erosionGrid, targetX, targetY);
                erosionGrid[squareMaxIndex, squareMaxIndex] = (val + depth) % 20183;
                geologicGrid[squareMaxIndex, squareMaxIndex] = val;
            };

            for (int y = 0; y < erosionGrid.GetUpperBound(1); y++)
            {
                for (int x = 0; x < erosionGrid.GetUpperBound(0); x++)
                {
                    var erosionLevel = erosionGrid[x, y];
                    finalGrid[x, y] = GetErosionType(erosionLevel);
                }
            }

            var pathFinderGrid = new byte[strideX, strideY];


            for (int y = 0; y < strideY; y++)
            {
                for (int x = 0; x < strideX; x++)
                {
                    pathFinderGrid[x,y] = (byte)1;
                }
            }

            var fastestRoute = new PathFinderDay22(pathFinderGrid, finalGrid);
            var path = fastestRoute.FindPath(new System.Drawing.Point(0, 0), new System.Drawing.Point(targetX, targetY));

            var totalCost = AddPathToGridAndCountSwitches(path,finalGrid) * 7 + path.Count -1 ;
            Print(finalGrid);
            return "";
        }

        private static int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
        {
            int min = int.MaxValue;
            int minIndex = 0;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (shortestPathTreeSet[v] == false && distance[v] <= min)
                {
                    min = distance[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }

        private static void Print(int[] distance, int verticesCount)
        {
            Debug.WriteLine("Vertex    Distance from source");

            for (int i = 0; i < verticesCount; ++i)
                Debug.WriteLine("{0}\t  {1}", i, distance[i]);
        }

        public static void DijkstraAlgo(int[,] graph, int source, int verticesCount)
        {
            int[] distance = new int[verticesCount];
            bool[] shortestPathTreeSet = new bool[verticesCount];

            for (int i = 0; i < verticesCount; ++i)
            {
                distance[i] = int.MaxValue;
                shortestPathTreeSet[i] = false;
            }

            distance[source] = 0;

            for (int count = 0; count < verticesCount - 1; ++count)
            {
                int u = MinimumDistance(distance, shortestPathTreeSet, verticesCount);
                shortestPathTreeSet[u] = true;

                for (int v = 0; v < verticesCount; ++v)
                    if (!shortestPathTreeSet[v] && Convert.ToBoolean(graph[u, v]) && distance[u] != int.MaxValue && distance[u] + graph[u, v] < distance[v])
                        distance[v] = distance[u] + graph[u, v];
            }

            Print(distance, verticesCount);
        }


        private int AddPathToGridAndCountSwitches(List<PathFinderNodeDay22> path, long[,] finalGrid ) {
            var countSwitches = 0;
            foreach (var step in path)
            {
                char character = 'X';
                switch (step.CaveTool)
                {
                    case CaveTool.ClimbingGear:
                        character = 'C';
                        break;
                    case CaveTool.Torch:
                        character = 'T';
                        break;
                    case CaveTool.None:
                        character = 'O';
                        break;
                    default:
                        break;
                }
                finalGrid[step.X, step.Y] = character; // (long)step.CaveTool + 5; ;
                if (step.CaveTool != step.ParentCaveTool) countSwitches++;
            }
            return countSwitches;
        }
    }
}

