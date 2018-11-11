using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCode2017
{
    public class Dag10 : AdventBase, AdventInterface
    {
        private static bool test = false;
        private static int baseListLength = (test ? 5 : 256);
        private string text = test ? "3, 4, 1, 5" : Resources.dag10_2017;

        private int baseLengthList { get { return baseList.Count(); } }
        private List<int> baseList = new List<int>();
        private List<int> knotLengthList = new List<int>();
        private int index = 0;
        private int skip = 0;
        private int rounds = 64;

        private int remainingLengthToEndOfBaseList { get { return baseLengthList - index; } }
        private int amountToBeTaken(int knotlength) { return knotlength > remainingLengthToEndOfBaseList ? remainingLengthToEndOfBaseList : knotlength; }
        private int amountToBeTakenFromStartOfBaseList(int knotlength) { var usedAtTheEnd = knotlength - remainingLengthToEndOfBaseList; return usedAtTheEnd < 0 ? 0 : usedAtTheEnd; }

        public Dag10()
        {
            InitializeBaseList();
            InitializeASCIIKnotList();
            Calculate();
            WriteDebugAnswers(this);
        }

        private void InitializeBaseList()
        {
            for (int i = 0; i < baseListLength; i++)
            {
                baseList.Add(i);
            }
        }

        private void InitializeKnotList()
        {
            knotLengthList = (text.Split(',')).Select(c => Int32.Parse(c)).ToList();
        }

        private void InitializeASCIIKnotList()
        {
            var knotCharList = text.Select(c => c).ToList();
            foreach (var charItem in knotCharList)
            {
                knotLengthList.Add((int)charItem);
            }
            knotLengthList.Add(17);
            knotLengthList.Add(31);
            knotLengthList.Add(73);
            knotLengthList.Add(47);
            knotLengthList.Add(23);
        }


        public void Calculate()
        {
           
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

            //Take a block of 16
            var bLocksize = 16;
            var blockSkip = 0;

            var denseBaseList = new List<int>();
            for (int i = 0; i < bLocksize; i++)
            {
                var sparseHash = baseList.Skip(blockSkip * bLocksize).Take(bLocksize).ToList();

                //XOR together
                // A bitwise XOR takes two bit patterns of equal length and performs the logical exclusive OR operation on each pair of corresponding bits.
                //The result in each position is 1 if only the first bit is 1 or only the second bit is 1, but will be 0 if both are 0 or both are 1.
                //In this we perform the comparison of two bits, being 1 if the two bits are different, and 0 if they are the same.For example:
                //    0101(decimal 5)
                //XOR 0011(decimal 3)
                //  = 0110(decimal 6)

                var xorValue = 0;
                foreach (var intVal in sparseHash)
                {
                    xorValue = xorValue ^ intVal;
                }
                denseBaseList.Add(xorValue);
                blockSkip++;
            }

            var hexList = new List<string>();
            foreach (var denseVal in denseBaseList)
            {
                var hexVal = denseVal.ToString("X");
                hexVal = hexVal.Length == 1 ? string.Format("0{0}", hexVal): hexVal;
                hexList.Add(hexVal);
            }

            var hashValue = string.Join("", hexList.ToArray()).ToLower();
            Answer1 = baseList.Take(1).First() * baseList.Skip(1).First();
            Answer2 = hashValue;
        }

        private void UpdateIndexValue(int knotlength)
        {
            index = index + skip + knotlength;
            if (index > baseLengthList) index = (index % baseLengthList);
            skip++;
        }


    }
}
