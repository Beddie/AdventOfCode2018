using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace AdventCode2017
{
    public static class LocalizationMarketExtensions
    {

    }




    public class Dag22 : AdventBase, AdventInterface
    {
        private bool test = false;
        private string puzzleString = Resources.dag22_2017;
        public Dag22()
        {
            CalculateAnswerA();
            CalculateAnswerB();
        }

        private byte[] startGrid;

        [Flags]
        private enum Direction
        {
            Left = 1
            , Right = 2
            , Up = 3
            , Down = 4
            , Reverse = 5
            , Forward = 6
        }

        [Flags]
        private enum VirusStatus
        {
            [Description(".")]
            Clean = 0,
            [Description("W")]
            Weakened = 1,
            [Description("#")]
            Infected = 3,
            [Description("F")]
            Flagged = 4,
            [Description("X")]
            unKnown = 5
        }

        public static Dictionary<int, string> VirusStatusHelper = EnumDictionary<VirusStatus>();


        public void CalculateAnswerA()
        {
            if (test)
            {
                startGrid = pixelStringToSquare("..##.....");
            }
            else
            {
                startGrid = pixelStringToSquare(puzzleString);
            }

            //Build big Grid around startgrid
            var gridMapStride = test ? 10
                : 1000;
            var gridMap = new byte[gridMapStride * gridMapStride];
            var gridMapMiddle = (int)(gridMapStride / 2d);
            var startGridStride = (int)Math.Sqrt(startGrid.Length);
            var startGridMiddle = ((startGridStride - 1) / 2);
            var startPositionLeftX = gridMapMiddle - startGridMiddle;
            var startPositionRightX = gridMapMiddle + startGridMiddle;
            var startPositionUpperY = gridMapMiddle - startGridMiddle;
            var startPositionLowerY = gridMapMiddle + startGridMiddle;
            var position = new Virus(gridMapStride, gridMapMiddle + (gridMapMiddle * gridMapStride));

            //Initialize startgrid into gridMap;
            for (int y = 0; y < gridMapStride; y++)
            {
                for (int x = 0; x < gridMapStride; x++)
                {
                    var gridPosition = x + y * gridMapStride;

                    if ((y >= startPositionUpperY && y <= startPositionLowerY) && (x >= startPositionLeftX && x <= startPositionRightX))
                    {
                        gridMap[gridPosition] = startGrid[(x - startPositionLeftX) + ((y - startPositionUpperY) * startGridStride)];
                    }
                    else
                    {
                        gridMap[gridPosition] = (byte)VirusStatus.Clean;
                    }
                }
            }

            //Walk grid
            for (int i = 0; i < 10000000; i++)
            {
                position.nodeValue = gridMap[position.currentPostition];
                gridMap[position.currentPostition] = (byte)position.NewNodeStatus;
                position.Move();
            }
                Print(gridMap, gridMapStride, gridMapMiddle, position);

            Debug.WriteLine($"Number of nodes being infected: {position.countInfectionCaused}");
        }

        private class Virus
        {

            public Virus(int _stride, int _currentPostition)
            {
                stride = _stride;
                currentPostition = _currentPostition;
            }
            public byte nodeValue { get; set; }
            public int countInfectionCaused { get; set; }
            public int currentPostition { get; set; }
            private VirusStatus status => (VirusStatus)nodeValue;
            private Direction vector { get; set; } = Direction.Up;
            private int stride { get; set; }
            private Direction directionBasedOnValue
            {
                get
                {
                    switch (status)
                    {
                        case VirusStatus.Clean:
                            return Direction.Left;
                        case VirusStatus.Weakened:
                            return Direction.Forward;
                        case VirusStatus.Infected:
                            return Direction.Right;
                        case VirusStatus.Flagged:
                            return Direction.Reverse;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            private void MovePositionBasedOnDirection(Direction direction) {

                switch (direction)
                {
                    case Direction.Left:
                        currentPostition--; vector = Direction.Left;
                        break;
                    case Direction.Right:
                        currentPostition++; vector = Direction.Right;
                        break;
                    case Direction.Up:
                        currentPostition = currentPostition - stride; vector = Direction.Up;
                        break;
                    case Direction.Down:
                        currentPostition = currentPostition + stride; vector = Direction.Down;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            private void MoveRight() => currentPostition++;

            public void Move()
            {
                var newDirection = directionBasedOnValue;
                switch (vector)
                {
                    case Direction.Left:
                        if (newDirection == Direction.Left) { MovePositionBasedOnDirection(Direction.Down); }
                        if (newDirection == Direction.Right) { MovePositionBasedOnDirection(Direction.Up); }
                        if (newDirection == Direction.Reverse) { MovePositionBasedOnDirection(Direction.Right); }
                        if (newDirection == Direction.Forward) { MovePositionBasedOnDirection(Direction.Left); }
                        break;
                    case Direction.Right:
                        if (newDirection == Direction.Left) { MovePositionBasedOnDirection(Direction.Up); }
                        if (newDirection == Direction.Right) { MovePositionBasedOnDirection(Direction.Down); }
                        if (newDirection == Direction.Reverse) { MovePositionBasedOnDirection(Direction.Left); }
                        if (newDirection == Direction.Forward) { MovePositionBasedOnDirection(Direction.Right); }
                        break;
                    case Direction.Up:
                        if (newDirection == Direction.Left) { MovePositionBasedOnDirection(Direction.Left); }
                        if (newDirection == Direction.Right) { MovePositionBasedOnDirection(Direction.Right); }
                        if (newDirection == Direction.Reverse) { MovePositionBasedOnDirection(Direction.Down); }
                        if (newDirection == Direction.Forward) { MovePositionBasedOnDirection(Direction.Up); }
                        break;
                    case Direction.Down:
                        if (newDirection == Direction.Left) { MovePositionBasedOnDirection(Direction.Right); }
                        if (newDirection == Direction.Right) { MovePositionBasedOnDirection(Direction.Left); }
                        if (newDirection == Direction.Reverse) { MovePositionBasedOnDirection(Direction.Up); }
                        if (newDirection == Direction.Forward) { MovePositionBasedOnDirection(Direction.Down); }
                        break;
                   
                    default:
                        throw new NotSupportedException();
                }
            }

            public VirusStatus NewNodeStatus
            {
                get
                {
                    switch (status)
                    {
                        case VirusStatus.Clean:
                            return VirusStatus.Weakened;
                        case VirusStatus.Weakened:
                            countInfectionCaused++;
                            return VirusStatus.Infected;
                        case VirusStatus.Infected:
                            return VirusStatus.Flagged;
                        case VirusStatus.Flagged:
                            return VirusStatus.Clean;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }
        }

        private static void Print(byte[] bitmapByte, int stride, int gridMapMiddle, Virus postition)
        {
            var printstring = new StringBuilder();
            for (int y = 0; y < stride; y++)
            {
                for (int x = 0; x < stride; x++)
                {
                    var position = x + y * stride;
                    var value = bitmapByte[position];
                    if (position == postition.currentPostition)
                    {
                        printstring.Remove(printstring.Length - 1, 1);
                        printstring.Append(string.Format("[{0}]", VirusStatusHelper[value]));
                    }
                    else
                    {
                        printstring.Append(string.Format("{0} ", VirusStatusHelper[value]));
                    }
                }
                printstring.AppendLine("");
            }
            printstring.AppendLine();
            Debug.Print(printstring.ToString());
        }




        /// <summary>
        /// Convert from # and . string (puzzleString) to a byte[]
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        private byte[] pixelStringToSquare(string pixels)
        {
            pixels = pixels.Replace(" ", "").Replace(">", "").Replace("\r", "").Replace("/", "").Replace("\n", "");
            var pixelbytes = new byte[pixels.Length];
            var i = 0;
            foreach (var chartic in pixels)
            {
                pixelbytes[i++] = chartic == '#' ? (byte)VirusStatus.Infected : (byte)VirusStatus.Clean;
            }
            return pixelbytes;
        }

        public void CalculateAnswerB()
        {
            //CalculateAnswerA();
        }
    }
}

