using AdventCode;
using AdventCode.Properties;
using System;
using System.Diagnostics;
using System.Text;

namespace AdventCode2017
{
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

        private enum Direction
        {
            Left = 1
            , Right = 2
            , Up = 3
            , Down = 4
        }

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
            var gridMapStride = test ? 1000 : 1000;
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
                        gridMap[gridPosition] = 0;
                    }
                }
            }

            //Walk grid
            for (int i = 0; i < 10000; i++)
            {
                position.nodeValue = gridMap[position.currentPostition];
                gridMap[position.currentPostition] = position.NewNodeValue;
                position.Move();
            }
            Print(gridMap, gridMapStride, gridMapMiddle, position);

            Debug.WriteLine($"Number of nodes being infected: {position.countInfectionCaused}");
        }

        private class Virus
        {
            public Virus(int Stride, int CurrentPostition)
            {
                stride = Stride;
                currentPostition = CurrentPostition;
            }
            public byte nodeValue { get; set; }
            public int countInfectionCaused { get; set; }
            public int currentPostition { get; set; }

            private Direction vector { get; set; } = Direction.Up;
            private int stride { get; set; }
            private Direction directionBasedOnValue { get { return isNotInfected ? Direction.Left : Direction.Right; } }
            private bool isNotInfected => nodeValue == 0;

            public void Move()
            {
                var newDirection = directionBasedOnValue;
                switch (vector)
                {
                    case Direction.Left:
                        if (newDirection == Direction.Left) { currentPostition = currentPostition + stride; vector = Direction.Down; }
                        if (newDirection == Direction.Right) { currentPostition = currentPostition - stride; vector = Direction.Up; }
                        break;
                    case Direction.Right:
                        if (newDirection == Direction.Left) { currentPostition = currentPostition - stride; vector = Direction.Up; }
                        if (newDirection == Direction.Right) { currentPostition = currentPostition + stride; vector = Direction.Down; }
                        break;
                    case Direction.Up:
                        if (newDirection == Direction.Left) { currentPostition--; vector = Direction.Left; }
                        if (newDirection == Direction.Right) { currentPostition++; vector = Direction.Right; }
                        break;
                    case Direction.Down:
                        if (newDirection == Direction.Left) { currentPostition++; vector = Direction.Right; }
                        if (newDirection == Direction.Right) { currentPostition--; vector = Direction.Left; }
                        break;
                    default:
                        break;
                }
            }

            public byte NewNodeValue
            {
                get
                {
                    if (isNotInfected)
                    {
                        countInfectionCaused++;
                        return 1;
                    }
                    else { return 0; }
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
                        printstring.Append(string.Format("[{0}]", value == 0 ? "." : "#"));
                    }
                    else
                    {
                        printstring.Append(string.Format("{0} ", value == 0 ? "." : "#"));
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
                pixelbytes[i++] = chartic == '#' ? (byte)1 : (byte)0;
            }
            return pixelbytes;
        }

        public void CalculateAnswerB()
        {
            //CalculateAnswerA();
        }
    }
}

