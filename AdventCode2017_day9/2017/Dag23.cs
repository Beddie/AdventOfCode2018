using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AdventCode2017
{
    public class Dag23 : AdventBase, AdventInterface
    {
        public static bool test = false;
        public string puzzleText = test ? Resources.dag18_2017_test : Resources.dag23_2017;
        public delegate int MyActionWrapper(Dictionary<string, decimal> vars, int index);

        public Dictionary<string, decimal> VariablesA { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> VariablesB { get; set; } = new Dictionary<string, decimal>();

        public List<(int, Duet)> DuetPairsA { get; set; } = new List<(int, Duet)>();
        public List<(int, Duet)> DuetPairsB { get; set; } = new List<(int, Duet)>();
        public static decimal countMul { get; set; } = 0;

        public Dag23()
        {
           // recoverSounddHZ = 0;
           // soundHZ = 0;
            CalculateAnswerA();
            WriteDebugAnswers(this);
        }

        public void CalculateAnswerA()
        {

            using (StringReader reader = new StringReader(puzzleText))
            {
                string line;
                var sequenceNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    DuetPairsA.Add((sequenceNumber++, new Duet(line, 0)));
                }
            }

            VariablesA = DuetPairsA.Select(c => c.Item2.DuetVar).Distinct().ToList().ToDictionary(c => c, c => (decimal)0);
            VariablesA["a"] = 1;
            var indexA = 0;
            var counter = 0;
            while (indexA < DuetPairsA.Count())
            {
                counter++;
                var currentDuet = DuetPairsA[indexA];
                indexA = currentDuet.Item2.DuetAction(VariablesA, indexA);
                indexA++;
            }
            Debug.WriteLine($"amount of multiply: {countMul}, in {counter} iterations");
        }

        public void CalculateAnswerB()
        {
            throw new NotImplementedException();
        }

        public class Duet
        {
            public MyActionWrapper DuetAction { get; set; }
            public string DuetVar { get; set; }

            public Duet(string duet, int programID)
            {
                var duetActions = duet.Split(' ');
                DuetVar = duetActions[1];
                switch (duetActions[0])
                {
                    case "jnz":
                        DuetAction = (Variables, i) =>
                        {
                            if (GetVal(duetActions[1], Variables) != 0)
                            {
                                var skip = GetVal(duetActions[2], Variables);
                                i += (int)skip + -1;
                            }
                            return i;
                        };
                        break;
                    case "set":
                        DuetAction = (Variables, i) =>
                        {
                           
                            Variables[duetActions[1]] = GetVal(duetActions[2], Variables);
                            if (duetActions[1] == "h") Debug.WriteLine($"Var h= {GetVal(duetActions[2], Variables)} {DateTime.Now}");
                            return i;
                        };
                        break;
                    case "sub":
                        DuetAction = (Variables, i) =>
                        {
                            Variables[duetActions[1]] -= GetVal(duetActions[2], Variables);
                            return i;
                        };
                        break;
                    case "mul":
                        
                        DuetAction = (Variables, i) =>
                        {
                            countMul++;
                            Variables[duetActions[1]] *= GetVal(duetActions[2], Variables);
                            return i;
                        };
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            private decimal GetVal(string value, Dictionary<string, decimal> vars)
            {
                if (decimal.TryParse(value, out decimal intVarValue))
                {
                    return intVarValue;
                }
                else
                {
                    return vars[value];
                }

            }
        }
    }
}
