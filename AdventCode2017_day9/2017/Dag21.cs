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

        private bool test = true;
        private string puzzleString = Resources.dag21_2017;
        public Dag21()
        {

            CalculateAnswerA();
            //  CalculateAnswerB();

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
                    var rule = rulestring.Replace(" ", "").Replace(">", "").Split('=');
                    rules.Add(pixelStringToSquare(rule[0]), pixelStringToSquare(rule[1]));
                }
            }
            var startGrid = pixelStringToSquare(".#...####");
            var gridList = new List<byte[]>();
            gridList.Add(startGrid);
            for (int i = 0; i < 5; i++)
            {
                //TODO refactor and improve
                CreateNewGrid(ref gridList);
            }
        }

        private void CreateNewGrid(ref List<byte[]> gridList)
        {
            var newGridList = new List<byte[]>();
            foreach (var newsq in gridList)
            {
                newGridList.Add(GetNewSquare(newsq));
            }
            var newCreatedGrid = create1Grid(newGridList);
            gridList = CreatedNewSquares(newCreatedGrid);
        }


        /// <summary>
        /// Create 1 total grid (which is one / 1 byte[]) again, to be used in new evaluation
        /// </summary>
        /// <param name="squares"></param>
        /// <returns></returns>
        private byte[] create1Grid(List<byte[]> squares)
        {
            var squareByteSize = squares.First().Length;
            var square1Length = (int)Math.Sqrt(squareByteSize);

            var grid1Length = squares.Count() > 1 ? (int)Math.Sqrt(squareByteSize) : square1Length;
            var byteLength = squareByteSize * squares.Count();
            var rowCount = square1Length * grid1Length;
            var newByte = new byte[byteLength];
            var x = 0;
            var y = 1;

            if (square1Length == 3)
            {
                foreach (var square in squares)
                {
                    newByte[x] = square[0];
                    newByte[x + 1] = square[1];
                    newByte[x + 2] = square[2];
                    newByte[x + grid1Length] = square[3];
                    newByte[x + 1 + grid1Length] = square[4];
                    newByte[x + 2 + grid1Length] = square[5];
                    newByte[x + 0 + 2 * grid1Length] = square[6];
                    newByte[x + 1 + 2 * grid1Length] = square[7];
                    newByte[x + 2 + 2 * grid1Length] = square[8];
                }

                if ((x + square1Length) % grid1Length == 0) { x = rowCount * y++; }
                else { x += square1Length; };
            }
            else if (square1Length == 4)
            {
                foreach (var square in squares)
                {
                    newByte[x] = square[0];
                    newByte[x + 1] = square[1];
                    newByte[x + 2] = square[2];
                    newByte[x + 3] = square[3];
                    newByte[x + grid1Length] = square[4];
                    newByte[x + 1 + grid1Length] = square[5];
                    newByte[x + 2 + grid1Length] = square[6];
                    newByte[x + 3 * grid1Length] = square[7];
                    newByte[x + 2 * grid1Length] = square[8];
                    newByte[x + 1 + 2 * grid1Length] = square[9];
                    newByte[x + 2 + 2 * grid1Length] = square[10];
                    newByte[x + 3 + 2 * grid1Length] = square[11];
                    newByte[x + 3 * grid1Length] = square[12];
                    newByte[x + 1 + 3 * grid1Length] = square[13];
                    newByte[x + 2 + 3 * grid1Length] = square[14];
                    newByte[x + 3 + 3 * grid1Length] = square[15];
                }

                if ((x + square1Length) % grid1Length == 0) { x = rowCount * y++; }
                else { x += square1Length; };
            }
            //TODO ADD square size 5
            else
            {
                foreach (var square in squares)
                {
                    newByte[x] = square[0];
                    newByte[x + 1] = square[1];
                    newByte[x + grid1Length] = square[2];
                    newByte[x + 1 + grid1Length] = square[3];
                }

                if ((x + square1Length) % grid1Length == 0) { x = rowCount * y++; }
                else { x += square1Length; };
            }

            return newByte;
        }

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
            if ((newSquare.Length / 2) % squareSize == 0)
            {
                var squareAmount = System.Math.Sqrt(newSquare.Length);
                var x = 0;
                var y = 1;
                for (int i = 0; i < squareAmount; i++)
                {
                    var new2by2Square = new byte[4];
                    new2by2Square[0] = newSquare[x + 0];
                    new2by2Square[1] = newSquare[x + 1];
                    new2by2Square[2] = newSquare[x + square1Length];
                    new2by2Square[3] = newSquare[x + 1 + square1Length];
                    newGrid.Add(new2by2Square);

                    if ((x + squareSize) % square1Length == 0) { x = rowCount * y++; }
                    else { x += squareSize; };
                }
            }
            else
            {
                squareSize = 3;
                square1Length = (int)System.Math.Sqrt(newSquare.Length);
                var squareAmount = square1Length;
                rowCount = squareSize * square1Length;
                var x = 0;
                var y = 1;
                for (int i = 0; i < squareAmount; i++)
                {
                    var new2by2Square = new byte[9];
                    new2by2Square[0] = newSquare[x + 0];
                    new2by2Square[1] = newSquare[x + 1];
                    new2by2Square[2] = newSquare[x + 2];
                    new2by2Square[3] = newSquare[x + square1Length];
                    new2by2Square[4] = newSquare[x + 1 + square1Length];
                    new2by2Square[5] = newSquare[x + 2 + square1Length];
                    new2by2Square[6] = newSquare[x + square1Length * 2];
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
            new Dag18Program(0, true).CalculateAnswerB();
        }
    }
}

