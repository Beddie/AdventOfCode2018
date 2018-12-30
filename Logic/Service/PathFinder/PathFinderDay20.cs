using System;
using System.Drawing;
using System.Collections.Generic;

namespace Logic.Service.Pathfinder
{
    #region Structs
    public struct PathFinderNodeDay20
    {
        #region Variables Declaration
        public int F;
        public int G;
        public int H;
        public int X;
        public int Y;
        public int PX;
        public int PY;
        #endregion
    }
    #endregion

    public class PathFinderDay20
    {
        #region Variables Declaration
        private int[,] mGrid = null;
        private PriorityQueue<PathFinderNodeDay20> mOpen = new PriorityQueue<PathFinderNodeDay20>(new ComparePFNode());
        private List<PathFinderNodeDay20> mClose = new List<PathFinderNodeDay20>();
        private bool mStop = false;
        private bool mStopped = true;
        private int mHoriz = 0;
        private int mHEstimate = 1;
        private bool mPunishChangeDirection = false;
        private bool mReopenCloseNodes = false;
        private bool mTieBreaker = false;
        private bool mHeavyDiagonals = false;
        private int mSearchLimit = 2000;
        #endregion

        #region Constructors
        public PathFinderDay20(int[,] grid)
        {
            mGrid = grid;
        }
        #endregion

        #region Properties
        public bool Stopped
        {
            get { return mStopped; }
        }

        public int HeuristicEstimate
        {
            get { return mHEstimate; }
            set { mHEstimate = value; }
        }
        #endregion

        #region Methods
        public void FindPathStop()
        {
            mStop = true;
        }

        public List<PathFinderNodeDay20> FindPath(Point start, Point end)
        {
            PathFinderNodeDay20 parentNode;
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
                    PathFinderNodeDay20 newNode;
                    newNode.X = parentNode.X + direction[i, 0];
                    newNode.Y = parentNode.Y + direction[i, 1];

                    if (newNode.X < 0 || newNode.Y < 0 || newNode.X >= gridX || newNode.Y >= gridY)
                        continue;

                    int newG;


                    var val = mGrid[newNode.X, newNode.Y];
                    if (val > 0)  val = 1;

                    newG = parentNode.G + val;

                    if (newG == parentNode.G) continue;

                    if (mPunishChangeDirection)
                    {
                        if ((newNode.X - parentNode.X) != 0)
                        {
                            if (mHoriz == 0)
                                newG += 20;
                        }
                        if ((newNode.Y - parentNode.Y) != 0)
                        {
                            if (mHoriz != 0)
                                newG += 20;

                        }
                    }

                    int foundInOpenIndex = -1;
                    for (int j = 0; j < mOpen.Count; j++)
                    {
                        if (mOpen[j].X == newNode.X && mOpen[j].Y == newNode.Y)
                        {
                            foundInOpenIndex = j;
                            break;
                        }
                    }
                    if (foundInOpenIndex != -1 && mOpen[foundInOpenIndex].G <= newG) continue;

                    int foundInCloseIndex = -1;
                    for (int j = 0; j < mClose.Count; j++)
                    {
                        if (mClose[j].X == newNode.X && mClose[j].Y == newNode.Y)
                        {
                            foundInCloseIndex = j;
                            break;
                        }
                    }
                    if (foundInCloseIndex != -1 && (mReopenCloseNodes || mClose[foundInCloseIndex].G <= newG)) continue;

                    newNode.PX = parentNode.X;
                    newNode.PY = parentNode.Y;
                    newNode.G = newG;
                    newNode.H = mHEstimate * (Math.Abs(newNode.X - end.X) + Math.Abs(newNode.Y - end.Y));
                    newNode.F = newNode.G + newNode.H;
                    mOpen.Push(newNode);
                }
                mClose.Add(parentNode);
            }

            if (found)
            {
                PathFinderNodeDay20 fNode = mClose[mClose.Count - 1];
                for (int i = mClose.Count - 1; i >= 0; i--)
                {
                    if (fNode.PX == mClose[i].X && fNode.PY == mClose[i].Y || i == mClose.Count - 1) fNode = mClose[i];
                    else mClose.RemoveAt(i);
                }
                mStopped = true;
                return mClose;
            }
            mStopped = true;
            return null;
        }
        #endregion

        internal class ComparePFNode : IComparer<PathFinderNodeDay20>
        {
            public int Compare(PathFinderNodeDay20 x, PathFinderNodeDay20 y)
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
