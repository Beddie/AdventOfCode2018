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

        private int remainingLengthToEndOfBaseList { get { return baseLengthList - index; } }
        private int amountToBeTaken(int knotlength) { return knotlength > remainingLengthToEndOfBaseList ?  remainingLengthToEndOfBaseList : knotlength; }
        private int amountToBeTakenFromStartOfBaseList(int knotlength) { var usedAtTheEnd = knotlength - remainingLengthToEndOfBaseList; return usedAtTheEnd < 0 ? 0 : usedAtTheEnd; }

        public Dag10()
        {
            InitializeBaseList();
            InitializeKnotList();
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

        public void Calculate()
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
                
                //CalculateNewIndex
                UpdateIndexValue(knotLength);
                WriteDebug<string>(string.Join(",", baseList.ToArray()), this);
            }


            Answer1 = baseList.Take(1).First() * baseList.Skip(1).First();
            //Answer2 = garbageScore;
        }

        private void UpdateIndexValue(int knotlength)
        {
            index = index + skip + knotlength;
            if (index > baseLengthList) index = index - baseLengthList;
            skip++;
        }


    }
}
