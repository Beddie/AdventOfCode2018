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
    public class Day18 : AdventBase
    {
        public Day18()
        {
            //Test = true;
            PuzzleInput = Test ? Resources.Day18Example : Resources.Day18;
            ID = 18;
            Name = "Day 18: Settlers of The North Pole";
        }

        public override string[] Solution()
        {
            return new string[] { "623583", "107912"
            };
        }

        public override string Part1()
        {
            var strideX = PuzzleInput.Substring(0, PuzzleInput.IndexOf("\r\n")).Length;
            var strideY = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Count();
            var yVal = 0;
            var acres = new int[strideX, strideY];

            foreach (var puzzleAcre in PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList())
            {
                var x = 0;
                foreach (var character in puzzleAcre)
                {
                    acres[x, yVal] = character;
                    x++;
                }
                yVal++;
            }
            if (Test) Print(acres);

            var minutes = 10;
            for (int i = 0; i < minutes; i++)
            {

                int[,] newAcres = acres.Clone() as int[,];

                for (int y = 0; y < acres.GetLength(1); y++)
                {
                    for (int x = 0; x < acres.GetLength(0); x++)
                    {
                        newAcres[x, y] = (int)GetNewAcreType(x, y, acres);
                    }
                }
                acres = newAcres;
                if (Test) Print(acres);
            }

            //Count wooded * lumberyars
            return LumberResourcesAnswer(acres);
        }

        private string LumberResourcesAnswer(int[,] acres)
        {
            var lumberYardCount = 0;
            var woodCount = 0;
            for (int y = 0; y <= acres.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= acres.GetUpperBound(0); x++)
                {
                    if (acres[x, y] == (int)AcreType.Trees) { woodCount++; }
                    else if (acres[x, y] == (int)AcreType.LumberYard) { lumberYardCount++; }
                }
            }
            return (woodCount * lumberYardCount).ToString();
        }

        private AcreType GetNewAcreType(int x, int y, int[,] acres)
        {
            var currentAcreType = (AcreType)acres[x, y];
            var acreTypes = GetAdjacentTypes(x, y, acres);
            switch (currentAcreType)
            {
                case AcreType.Open:
                    if (acreTypes.Count(c => c == AcreType.Trees) >= 3) return AcreType.Trees;
                    return AcreType.Open;
                case AcreType.Trees:
                    if (acreTypes.Count(c => c == AcreType.LumberYard) >= 3) return AcreType.LumberYard;
                    return AcreType.Trees;
                case AcreType.LumberYard:
                    if ((acreTypes.Count(c => c == AcreType.LumberYard) >= 1) && (acreTypes.Count(c => c == AcreType.Trees) >= 1)) return AcreType.LumberYard;
                    return AcreType.Open;
                default:
                    throw new Exception("Non existing acre");
            }
        }

        private List<AcreType> GetAdjacentTypes(int x, int y, int[,] acres)
        {
            var acreTypes = new List<AcreType>();
            var checkX = x - 1;
            var checkY = y;
            if (checkX >= 0)
            {
                if (checkY - 1 >= 0) acreTypes.Add((AcreType)acres[checkX, checkY - 1]);
                acreTypes.Add((AcreType)acres[checkX, checkY]);
                if (checkY + 1 <= acres.GetUpperBound(1)) acreTypes.Add((AcreType)acres[checkX, checkY + 1]);
            }
            checkX = x + 1;
            if (checkX <= acres.GetUpperBound(0))
            {
                if (checkY - 1 >= 0) acreTypes.Add((AcreType)acres[checkX, checkY - 1]);
                acreTypes.Add((AcreType)acres[checkX, checkY]);
                if (checkY + 1 <= acres.GetUpperBound(1)) acreTypes.Add((AcreType)acres[checkX, checkY + 1]);
            }
            checkX = x;
            checkY = y - 1;
            if (checkY >= 0)
            {
                acreTypes.Add((AcreType)acres[checkX, checkY]);
            }
            checkY = y + 1;
            if (checkY <= acres.GetUpperBound(1))
            {
                acreTypes.Add((AcreType)acres[checkX, checkY]);
            }

            return acreTypes;
        }

        private enum AcreType
        {
            Open = (int)'.'
           , Trees = (int)'|'
           , LumberYard = (int)'#'
        }

        private void Print(int[,] grid, int firstX = 0, int lastX = 0, int firstY = 0, int lastY = 0)
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
            var strideX = PuzzleInput.Substring(0, PuzzleInput.IndexOf("\r\n")).Length;
            var strideY = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Count();
            var yVal = 0;
            var acres = new int[strideX, strideY];
            var patternHistory = new Dictionary<int, int[,]>();
            int[,] patternToFind = null;
            var sequence = 0;
            foreach (var puzzleAcre in PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList())
            {
                var x = 0;
                foreach (var character in puzzleAcre)
                {
                    acres[x, yVal] = character;
                    x++;
                }
                yVal++;
            }
            if (Test) Print(acres);

            var minutes = 1000000000;
            for (int i = 0; i < minutes; i++)
            {
                int[,] newAcres = acres.Clone() as int[,];

                for (int y = 0; y < acres.GetLength(1); y++)
                {
                    for (int x = 0; x < acres.GetLength(0); x++)
                    {
                        newAcres[x, y] = (int)GetNewAcreType(x, y, acres);
                    }
                }

                //First find recurrent pattern
                if (patternToFind == null)
                {
                    int? isknownpattern = CheckPatterns(patternHistory, newAcres);

                    if (isknownpattern.HasValue)
                    {
                        sequence = i;
                        patternToFind = patternHistory.Where(c => c.Key == isknownpattern).First().Value;
                    }
                    patternHistory.Add(i, newAcres);
                }
                //Recurrent pattern has been found, check after how many minutes pattern shows up again 
                else
                {
                    if (CheckPattern(patternToFind, newAcres))
                    {
                        //Reduce remainingminutes based on pattern-stride
                        sequence = i - sequence;
                        var remainingloops = (minutes - i) % sequence;
                        i = minutes - remainingloops;
                    };
                    patternHistory.Add(i, newAcres);
                }
                acres = newAcres;
            }

            //Count wooded * lumberyars
            return LumberResourcesAnswer(acres);
        }

        private int? CheckPatterns(Dictionary<int, int[,]> checkPattern, int[,] newAcres)
        {
            foreach (var pattern in checkPattern)
            {
                var same = true;
                for (int y = 0; y < newAcres.GetLength(1); y++)
                {
                    for (int x = 0; x < newAcres.GetLength(0); x++)
                    {
                        if (newAcres[x, y] != pattern.Value[x, y]) same = false;
                    }
                }

                if (same) return pattern.Key;
            }
            return null;
        }

        private bool CheckPattern(int[,] checkPattern, int[,] newAcres)
        {
            for (int y = 0; y < newAcres.GetLength(1); y++)
            {
                for (int x = 0; x < newAcres.GetLength(0); x++)
                {
                    if (newAcres[x, y] != checkPattern[x, y]) return false;
                }
            }
            return true;
        }
    }
}

