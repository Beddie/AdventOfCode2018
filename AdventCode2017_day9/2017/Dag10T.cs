using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCode2017
{
    public class Dag10T : AdventBase
    {
        private static bool test = false;
        private string text = test ? "3, 4, 1, 5" : Resources.dag10_2017;
        private int index = 0;

        private List<int> baseList;
        private int baseLengthList;

        private int remainingLengthToEndOfBaseList { get { return baseLengthList - index; } }
        private int amountToBeTaken(int knotlength) { return knotlength > remainingLengthToEndOfBaseList ? remainingLengthToEndOfBaseList : knotlength; }
        private int amountToBeTakenFromStartOfBaseList(int knotlength) { var usedAtTheEnd = knotlength - remainingLengthToEndOfBaseList; return usedAtTheEnd < 0 ? 0 : usedAtTheEnd; }

        public Dag10T()
        {
            baseList = Enumerable.Range(0, test ? 5 : 256).ToList();
            baseLengthList = baseList.Count();
            var input1 = InitializeKnotList();
            var input2 = InitializeASCIIKnotList();
            var answer1 = Calculate(input1, 1);
            var answer2 = Calculate(input2, 64);
            var denseBaseList = DenseHash(answer2);
            Answer1 = answer1.Take(2).Aggregate( (prev, curr) => prev * curr);
            Answer2 = denseBaseList.Select(x => x.ToString("x2")).Aggregate((prev, curr) => prev + curr).AsParallel().AsOrdered();
            
            WriteDebugAnswers(this);
        }

        private List<int> InitializeKnotList()
        {
            return text.Split(',').Select(c => Int32.Parse(c)).ToList();
        }

        private List<int> InitializeASCIIKnotList()
        {
           return  text.Select(c => (int)c).Concat(new List<int> { 17, 31, 73, 47, 23 }).ToList();
        }

        private List<int> DenseHash(List<int> hash)
        {
            //Take a block of 16
            var blocksize = 16;
            var blockSkip = 0;

            var denseBaseList = new List<int>();
            for (int i = 0; i < blocksize; i++)
            {
                var sparseHash = hash.Skip(blockSkip * blocksize).Take(blocksize).ToArray();

                //XOR together
                // A bitwise XOR takes two bit patterns of equal length and performs the logical exclusive OR operation on each pair of corresponding bits.
                //The result in each position is 1 if only the first bit is 1 or only the second bit is 1, but will be 0 if both are 0 or both are 1.
                //In this we perform the comparison of two bits, being 1 if the two bits are different, and 0 if they are the same.For example:
                //    0101(decimal 5)
                //XOR 0011(decimal 3)
                //  = 0110(decimal 6)

                var xorValue = 0;

                xorValue = sparseHash.Aggregate((prev, curr) => prev ^ curr);


                //foreach (var intVal in sparseHash)
                //{
                //    xorValue = xorValue ^ intVal;
                //}
                //denseBaseList.Add(xorValue);
                blockSkip++;
            }
            return denseBaseList;
        }

        public List<int> Calculate(List<int> knotLengthList, int rounds)
        {
            int index = 0;
            int skip = 0;
            for (int i = 0; i < rounds; i++)
            {
                foreach (var knotLength in knotLengthList)
                {
                    var knotListFromIndex = baseList.Skip(index).Take(amountToBeTaken(knotLength)).ToList();
                    var knotListFromBegin = baseList.Take(amountToBeTakenFromStartOfBaseList(knotLength)).ToList();

                    var joinedList = knotListFromIndex.Concat(knotListFromBegin).ToList();
                    joinedList.Reverse();

                    //Build new list
                    //When jumpedFromEnd
                    if (amountToBeTakenFromStartOfBaseList(knotLength) > 0)
                    {
                        var endPart = joinedList.Take(amountToBeTaken(knotLength)).ToList();
                        var beginPart = joinedList.Where(c => !endPart.Contains(c)).Take(amountToBeTakenFromStartOfBaseList(knotLength)).ToList();
                        var remainingUntouchedList = baseList.Where(c => !joinedList.Contains(c)).ToList();
                        baseList = beginPart.Concat(remainingUntouchedList).Concat(endPart).ToList();
                    }
                    //When in the middle?
                    else
                    {
                        var beginPart = baseList.Take(index).ToList();
                        var endPart = baseList.Where(c => !beginPart.Contains(c) && !joinedList.Contains(c)).ToList();
                        baseList = beginPart.Concat(joinedList).Concat(endPart).ToList();
                    }
                    WriteDebug<string>(string.Join(",", baseList.ToArray()), this);
                    UpdateIndexValue(knotLength);
                }
            }

            return baseList;
        }

        private void UpdateIndexValue(int knotlength)
        {
            //index = index + skip + knotlength;
            //if (index > baseLengthList) index = (index % baseLengthList);
            //skip++;
        }
    }
}
