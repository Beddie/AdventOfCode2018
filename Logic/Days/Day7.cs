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
    public class Day7 : AdventBase, AdventInterface
    {
        public Day7()
        {
            Test = true;
            PuzzleInput = Test ? Resources.Day7Example : Resources.Day7;
        }

        public string[] Solution()
        {
            return new string[] {
                "CFMNLOAHRKPTWBJSYZVGUQXIDE",
                ""
            };
        }

        private List<char> orderedInstructions = new List<char>();
        private List<char[]> instructions = new List<char[]>();
        private HashSet<char> queue = new HashSet<char>();
        //private List<char> availables = new List<char>();

        public string Part1()
        {
            instructions = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => new char[2] { c.Skip(5).Take(1).First(), c.Skip(36).Take(1).First() }).ToList();
            var allChars = instructions.Select(c => c[0]).Union(instructions.Select(c => c[1])).Distinct().OrderBy(c => c).ToList();
            var iteratingAvailables = allChars.Where(c => !instructions.Select(d => d[1]).Contains(c)).OrderBy(c => c).ToList();

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

        private class Worker
        {
            public char Letter { get; set; }
            public int Seconds { get; set; }
        }

        private List<Worker> workers { get; set; } = new List<Worker>() { new Worker(), new Worker()};

        public string Part2()
        {
            instructions = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => new char[2] { c.Skip(5).Take(1).First(), c.Skip(36).Take(1).First() }).ToList();
            var allChars = instructions.Select(c => c[0]).Union(instructions.Select(c => c[1])).Distinct().OrderBy(c => c).Select((d, index) => new { thisChar = d, seconds = index + 1 }).ToList();
            var startingInstructions = allChars.Where(c => !instructions.Where(d => d[1] == c.thisChar).Any()).Select(c=> c.thisChar).ToList();
            var iteratingAvailables = instructions.Where(c => startingInstructions.Contains(c[0])).ToList();
            var seconds = 0;

            while (orderedInstructions.Count < allChars.Count && workers.All(c => c.Seconds == 0))
            {
                devideInstructies(iteratingAvailables.Select(c => c[1]));
                foreach (var letter in iteratingAvailables)
                {
                    var instructies = instructions.Where(c => c[0] == letter[0]).ToList();
                        //seconds += letter.seconds;
                        var avail = ProcessAvailablesPart2(instructies);
                        if (avail)
                        {
                            break;
                        }
                }
            }
            var charOrder = new string(orderedInstructions.ToArray());
            return charOrder;
        }

        private void devideInstructies(IEnumerable<char> instructies)
        {
            //var workedInstructies= new List<Worker>();
            foreach (var instructie in instructies)
            {
                foreach (var worker in workers)
                {
                    if (worker.Seconds == 0) { 
                        worker.Letter = instructie;
                        worker.Seconds = instructie - 64;
                        break;
                    };
                }
            }
        }

        private bool ProcessAvailablesPart2(List<char[]> instructies)
        {
            //if (orderedInstructions.Contains(available)) return false;
            //var anchestorAvailables = instructions.Where(c => c[1] == available).Select(c => c[0]).ToList();
            //var isAvailable = anchestorAvailables.TrueForAll(c => orderedInstructions.Contains(c));
            //if (!isAvailable) return false;

            ////Is available!
            ////if (!orderedInstructions.Contains(available))
            //orderedInstructions.Add(available);
            return true;
        }

        public string GetListName()
        {
            return "Day 7";
        }

        public int GetID()
        {
            return 7;
        }
    }
}

