using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventCode2017
{
    public class Dag21 : AdventBase, AdventInterface
    {

        private bool test = false;
        private string puzzleString = Resources.dag21_2017;
        private int iterations = 5;
        public Dag21()
        {
            CalculateAnswerA();
            CalculateAnswerB();
        }

        //../.# => ##./#../...
        //.#./..#/### => #..#/..../..../#..#

        private Dictionary<byte[], byte[]> rules = new Dictionary<byte[], byte[]>();
        // "../.# => ##./#../...
        //.#./..#/### => #..#/..../..../#..#"}

        public void CalculateAnswerA()
        {
            //.#.
            //..#
            //###
            //test = false;
            if (test)
            {
                rules.Add(pixelStringToSquare("../.#"), pixelStringToSquare("##./#../..."));
                rules.Add(pixelStringToSquare(".#./..#/###"), pixelStringToSquare("#..#/..../..../#..#"));
            }
            else
            {
                foreach (var rulestring in puzzleString.Split('\n'))
                {
                    var rule = rulestring.Replace(" ", "").Replace(">", "").Replace("\r", "").Split('=');
                    rules.Add(pixelStringToSquare(rule[0]), pixelStringToSquare(rule[1]));
                }
            }

            var startGrid = pixelStringToSquare(".#...####");
            var gridList = new List<byte[]>();
            var newCreatedGrid = startGrid;
            gridList.Add(startGrid);
            for (int i = 0; i < (test ? 2 : iterations); i++)
            {
                var newGridList = new List<byte[]>();
                foreach (var newsq in gridList)
                {
                    newGridList.Add(GetNewSquare(newsq));
                }
                newCreatedGrid = Create1Grid(newGridList);
                gridList = CreatedNewSquares(newCreatedGrid);
            }
            Debug.WriteLine($"{newCreatedGrid.Where(c => c == (byte)1).Count()}");
        }

        /// <summary>
        /// Create 1 total grid (which is one / 1 byte[]) again, to be used in new evaluation
        /// </summary>
        /// <param name="squares"></param>
        /// <returns></returns>
        private byte[] Create1Grid(List<byte[]> squares)
        {
            var squareByteSize = squares.Sum(c => c.Length);
            var square1Length = (int)Math.Sqrt(squares.First().Length);
            var grid1Length = (int)Math.Sqrt(squareByteSize);
            var rowCount = square1Length * grid1Length;
            var newByte = new byte[squareByteSize];
            int x = 0;
            int y = 1;

            foreach (var square in squares)
            {
                for (int vertical = 0; vertical < square1Length; vertical++)
                {
                    for (int horizontalMove = 0; horizontalMove < square1Length; horizontalMove++)
                    {
                        newByte[x + horizontalMove + (vertical * grid1Length)] = square[(horizontalMove + (vertical * square1Length))];
                    }
                }
                if (((x + square1Length) % grid1Length) == 0) { x = rowCount * y++; } else { x += square1Length; };
            }
            return newByte;
        }

        ///TODO refactor DRY
        /// <summary>
        /// Create new squares based on the total grid
        /// </summary>
        /// <param name="newSquare"></param>
        /// <returns></returns>
        private List<byte[]> CreatedNewSquares(byte[] newSquare)
        {
            var newGrid = new List<byte[]>();
            var squareSize = 2;
            var square1Length = (int)System.Math.Sqrt(newSquare.Length);
            var rowCount = squareSize * square1Length;
            //var testsQuareLength = (newSquare.Length / 2d) % squareSize;
            if ((newSquare.Length / 2d) % squareSize == 0d)
            {
                var squareAmount = newSquare.Length / 4;
                var x = 0;
                var y = 1;
                for (int i = 0; i < squareAmount; i++)
                {
                    var new2by2Square = new byte[4];
                    new2by2Square[0] = newSquare[x + 0];
                    new2by2Square[1] = newSquare[x + 1];
                    new2by2Square[2] = newSquare[x + 0 + square1Length];
                    new2by2Square[3] = newSquare[x + 1 + square1Length];
                    newGrid.Add(new2by2Square);

                    if ((x + squareSize) % square1Length == 0) { x = rowCount * y++; }
                    else { x += squareSize; };
                }
            }
            else
            {
                squareSize = 3;
                var squareAmount = newSquare.Length / 9;
                rowCount = squareSize * square1Length;
                var x = 0;
                var y = 1;
                for (int i = 0; i < squareAmount; i++)
                {
                    var new2by2Square = new byte[9];
                    new2by2Square[0] = newSquare[x + 0];
                    new2by2Square[1] = newSquare[x + 1];
                    new2by2Square[2] = newSquare[x + 2];
                    new2by2Square[3] = newSquare[x + 0 + square1Length];
                    new2by2Square[4] = newSquare[x + 1 + square1Length];
                    new2by2Square[5] = newSquare[x + 2 + square1Length];
                    new2by2Square[6] = newSquare[x + 0 + square1Length * 2];
                    new2by2Square[7] = newSquare[x + 1 + square1Length * 2];
                    new2by2Square[8] = newSquare[x + 2 + square1Length * 2];
                    newGrid.Add(new2by2Square);

                    if ((x + squareSize) % square1Length == 0) { x = rowCount * y++; ; }
                    else { x += squareSize; };
                }
            }
            return newGrid;
        }

        /// <summary>
        /// Return compared value or if not found, return item1 = false
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        private (bool, byte[]) isEqualCompare(byte[] square)
        {
            foreach (var rule in rules)
            {
                if (rule.Key.SequenceEqual(square))
                {
                    return (true, rule.Value);
                }
            }
            return (false, null);
        }

        /// <summary>
        /// Rotate and flip to find a match and return new square
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        private byte[] GetNewSquare(byte[] square)
        {
            for (int i = 0; i < 4; i++)
            {
                var isEqual = isEqualCompare(square);
                if (isEqual.Item1) return isEqual.Item2;
                Rotate(ref square);
            }
            FlipX(ref square);
            for (int i = 0; i < 4; i++)
            {
                var isEqual = isEqualCompare(square);
                if (isEqual.Item1) return isEqual.Item2;
                Rotate(ref square);
            }
            FlipX(ref square);
            FlipY(ref square);
            for (int i = 0; i < 4; i++)
            {
                var isEqual = isEqualCompare(square);
                if (isEqual.Item1) return isEqual.Item2;
                Rotate(ref square);
            }
            return null;
        }

        /// <summary>
        /// Flip squares X-as
        /// </summary>
        /// <param name="square"></param>
        private void FlipX(ref byte[] square)
        {
            var rs = new byte[square.Length];

            switch (square.Length)
            {
                case 9:
                    rs[0] = square[2];
                    rs[1] = square[1];
                    rs[2] = square[0];
                    rs[3] = square[5];
                    rs[4] = square[4];
                    rs[5] = square[3];
                    rs[6] = square[8];
                    rs[7] = square[7];
                    rs[8] = square[6];
                    break;
                default:
                    rs[0] = square[1];
                    rs[1] = square[0];
                    rs[2] = square[3];
                    rs[3] = square[2];
                    break;
            }
            square = rs;
        }

        /// <summary>
        /// Flip squares Y-as
        /// </summary>
        /// <param name="square"></param>
        private void FlipY(ref byte[] square)
        {
            var rs = new byte[square.Length];

            switch (square.Length)
            {
                case 9:
                    rs[0] = square[6];
                    rs[1] = square[7];
                    rs[2] = square[8];
                    rs[3] = square[3];
                    rs[4] = square[4];
                    rs[5] = square[5];
                    rs[6] = square[0];
                    rs[7] = square[1];
                    rs[8] = square[2];
                    break;
                default:
                    rs[0] = square[2];
                    rs[1] = square[3];
                    rs[2] = square[0];
                    rs[3] = square[1];
                    break;
            }
            square = rs;
        }

        /// <summary>
        /// Rotate Square
        /// </summary>
        /// <param name="square"></param>
        private void Rotate(ref byte[] square)
        {
            var rs = new byte[square.Length];

            switch (square.Length)
            {
                case 9:
                    rs[0] = square[6];
                    rs[1] = square[3];
                    rs[2] = square[0];
                    rs[3] = square[7];
                    rs[4] = square[4];
                    rs[5] = square[1];
                    rs[6] = square[8];
                    rs[7] = square[5];
                    rs[8] = square[2];
                    break;
                default:
                    rs[0] = square[2];
                    rs[1] = square[0];
                    rs[2] = square[3];
                    rs[3] = square[1];
                    break;
            }
            square = rs;
        }

        /// <summary>
        /// Convert from # and . string (puzzleString) to a byte[]
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        private byte[] pixelStringToSquare(string pixels)
        {
            pixels = pixels.Replace("/", "");
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
            iterations = 18;
            CalculateAnswerA();
        }
    }
}

