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
    public class Day16 : AdventBase
    {
        public Day16()
        {
            // Test = true;
            PuzzleInput = Test ? Resources.Day16Example : Resources.Day16;
            ID = 16;
            Name = "Day 16: Chronal Classification";
        }

        public override string[] Solution()
        {
            return new string[] {
            };
        }

        private class Sample
        {
            public Register Before { get; set; }
            public Instruction Instruction { get; set; }
            public Register After { get; set; }
            public List<Opcodes> Opcodes { get; set; } = new List<Opcodes>();
        }

        private class Code
        {
            private Opcodes opcode;


            public int ID { get; set; }
            public int A { get; set; }
            public int B { get; set; }
            public int Output { get; set; }
        }

        private class Register : Code
        {
            public Register(string id, string a, string b, string output)
            {
                ID = Convert.ToInt32(id);
                A = Convert.ToInt32(a);
                B = Convert.ToInt32(b);
                Output = Convert.ToInt32(output);
            }
        }
        private class Instruction : Code
        {


            public Instruction(string id, string a, string b, string output)
            {
                ID = Convert.ToInt32(id);
                A = Convert.ToInt32(a);
                B = Convert.ToInt32(b);
                Output = Convert.ToInt32(output);
            }
        }



        public override string Part1()
        {
            PuzzleInput = @"Before: [3, 2, 1, 1]
9 2 1 2
After:  [3, 2, 2, 1]";

            //            PuzzleInput = @"Before: [1, 1, 0, 1]
            //0 1 0 1
            //After:  [1, 1, 0, 1]

            //Before: [2, 2, 2, 1]
            //2 1 2 2
            //After:  [2, 2, 1, 1]
            //";


            var lines = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None);
            var spacesAndNumbers = new Regex("[^0-9 \n]+");
            List<Sample> samples = new List<Sample>();

            var y = 0;
            var takeLines = true;
            while (takeLines)
            {

                var sample = new Sample();
                var allLines = lines.Skip(y * 4).Take(4).ToList();
                y++;

                if (!allLines.Any()) takeLines = false;

                foreach (var line in allLines)
                {
                    var firstChar = line.FirstOrDefault();

                    if (line.Length > 0)
                    {
                        switch (firstChar)
                        {
                            case 'B':
                                var before = spacesAndNumbers.Replace(line, "").Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                                var beforeRegister = new Register(before[0], before[1], before[2], before[3]);
                                sample.Before = beforeRegister;
                                break;
                            case 'A':
                                var after = spacesAndNumbers.Replace(line, "").Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                                var afterRegister = new Register(after[0], after[1], after[2], after[3]);
                                sample.After = afterRegister;
                                break;
                            default:
                                var codeArray = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                                var instruction = new Instruction(codeArray[0], codeArray[1], codeArray[2], codeArray[3]);
                                sample.Instruction = instruction;
                                break;
                        }
                    }


                }
                samples.Add(sample);
            }


            var opcodes = new List<Opcodes>();
            opcodes.Add(Addr);
            opcodes.Add(Addi);
            opcodes.Add(Mulr);
            opcodes.Add(Muli);

            //GenerateOpCodes();


            foreach (var sample in samples)
            {
                //try opcodes
                foreach (var opcode in opcodes)
                {
                    if (opcode(sample)) {
                        sample.Opcodes.Add(Addr);
                    };
                }

                //if correct, add opcode to instruction
                
            }

            return "";
        }

        private bool Addr(Sample sample)
        {
            var reg0 = sample.Before.ID;
            var reg1 = sample.Before.A;
            var reg2 = sample.Before.B;
            var reg3 = reg1 + reg2;

            if (reg0 == sample.After.ID && reg1 == sample.After.A && reg2 == sample.After.B && reg3 == sample.After.Output)
            {
                return true;
            }
            return false;
        }

        private bool Addi(Sample sample)
        {
            var reg0 = sample.Before.ID;
            var reg1 = sample.Before.A;
            var reg2 = sample.Before.B + sample.Instruction.B;
            var reg3 = sample.Before.Output;

            if (reg0 == sample.After.ID && reg1 == sample.After.A && reg2 == sample.After.B && reg3 == sample.After.Output)
            {
                return true;
            }
            return false;
        }

        private bool Mulr(Sample sample)
        {
            var reg0 = sample.Before.ID;
            var reg1 = sample.Before.A;
            var reg2 = sample.Before.B;
            var reg3 = reg1 * reg2;

            if (reg0 == sample.After.ID && reg1 == sample.After.A && reg2 == sample.After.B && reg3 == sample.After.Output)
            {
                return true;
            }
            return false;
        }
        private bool Muli(Sample sample)
        {
            var reg0 = sample.Before.ID;
            var reg1 = sample.Before.A;
            var reg2 = sample.Before.B;
            var reg3 = reg1 * sample.Instruction.B;

            if (reg0 == sample.After.ID && reg1 == sample.After.A && reg2 == sample.After.B && reg3 == sample.After.Output)
            {
                return true;
            }
            return false;
        }
        //private void GenerateOpCodes()
        //{
        //    var 
        //}

        delegate bool Opcodes(Sample sample);

        public override string Part2()
        {
            return "";
        }
    }
}

