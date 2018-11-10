using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCode2017
{
    public class Dag10 : AdventBase, AdventInterface
    {
        public Dag10()
        {
            Calculate();
            WriteDebugAnswers(this);
        }

        public void Calculate()
        {
            Test = true;
            string text = Test ? "3, 4, 1, 5" : Resources.dag10_2017;

            var baseList = new List<int>();
            var index = 0;
            var skip = 0;

            for (int i = 0; i < (Test ? 5 : 256); i++)
            {
                baseList.Add(i);
            }

            var knotLengthList = (text.Split(',')).Select(c=> Int32.Parse(c));
            foreach (var knotLength in knotLengthList)
            {
                
                var endListLength = baseList.Count() - index;
                var takeFromBegin = 0;
                var takeLengthStartFromIndex = knotLength;
                var takeToIndex = index + knotLength;
                var isStartedOverAgain = false;
                var isInTheMiddle = false;
                if (knotLength > endListLength) {
                    isStartedOverAgain = true;
                    takeLengthStartFromIndex = endListLength;
                    takeFromBegin = knotLength - endListLength;
                }
                if (index > 0 && !isStartedOverAgain) {
                    isInTheMiddle = true;
                    index = index - 1;
                }
                var knotListFromIndex  = baseList.Skip(index).Take(takeLengthStartFromIndex).ToList();
                var knotListFromBegin = baseList.Take(takeFromBegin).ToList();

                var joinedList = knotListFromBegin.Concat(knotListFromIndex).ToList();
                joinedList.Reverse();
                //place reverserdList back into position. When 
                var restList = baseList.Where(c => !joinedList.Contains(c)).ToList();
                if (isStartedOverAgain)
                {
                    var beginPart = joinedList.Take(takeFromBegin);
                    baseList = beginPart.Concat(restList).Concat(joinedList.Where(c => !beginPart.Contains(c))).ToList();
                    //baseList = joinedList.Take(takeFromBegin).Concat(restList).Concat(joinedList.Skip(index).Take(takeLengthStartFromIndex)).ToList();
                }
                else if (isInTheMiddle){
                    var beginPart = restList.Take(index);
                    var endPart = restList.Skip(index + joinedList.Count() - 1);
                    //var endPart = baseList.Where(c => !beginPart.Contains(c) && !restList.Contains(c)).ToList();
                    baseList = beginPart.Concat(joinedList).Concat(endPart).ToList();
                }
                else {
                    baseList = joinedList.Concat(restList).ToList();
                }
                
                
                
                
                index = index + knotLength + skip;
                if (index > baseList.Count()) { index = index - baseList.Count(); }
                skip++;
                WriteDebug<string>(string.Join(",",baseList.ToArray()), this);

            }

            //Answer1 = score;
            //Answer2 = garbageScore;
        }

    }
}
