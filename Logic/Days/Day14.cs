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
    public class Day14 : AdventBase
    {
        public Day14()
        {
            //Test = true;
            PuzzleInput = Test ? Resources.Day14Example : Resources.Day14;
            ID = 14;
            Name = "Day 14: Chocolate Charts";
        }

        public static List<int> recipes { get; set; } = new List<int>();
        public List<Elf> Elfs { get; set; } = new List<Elf>();
        public static StringBuilder recipesBuilder { get; set; } = new StringBuilder();
        public ElfPart2[] ElfsPart2 { get; set; }

        public class Elf
        {
            public Elf(int id, int index)
            {
                ID = id;
                Index = index;
            }

            public int ID { get; set; }
            public int Index { get; set; }

            public void Move()
            {
                Index = (Index + (recipes[Index] + 1)) % (recipes.Count);
            }
        }

        //TODO refactor Elf class with other Elf class
        public class ElfPart2
        {
            public ElfPart2(int id, int index)
            {
                ID = id;
                Index = index;
            }

            public int ID { get; set; }
            public int Index { get; set; }
            public int RecipeNumber { get; set; }

            public void Move()
            {
                Index = (Index + (RecipeNumber + 1)) % (recipesBuilder.Length);
                RecipeNumber = Convert.ToInt16(recipesBuilder[Index].ToString());
            }
        }

        public override string[] Solution()
        {
            return new string[] {
                "2111113678",
                "20195114"
            };
        }

        public override string Part1()
        {
            recipes.Add(3);
            recipes.Add(7);
            Elfs.Add(new Elf(1, 0));
            Elfs.Add(new Elf(2, 1));

            var recipesGoal = (Test ? 2018 : 286051);
            var score = (long?)null;
            while (!score.HasValue)
            {
                foreach (var elf in Elfs)
                {
                    elf.Move();
                }
                if (Test) Print();
                var sumElfRecipes = Elfs.Sum(c => recipes[c.Index]);
                foreach (var sumElfs in sumElfRecipes.ToString()) recipes.Add((byte)Convert.ToInt32(sumElfs.ToString()));

                if ((recipesGoal + 10) <= recipes.Count - 1)
                {
                    score = Convert.ToInt64(string.Join("", recipes.Skip(recipesGoal).Take(10).ToArray()));
                }
            }
            return score.ToString();
        }

        //TODO Performance increase
        public override string Part2()
        {
            //Test = true;
            recipesBuilder.Append(3);
            recipesBuilder.Append(7);
            ElfsPart2 = new ElfPart2[] { new ElfPart2(1, 0), new ElfPart2(2, 1) };

            var recipesGoal = (Test ? "01245" : "286051");
            var goalLength = recipesGoal.Length;
            var score = (long?)null;
            var count = 0;
            while (!score.HasValue)
            {
                count++;
                ElfsPart2[0].Move();
                ElfsPart2[1].Move();
                if (Test) PrintPart2();
                recipesBuilder.Append(ElfsPart2[0].RecipeNumber + ElfsPart2[1].RecipeNumber);

                if (count % 20000000 == 0)
                {
                    var tt = recipesBuilder.ToString().IndexOf(recipesGoal);

                    if (tt > 0)
                    {
                        var stringLength = recipesBuilder.ToString().Substring(0, recipesBuilder.ToString().IndexOf(recipesGoal));// TakeWhile(c=> c == recipesGoal.ToString());
                        score = stringLength.Length;
                    }
                }
            }
            return score.ToString();
        }

        private void Print()
        {
            var sb = new StringBuilder();
            var elfIndex = Elfs.Select(c => c.Index).ToArray();
            for (int i = 0; i < recipes.Count; i++)
            {
                if (elfIndex.Contains(i))
                {
                    var currentElf = Elfs.Where(c => c.Index == i).First();

                    var char1 = (currentElf.ID == 1) ? "(" : "[";
                    var char2 = (currentElf.ID == 1) ? ")" : "]";

                    sb.Append($"{char1}{recipes[i]}{char2}");
                }
                else
                {
                    sb.Append($" {recipes[i]} ");
                }
            }
            Debug.WriteLine(sb.ToString());
        }

        private void PrintPart2()
        {
            var sb = new StringBuilder();
            var elfIndex = ElfsPart2.Select(c => c.Index).ToArray();
            for (int i = 0; i < recipesBuilder.Length; i++)
            {
                if (elfIndex.Contains(i))
                {
                    var currentElf = ElfsPart2.Where(c => c.Index == i).First();

                    var char1 = (currentElf.ID == 1) ? "(" : "[";
                    var char2 = (currentElf.ID == 1) ? ")" : "]";

                    sb.Append($"{char1}{recipesBuilder[i]}{char2}");
                }
                else
                {
                    sb.Append($" {recipesBuilder[i]} ");
                }
            }
            Debug.WriteLine(sb.ToString());
        }
    }