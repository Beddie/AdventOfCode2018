using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventCode2017
{
    public class Dag15 : AdventBase, AdventInterface
    {
        private static bool test = false;
        private class GeneratorValue { public double GeneratorA { get; set; } public double GeneratorB { get; set; } }
        private List<GeneratorValue> baseList = test ? new List<GeneratorValue>() { new GeneratorValue() { GeneratorA = 65, GeneratorB = 8921 } } : new List<GeneratorValue>() { new GeneratorValue() { GeneratorA = 883, GeneratorB = 879 } };// List basically became useless during development, TODO refactor
        private List<string> genReadyBinaryCompareStringA = new List<string>();
        private List<string> genReadyBinaryCompareStringB = new List<string>();
        private const double generatorAFactor = 16807;
        private const double generatorBFactor = 48271;
        private const double dividerA = 4;
        private const double dividerB = 8;
        //and then keep the remainder of dividing that resulting product by 2147483647. That final remainder is the value it produces next.
        private const double dividingFactor = 2147483647d;
        private const int loopAmount = 5000000;

        public void CalculateAnswerB()
        {
            var genA = baseList.First().GeneratorA;
            var genB = baseList.First().GeneratorB;

            while (genReadyBinaryCompareStringB.Count() < loopAmount || genReadyBinaryCompareStringA.Count() < loopAmount)
            {
                genA = pushGenerator(genA, generatorAFactor);
                genB = pushGenerator(genB, generatorBFactor);

                EvaluateAndAddBinaryCode(genA, dividerA, genReadyBinaryCompareStringA);
                EvaluateAndAddBinaryCode(genB, dividerB, genReadyBinaryCompareStringB);
            }

            var matchCOunt = 0;
            for (int i = 0; i < (test ? loopAmount : loopAmount); i++)
            {
                matchCOunt = genReadyBinaryCompareStringA[i] == genReadyBinaryCompareStringB[i] ? matchCOunt + 1 : matchCOunt;
            }
            Debug.WriteLine(matchCOunt);

        }

        private void EvaluateAndAddBinaryCode(double gen, double divider, List<string> genlist)
        {
            if (gen % divider == 0)
            {
                var newGebn = gen.ConvertDoubleToBinaryString();
                genlist.Add(newGebn.Substring(newGebn.Length - 16));
            }
        }

        private static double pushGenerator(double gen, double generatorAFactor)
        {
            return ((gen * generatorAFactor) % dividingFactor);
        }

        public void CalculateAnswerA()
        {
            var genA = baseList.First().GeneratorA;
            var genB = baseList.First().GeneratorB;
            var matchCOunt = 0;
            for (int i = 0; i < (test ? 5000000 : 5000000); i++)
            {
                genA = ((genA * generatorAFactor) % 2147483647d);
                genB = ((genB * generatorBFactor) % 2147483647d);

                var a = genA.ConvertDoubleToBinaryString();
                var b = genB.ConvertDoubleToBinaryString();
                matchCOunt = a.Substring(a.Length - 16) == b.Substring(a.Length - 16) ? matchCOunt + 1 : matchCOunt;
            }
            Debug.WriteLine(matchCOunt);
        }

        public Dag15()
        {
            CalculateAnswerA();
            WriteDebugAnswers(this);
        }

        public byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public String ToBinary(Byte[] data)
        {
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }

    }
}
