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
    public class Day7 : AdventBase
    {
        public Day7()
        {
            //Test = true;
            PuzzleInput = Test ? Resources.Day7Example : Resources.Day7;
            ID = 7;
            Name = "Day 7: The Sum of Its Parts";
        }

        public override string[] Solution()
        {
            return new string[] {
                "CFMNLOAHRKPTWBJSYZVGUQXIDE",
                "971"
            };
        }

        private List<char> orderedInstructions = new List<char>();
        private List<char[]> instructions = new List<char[]>();

        public override string Part1()
        {
            instructions = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => new char[2] { c.Skip(5).Take(1).First(), c.Skip(36).Take(1).First() }).ToList();
            var allChars = instructions.Select(c => c[0]).Union(instructions.Select(c => c[1])).Distinct().OrderBy(c => c).ToList();

            while (allChars.Count() > 0)
            {
                foreach (var letter in allChars)
                {
                    var avail = ProcessAvailables(letter);
                    if (avail)
                    {
                        allChars.Remove(letter);
                        break;
                    }
                }
            }
            var charOrder = new string(orderedInstructions.ToArray());
            return charOrder;
        }

        private bool ProcessAvailables(char available)
        {
            var anchestorAvailables = instructions.Where(c => c[1] == available).Select(c => c[0]).ToList();
            var isAvailable = anchestorAvailables.TrueForAll(c => orderedInstructions.Contains(c));
            if (!isAvailable) return false;

            //Is available!
            if (!orderedInstructions.Contains(available)) orderedInstructions.Add(available);
            return true;
        }

        #region Part 2

        private class InstuctionLetter
        {
            public char Letter { get; set; }
            public int Seconds { get; set; } = 0;
            public bool WorkerDone { get; set; }
            public bool Available { get; set; }
            public bool Done => WorkerDone && Available;
            public int Order { get; set; }
            public bool InProcess { get; set; }
        }

        private class Worker
        {
            public InstuctionLetter InstuctionLetter = new InstuctionLetter() { Seconds = -1 };
            public bool Working => InstuctionLetter.Seconds > 0;

            public void ProcessAvailables(InstuctionLetter available)
            {
                InstuctionLetter = available;
                InstuctionLetter.InProcess = true;
            }

            public void Work()
            {
                InstuctionLetter.Seconds -= (InstuctionLetter.Seconds > 0 ? 1 : 0);
                if (InstuctionLetter.Seconds == 0)
                {
                    InstuctionLetter.WorkerDone = true;
                    InstuctionLetter.InProcess = false;
                }
            }
        }

        private List<InstuctionLetter> allChars = new List<InstuctionLetter>();
        private List<Worker> workers { get; set; } = Test ? new List<Worker>() { new Worker(), new Worker() } : new List<Worker>() { new Worker(), new Worker(), new Worker(), new Worker(), new Worker() };

        public override string Part2()
        {
            instructions = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => new char[2] { c.Skip(5).Take(1).First(), c.Skip(36).Take(1).First() }).ToList();
            allChars = instructions.Select(c => c[0]).Union(instructions.Select(c => c[1])).Distinct().OrderBy(c => c).Select((d) => new InstuctionLetter { Letter = d, Seconds = d - (Test ? 64 : 4) }).ToList();
            var seconds = 0;

            while (allChars.Any(c => !c.Done))
            {
                //Set Available letter properties
                SetAvailablePropetyToAllAvailableLetters();

                foreach (var letter in allChars.OrderBy(o => o.Letter).Where(c => !c.Done && c.Available && !c.InProcess && !c.WorkerDone))
                {
                    var worker = workers.Where(c => !c.Working).FirstOrDefault();
                    if (worker != null) worker.ProcessAvailables(letter);
                }

                //Let the Elves do their work
                workers.ForEach(c => c.Work());

                if (Test)
                {
                    Debug.WriteLine(string.Format("{0} AFTER WORK    DONE:{1}    PROCESS:{2}", seconds, new string(allChars.Where(c => c.Done).OrderBy(c => c.Order).Select(c => c.Letter).ToArray()), new string(allChars.Where(c => c.InProcess).OrderBy(c => c.Order).Select(c => c.Letter).ToArray())));
                    foreach (var myChar in allChars.OrderBy(o => o.Letter).Where(c => c.Done && c.Order == 0)) myChar.Order = allChars.Max(c => c.Order) + 1;
                }
                seconds++;
            }

            return seconds.ToString();
        }

        private void SetAvailablePropetyToAllAvailableLetters()
        {
            foreach (var chartic in allChars.OrderBy(o => o.Letter).Where(c => !c.Available))
            {
                chartic.Available = instructions.Where(c => c[1] == chartic.Letter).Select(c => c[0]).All(c => allChars.OrderBy(o => o.Letter).Where(d => d.Letter == c).First().WorkerDone);
            }
        }


        #endregion
    }
}