using System;
using System.Drawing;
using System.Collections.Generic;

namespace Logic.Service.Pathfinder
{
    #region Structs
    public struct PathFinderNodeDay22
    {
        #region Variables Declaration
        public int F;
        public int G;
        public int H;
        public int X;
        public int Y;
        public int PX;
        public int PY;
        public CaveTool CaveTool;
        public CaveTool ParentCaveTool;
        #endregion
    }
    #endregion

    public enum CaveTool
    {
        ClimbingGear = 11
       , Torch = 22
       , None = 33
    }

    public class PathFinderDay22
    {
        #region Variables Declaration
        private byte[,] mGrid = null;
        private long[,] mCaveGrid = null;
        private PriorityQueue<PathFinderNodeDay22> mOpen = new PriorityQueue<PathFinderNodeDay22>(new ComparePFNode());
        private List<PathFinderNodeDay22> mClose = new List<PathFinderNodeDay22>();
        private bool mStop = false;
        private bool mStopped = true;
        private int mHoriz = 0;
        private int mHEstimate = 1;
        private bool mPunishChangeDirection = false;
        private bool mReopenCloseNodes = true;
        private bool mTieBreaker = true;
        private int mSearchLimit = 90000;
        #endregion

        #region Constructors
        public PathFinderDay22(byte[,] grid, long[,] caveGrid)
        {
            mGrid = grid;
            mCaveGrid = caveGrid;
        }
        #endregion

        #region Properties
        public int HeuristicEstimate
        {
            get { return mHEstimate; }
            set { mHEstimate = value; }
        }
        #endregion

        #region Methods
        public List<PathFinderNodeDay22> FindPath(Point start, Point end)
        {
            PathFinderNodeDay22 parentNode = new PathFinderNodeDay22();
            bool found = false;
            int gridX = mGrid.GetUpperBound(0);
            int gridY = mGrid.GetUpperBound(1);

            mStop = false;
            mStopped = false;
            mOpen.Clear();
            mClose.Clear();

            sbyte[,] direction = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

            parentNode.G = 0;
            parentNode.H = mHEstimate;
            parentNode.F = parentNode.G + parentNode.H;
            parentNode.X = start.X;
            parentNode.Y = start.Y;
            parentNode.PX = parentNode.X;
            parentNode.PY = parentNode.Y;
            parentNode.CaveTool = CaveTool.Torch;
            parentNode.ParentCaveTool = CaveTool.Torch;
            mOpen.Push(parentNode);

            while (mOpen.Count > 0 && !mStop)
            {
                parentNode = mOpen.Pop();

                if (parentNode.X == end.X && parentNode.Y == end.Y)
                {
                    mClose.Add(parentNode);
                    found = true;
                    break;
                }

                for (int i = 0; i < 4; i++)
                {
                    PathFinderNodeDay22 newInitialNode;
                    newInitialNode.X = parentNode.X + direction[i, 0];
                    newInitialNode.Y = parentNode.Y + direction[i, 1];

                    if (newInitialNode.X < 0 || newInitialNode.Y < 0 || newInitialNode.X >= gridX || newInitialNode.Y >= gridY)
                        continue;

                    int newG;

                    //TOOL
                    var toNodeCave = mCaveGrid[newInitialNode.X, newInitialNode.Y];
                    var optionalCaveTools = GetToolOptions(toNodeCave);

                    foreach (var toolPath in optionalCaveTools)
                    {
                        PathFinderNodeDay22 newNode = new PathFinderNodeDay22();
                        newNode.X = newInitialNode.X;
                        newNode.Y = newInitialNode.Y;

                        //You need the torch in last step
                        newNode.CaveTool = (newNode.X == end.X && newNode.Y == end.Y) ? CaveTool.Torch : toolPath;

                        var switchCost = newNode.CaveTool != parentNode.CaveTool ? 7 : 0;
                        newG = switchCost + parentNode.G + mGrid[newNode.X, newNode.Y];

                        if (newG == parentNode.G) continue;

                        int foundInOpenIndex = -1;
                        for (int j = 0; j < mOpen.Count; j++)
                        {
                            if (mOpen[j].X == newNode.X && mOpen[j].Y == newNode.Y && mOpen[j].CaveTool == newNode.CaveTool)
                            {
                                foundInOpenIndex = j;
                                break;
                            }
                        }
                        if (foundInOpenIndex != -1 && mOpen[foundInOpenIndex].G <= newG) continue;

                        int foundInCloseIndex = -1;
                        for (int j = 0; j < mClose.Count; j++)
                        {
                            if (mClose[j].X == newNode.X && mClose[j].Y == newNode.Y && mClose[j].CaveTool == newNode.CaveTool)
                            {
                                foundInCloseIndex = j;
                                break;
                            }
                        }
                        if (foundInCloseIndex != -1 && (mReopenCloseNodes || mClose[foundInCloseIndex].G <= newG)) continue;

                        newNode.PX = parentNode.X;
                        newNode.PY = parentNode.Y;
                        newNode.ParentCaveTool = parentNode.CaveTool;
                        newNode.G = newG;
                        newNode.H = mHEstimate * (Math.Abs(newNode.X - end.X) + Math.Abs(newNode.Y - end.Y));

                        newNode.F = newNode.G + newNode.H;
                        mOpen.Push(newNode);
                    }
                    mClose.Add(parentNode);
                }
            }
            if (found)
            {
                PathFinderNodeDay22 fNode = mClose[mClose.Count - 1];
                for (int i = mClose.Count - 1; i >= 0; i--)
                {
                    if (fNode.PX == mClose[i].X && fNode.PY == mClose[i].Y && fNode.ParentCaveTool == mClose[i].CaveTool || i == mClose.Count - 1) fNode = mClose[i];
                    else mClose.RemoveAt(i);
                }
                mStopped = true;
                return mClose;
            }

            mStopped = true;
            return null;
        }

        private List<CaveTool> GetToolOptions(long toNodeCave)
        {
            switch (toNodeCave)
            {
                case '.':
                    return new List<CaveTool> {  CaveTool.Torch, CaveTool.ClimbingGear };
                case '=':
                    return new List<CaveTool> { CaveTool.ClimbingGear , CaveTool.None};
                case '|':
                    return new List<CaveTool> { CaveTool.Torch,CaveTool.None };
                default:
                    return null;
            }

            //var fromTool = 
            //switch (toNodeCave)
            //{
            //    case '.':
            //        return new List<(int, CaveTool)> { (caveToolInUse != (CaveTool)toNodeCave ? 7 : 0,  CaveTool.ClimbingGear), (caveToolInUse != (CaveTool)toNodeCave ? 7 : 0, CaveTool.Torch) };
            //    case '=':
            //        return new List<(int, CaveTool)> { (caveToolInUse != (CaveTool)toNodeCave ? 7 : 0, CaveTool.ClimbingGear), (caveToolInUse != (CaveTool)toNodeCave ? 7 : 0, CaveTool.None) };
            //    case '|':
            //        return new List<(int, CaveTool)> { (caveToolInUse != (CaveTool)toNodeCave ? 7 : 0, CaveTool.Torch), (caveToolInUse != (CaveTool)toNodeCave ? 7 : 0, CaveTool.None) };
            //    default:
            //        return null;
            //}
        }


        #endregion

        internal class ComparePFNode : IComparer<PathFinderNodeDay22>
        {
            public int Compare(PathFinderNodeDay22 x, PathFinderNodeDay22 y)
            {
                if (x.F > y.F)
                    return 1;
                else if (x.F < y.F)
                    return -1;
                return 0;
            }
        }


    }

}
