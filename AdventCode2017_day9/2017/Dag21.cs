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
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Concurrent;

namespace AdventCode2017
{
    public class Dag21 : AdventBase, AdventInterface
    {

        private bool test = false;
        private string puzzleString = Resources.dag21_2017;
        private int iterations = 5;
        private readonly ConcurrentDictionary<Bitmap, Bitmap> bitmapPatterns = new ConcurrentDictionary<Bitmap, Bitmap>();
        private readonly Dictionary<byte[], Bitmap> cachedMatches = new Dictionary<byte[], Bitmap>();
        private object lockBitmap = new object();

        private byte[] startGrid => pixelStringToByteSquare(".#...####");

        public Dag21()
        {
            CalculateAnswerA();
             CalculateAnswerB();
        }

        //Test patterns
        //      ../.# => ##./#../...
        //      .#./..#/### => #..#/..../..../#..#"

        //      .#.
        //      ..#
        //      ###

        public void CalculateAnswerA()
        {
            
            if (test)
            {
                bitmapPatterns.TryAdd(pixelStringToBitmapSquare("../.#"), pixelStringToBitmapSquare("##./#../..."));
                bitmapPatterns.TryAdd(pixelStringToBitmapSquare(".#./..#/###"), pixelStringToBitmapSquare("#..#/..../..../#..#"));
            }
            else
            {
                foreach (var rulestring in puzzleString.Split('\n'))
                {
                    var rule = rulestring.Replace(" ", "").Replace(">", "").Replace("\r", "").Split('=');
                    bitmapPatterns.TryAdd(pixelStringToBitmapSquare(rule[0]), pixelStringToBitmapSquare(rule[1]));
                }
            }

            (int, List<byte[]>) gridList = (0, new List<byte[]>());
            Bitmap newCreatedGrid = null;
            gridList = (3, new List<byte[]>() { startGrid });

            ///Iterate all divide and reunite steps. Answer A will do 5, Answer B will do 11
            for (int i = 0; i < (test ? 2 : iterations); i++)
            {
                var newGridList = new List<Bitmap>();

                foreach (var newsq in gridList.Item2)
                {
                    newGridList.Add(GetNewSquare((gridList.Item1, newsq)));
                }
                newCreatedGrid = CreateTotalGrid(newGridList);
                var newSquares = CreatedNewSquares(newCreatedGrid);
                gridList = newSquares;

                //Task.Run(() => PrintDebugMessage(newCreatedGrid, i));
            }
            Debug.WriteLine($"{CountPattern(newCreatedGrid)}");
        }

        static async Task PrintDebugMessage(Bitmap bmp, int i)
        {
            await Task.Delay(1000);
            await Task.Run(() => Print(bmp, $"iteration {i}"));
        }


        /// <summary>
        /// Create 1 total grid (which is one / 1 byte[]) again, to be used in new evaluation
        /// </summary>
        /// <param name="squares"></param>
        /// <returns></returns>
        private Bitmap CreateTotalGrid(List<Bitmap> squares)
        {
            var squareByteSize = squares.Sum(c => c.Size.Height * c.Size.Width);
            var squareStride = (int)squares.First().Width;
            var gridStride = (int)Math.Sqrt(squareByteSize);
            var rowCount = squareStride * gridStride;
            var newTotalGrid = new Bitmap(gridStride, gridStride);
            int x = 0;
            int y = 0;

            foreach (var square in squares)
            {
                for (int vertical = 0; vertical < squareStride; vertical++)
                {
                    for (int horizontalMove = 0; horizontalMove < squareStride; horizontalMove++)
                    {
                        var color = square.GetPixel(horizontalMove, vertical);
                        newTotalGrid.SetPixel(x + horizontalMove, y + vertical, color);
                    }
                }
                if (((x + squareStride) % gridStride) == 0) { x = 0; y += squareStride; } else { x += squareStride; };
            }
            return newTotalGrid;
        }

        /// <summary>
        /// Create new squares based on the total grid
        /// </summary>
        /// <param name="totalGrid"></param>
        /// <returns></returns>
        private (int, List<byte[]>) CreatedNewSquares(Bitmap totalGrid)
        {
            var newGrid = new List<Bitmap>();
            var gridSizeLength = totalGrid.Width;
            var squareSize = gridSizeLength % 2d == 0d ? 2 : 3;
            var totalSquareAmount = (gridSizeLength * gridSizeLength) / (squareSize * squareSize);
            var x = 0;
            var y = 0;

            for (int i = 0; i < totalSquareAmount; i++)
            {
                var newSquareImage = new Bitmap(squareSize, squareSize);
                for (int vertical = 0; vertical < squareSize; vertical++)
                {
                    for (int horizontalMove = 0; horizontalMove < squareSize; horizontalMove++)
                    {
                        var color = totalGrid.GetPixel(x + horizontalMove, vertical + y);
                        newSquareImage.SetPixel(horizontalMove, vertical, color);
                    }
                }
                newGrid.Add(newSquareImage);
                if (((x + squareSize) % gridSizeLength) == 0) { x = 0; y += squareSize; } else { x += squareSize; };
            }
            return (squareSize, newGrid.Select(c => ImageToByte(c)).ToList());
        }


        private static void Print(Bitmap newSquareImage, string prefix)
        {

            var printstring = new StringBuilder();
            printstring.AppendLine(prefix);
            for (int y = 0; y < newSquareImage.Height; y++)
            {
                for (int x = 0; x < newSquareImage.Width; x++)
                {
                    var pixel = newSquareImage.GetPixel(x, y);
                    printstring.Append(string.Format("{0}", pixel.Name.Equals("ffffffff") ? "." : "#"));
                }
                printstring.AppendLine("");
            }
            printstring.AppendLine();
            Debug.Print(printstring.ToString());
        }


        private static object lockDictionaryBitmap = new object();
        /// <summary>
        /// Return compared value or if not found, return item1 = false
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        private (bool, Bitmap) isEqualCompare(Bitmap square)
        {

            var arr1 = ImageToByte(square);
            var matchedCachedImage = cachedMatches.Where(c => arr1.SequenceEqual(c.Key)).Any();
            if (matchedCachedImage)
            {
                var retv = cachedMatches.Where(c => arr1.SequenceEqual(c.Key)).Select(c => c.Value).Single();
                return (true, retv);
            }

            var retTuple = (false, (Bitmap)null);
            //byte[] matchArr = null;

            foreach (var rule in bitmapPatterns.Where(c => c.Key.Width == square.Width))
            {


                // Parallel.ForEach(bitmapPatterns.Where(c=> c.Key.Width == square.Width), (rule) =>
                //{
                if (CompareBitmapsFast(rule.Key, arr1))
                {
                   // matchArr = ImageToByte(rule.Key);
                    retTuple = (true, rule.Value);
                }
                // });
            }
            if (retTuple.Item1 && !matchedCachedImage)
            {
                cachedMatches.Add(arr1, retTuple.Item2);
            }
            return retTuple;
        }


        public bool CompareBitmapsFast(Bitmap bmp1, byte[] arr2)
        {
            byte[] arr1;
            //byte[] arr2;
            lock (lockBitmap)
            {
                arr1 = ImageToByte(bmp1);
              //  arr2 = ImageToByte(bmp2);
            }
            return arr1.SequenceEqual(arr2);
        }

        public int CountPattern(Bitmap bmp1)
        {
            Print(bmp1, "Final Result");
            var count = 0;
            for (int y = 0; y < bmp1.Height; y++)
            {
                for (int x = 0; x < bmp1.Width; x++)
                {
                    var pixel = bmp1.GetPixel(x, y);
                    if (!pixel.Name.Equals("ffffffff")) count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Rotate and flip to find a match and return new square
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        private Bitmap GetNewSquare((int, byte[]) squareByte)
        {
            var matchedCachedImage = cachedMatches.Where(c => squareByte.Item2.SequenceEqual(c.Key)).Any();
            if (matchedCachedImage)
            {
                return cachedMatches.Where(c => squareByte.Item2.SequenceEqual(c.Key)).Select(c => c.Value).Single();
            }


            //Convert from bytearray to Bitmap
            var squareImage = CopyDataToBitmap(squareByte);
            for (int i = 0; i < 4; i++)
            {
                var isEqual = isEqualCompare(squareImage);
                if (isEqual.Item1)
                {
                    CacheOriginalByte(squareByte.Item2, isEqual.Item2);
                    return isEqual.Item2;
                }
                squareImage.RotateFlip(RotateFlipType.Rotate90FlipNone); // Rotate(ref square);
            }
            squareImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            for (int i = 0; i < 4; i++)
            {
                var isEqual = isEqualCompare(squareImage);
                if (isEqual.Item1)
                {
                    CacheOriginalByte(squareByte.Item2, isEqual.Item2);
                    return isEqual.Item2;
                }
                squareImage.RotateFlip(RotateFlipType.Rotate90FlipNone); // Rotate(ref square);
            }
            throw new KeyNotFoundException("Output square is not found after rotating and flipping original square");
        }

        private void CacheOriginalByte(byte[] squareByte, Bitmap item2)
        {
            var saveMatchedNotExistingCachedImage = !cachedMatches.Where(c => squareByte.SequenceEqual(c.Key)).Any();
            if (saveMatchedNotExistingCachedImage)
            {
                cachedMatches.Add(squareByte, item2);
            }
        }

        public Bitmap CopyDataToBitmap((int, byte[]) data)
        {
            //Here create the Bitmap to the know height, width and format
            Bitmap bmp = new Bitmap(data.Item1, data.Item1);
            //Create a BitmapData and Lock all pixels to be written 
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            //Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(data.Item2, 0, bmpData.Scan0, data.Item2.Length);
            //Unlock the pixels
            bmp.UnlockBits(bmpData);
            //Return the bitmap 
            return bmp;
        }

        /// <summary>
        /// Convert from # and . string (puzzleString) to a byte[]
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        private byte[] pixelStringToByteSquare(string pixels)
        {
            pixels = pixels.Replace("/", "");
            int amount = pixels.Count();
            int stride = (int)Math.Sqrt(amount);
            var newImage = new Bitmap(stride, stride);
            var charCount = 0;
            for (int y = 0; y < stride; y++)
            {
                for (int x = 0; x < stride; x++)
                {
                    newImage.SetPixel(x, y, pixels[charCount++] == '.' ? Color.White : Color.Black);

                }
            }
            return ImageToByte(newImage);
        }

        private Bitmap pixelStringToBitmapSquare(string pixels)
        {
            pixels = pixels.Replace("/", "");
            int amount = pixels.Count();
            int stride = (int)Math.Sqrt(amount);
            var newImage = new Bitmap(stride, stride);
            var charCount = 0;
            for (int y = 0; y < stride; y++)
            {
                for (int x = 0; x < stride; x++)
                {
                    newImage.SetPixel(x, y, pixels[charCount++] == '.' ? Color.White : Color.Black);

                }
            }
            return newImage;
        }

        public static byte[] ImageToByte(Bitmap bitmap)
        {
            BitmapData bmpdata = null;
            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;
                Marshal.Copy(ptr, bytedata, 0, numbytes);
                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }
        }

        public void CalculateAnswerB()
        {
            iterations = 18;
            CalculateAnswerA();
        }
    }
}

