using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventCode2017
{
    public class Dag18Program : AdventBase, AdventInterface
    {
        public static bool test = false;
        public string puzzleText = test ? Resources.dag18_2017_test : Resources.dag18_2017;
        public delegate int MyActionWrapper(Dictionary<string, decimal> vars, int index);

        public Dictionary<string, decimal> VariablesA { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> VariablesB { get; set; } = new Dictionary<string, decimal>();

        public List<(int, Duet)> DuetPairsA { get; set; } = new List<(int, Duet)>();
        public List<(int, Duet)> DuetPairsB { get; set; } = new List<(int, Duet)>();
        public static decimal soundHZ { get; set; } = 0;
        public static decimal recoverSounddHZ { get; set; } = 0;

        public static List<(int, decimal)> queueList = new List<(int, decimal)>();
        public static int countSend = 0;
        public static bool ALocked = false;
        public static bool BLocked = false;
        public static bool AFinished = false;
        public static bool BFinished = false;
        public static object mylock = new object();
        public static object mylock2 = new object();

        public Dag18Program(int id = 0, bool answerB = false)
        {
            //if (answerB) puzzleText = Resources.dag18_2017_sendReceive_test;
            recoverSounddHZ = 0;
            soundHZ = 0;
            WriteDebugAnswers(this);
        }

        public void CalculateAnswerA()
        {

            //using (StringReader reader = new StringReader(puzzleText))
            //{
            //    string line;
            //    var sequenceNumber = 0;
            //    while ((line = reader.ReadLine()) != null)
            //    {
            //        DuetPairs.Add(sequenceNumber++, new Duet(line, Programid));
            //    }
            //}

            //VariablesA = DuetPairs.Select(c => c.Value.DuetVar).Distinct().ToList().ToDictionary(c => c, c => (decimal)0);

            ////while (true)
            ////{
            ////    if (!queueList.Where(c => c.Item1 == Programid).Any()) continue;
            ////    Debug.WriteLine($"Program {Programid} : {index}");
            ////    var currentDuet = DuetPairs[index];
            ////    index = currentDuet.DuetAction(VariablesA, index);
            ////    index++;
            ////    if (index > DuetPairs.Count())
            ////    {
            ////        break;
            ////    }
            ////    //MySendQueue(8);

            ////    // recoverSounddHZ = currentDuet.recoverSounddHZ;
            ////}

            Debug.WriteLine(recoverSounddHZ);
            //return recoverSounddHZ;
        }

        public void CalculateAnswerB()
        {
            using (StringReader reader = new StringReader(puzzleText))
            {
                string line;
                var sequenceNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    var s = sequenceNumber++;
                    DuetPairsA.Add((s, new Duet(line, 0)));
                    DuetPairsB.Add((s, new Duet(line, 1)));
                }
            }

            VariablesA = DuetPairsA.Select(c => c.Item2.DuetVar).Distinct().ToList().ToDictionary(c => c, c => (decimal)0); //.Where(d => (!d.Value.DuetVar.Contains("snd") && !Int32.TryParse(d.Value.DuetVar, out int test)))
            VariablesA["p"] = 0;
            VariablesB= DuetPairsB.Select(c => c.Item2.DuetVar).Distinct().ToList().ToDictionary(c => c, c => (decimal)0); //.Where(d => (!d.Value.DuetVar.Contains("snd") && !Int32.TryParse(d.Value.DuetVar, out int test)))
            VariablesB["p"] = 1;

            var indexA = 0;
            var indexB = 0;

            while (true)
            {
                var currentDuetA = DuetPairsA[indexA];
                var currentDuetB = DuetPairsB[indexB];
                indexA = currentDuetA.Item2.DuetAction(VariablesA, indexA);
                indexB = currentDuetB.Item2.DuetAction(VariablesB, indexB);
                if (!ALocked) indexA++;
                if (!BLocked) indexB++;


                if (countSend % 10000 == 0)
                {
                    Debug.Write($"count {countSend} / ");
                    Debug.Write(string.Join(",", VariablesA));
                    Debug.WriteLine(" / " + string.Join(",", VariablesB));
                }

                if ((indexA >= DuetPairsA.Count() && indexB >= DuetPairsB.Count()) || (ALocked && BLocked))
                {
                    break;
                }
            }
            Debug.WriteLine(countSend);
        }

        public class Duet
        {
            public MyActionWrapper DuetAction { get; set; }
            public string DuetVar { get; set; }
            public int programID { get; set; }

            public Duet(string duet, int programID)
            {

                var duetActions = duet.Split(' ');
                DuetVar = duetActions[1];
                switch (duetActions[0])
                {
                    case "snd":
                        DuetAction = (Variables, i) =>
                        {
                            soundHZ = GetVal(duetActions[1], Variables);
                            lock (mylock) {
                                queueList.Add((programID == 0 ? 1 : 0, GetVal(duetActions[1], Variables)));
                            }
                            if (programID == 0) { if (!BFinished) BLocked = false; }
                            else {
                                if (!AFinished) ALocked = false; countSend += 1;
                            }
                            return i;
                        };
                        break;
                    case "set":
                        DuetAction = (Variables, i) =>
                        {
                            Variables[duetActions[1]] = GetVal(duetActions[2], Variables);
                            return i;
                        };
                        break;
                    case "add":
                        DuetAction = (Variables, i) =>
                        {
                            Variables[duetActions[1]] += GetVal(duetActions[2], Variables);
                            return i;
                        };
                        break;
                    case "mul":
                        DuetAction = (Variables, i) =>
                        {
                            Variables[duetActions[1]] *= GetVal(duetActions[2], Variables);
                            return i;
                        };
                        break;
                    case "mod":
                        DuetAction = (Variables, i) =>
                        {
                            Variables[duetActions[1]] %= GetVal(duetActions[2], Variables);
                            return i;
                        };
                        break;
                    case "rcv":

                        DuetAction = (Variables, i) =>
                        {
                            if (!queueList.Where(c => c.Item1 == programID).Any())
                            {
                                if (programID == 0) { ALocked = true; }
                                else { BLocked = true; }
                            }

                            if (queueList.Where(c => c.Item1 == programID).Any())
                            {
                                var receiveValue = queueList.Where(c => c.Item1 == programID).Take(1).FirstOrDefault();
                                lock (mylock2)
                                {
                                    queueList = queueList.Where(c => c.Item1 == programID).Skip(1).ToList().Concat(queueList.Where(c => c.Item1 != programID).ToList()).ToList();
                                }
                                
                                Variables[duetActions[1]] = receiveValue.Item2;
                            }

                            if (BLocked && ALocked) i = 999999999;
                            return i;
                        };
                        break;
                    case "jgz":
                        DuetAction = (Variables, i) =>
                        {
                            if (GetVal(duetActions[1], Variables) > 0)
                            {
                                var skip = GetVal(duetActions[2], Variables);
                                i += (int)skip + -1;
                            }
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
