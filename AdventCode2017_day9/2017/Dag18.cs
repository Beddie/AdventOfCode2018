using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AdventCode2017
{
    public class Dag18 : AdventBase, AdventInterface
    {
        private static bool test = false;
        private string puzzleText = test ? Resources.dag18_2017_test : Resources.dag18_2017;
        public delegate void MyActionWrapper();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern void Beep(uint dwFreq, uint dwDuration);

        public static Dictionary<string, long> Variables { get; set; } = new Dictionary<string, long>();

        public Dictionary<int, Duet> DuetPairs { get; set; } = new Dictionary<int, Duet>();
        private static int index { get; set; } = 0;
        private static long soundHZ { get; set; } = 0;
        private static long recoverSounddHZ { get; set; } = 0;

        public Dag18()
        {
            CalculateAnswerA();
            CalculateAnswerB();
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
                    DuetPairs.Add(sequenceNumber++, new Duet(line));
                }
            }

            Variables = DuetPairs.Select(c => c.Value.DuetVar).Distinct().ToList().ToDictionary(c => c, c => (long)0);

            while (true)
            {
                var currentDuet = DuetPairs[index];
                currentDuet.DuetAction();
                index++;
                if (index > DuetPairs.Count()) break;
            }

            Debug.WriteLine(recoverSounddHZ);
        }

        public class Duet
        {
            public MyActionWrapper DuetAction { get; set; }
            public string DuetVar { get; set; }

            public Duet(string duet)
            {
                var duetActions = duet.Split(' ');
                DuetVar = duetActions[1];
                switch (duetActions[0])
                {
                    case "snd":
                        DuetAction = () => {
                            soundHZ = GetVal(duetActions[1]);
                        };
                        //Beep(Convert.ToUInt16(GetVal(duetActions[1])), Convert.ToUInt16("500")); };
                        break;
                    case "set":
                        DuetAction = () => {
                            Variables[duetActions[1]] = GetVal(duetActions[2]);
                        };
                        break;
                    case "add":
                        DuetAction = () => {
                            Variables[duetActions[1]] += GetVal(duetActions[2]);
                        };
                        break;
                    case "mul":
                        DuetAction = () => {
                            Variables[duetActions[1]] *= GetVal(duetActions[2]);
                        };
                        break;
                    case "mod":
                        DuetAction = () => {
                            Variables[duetActions[1]] %= GetVal(duetActions[2]);
                        };
                        break;
                    case "rcv":
                        DuetAction = () =>
                        {
                            if (GetVal(duetActions[1]) != 0)
                            {
                                recoverSounddHZ = soundHZ;
                                index = 9999999;
                            }
                        };
                        break;
                    case "jgz":
                        DuetAction = () =>
                        {
                            if (GetVal(duetActions[1]) > 0)
                            {
                                var skip = GetVal(duetActions[2]);
                                index += (int)skip + -1;
                            }
                        };
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private static long GetVal(string value)
        {
            if (long.TryParse(value, out long intVarValue))
            {
                return intVarValue;
            }
            else
            {
                return Variables[value];
            }

        }

        public void CalculateAnswerB()
        {
        }
    }
}
