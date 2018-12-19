using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

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
                "531","649"
            };
        }

        private class Sample
        {
            public Register Before { get; set; }
            public Instruction Instruction { get; set; }
            public Register After { get; set; } = new Register(0, 0, 0, 0);
            public List<(int, Opcodes)> Opcodes { get; set; } = new List<(int, Opcodes)>();
            public List<(int, string)> OpcodesString { get; set; } = new List<(int, string)>();

            public int[] UsedCodes => Opcodes.Select(x => x.Item1).ToArray();
        }

        private class Code
        {
            private Opcodes opcode;


            public int OpcodeID { get; set; }
            public int InputA { get; set; }
            public int InputB { get; set; }
            public int OutputC { get; set; }
        }

        private class Register
        {
            public int[] registers { get; set; }

            public Register(string id, string a, string b, string output)
            {
                registers = new[] { Convert.ToInt32(id), Convert.ToInt32(a), Convert.ToInt32(b), Convert.ToInt32(output) };
            }
            public Register(int id, int a, int b, int output)
            {
                registers = new[] { id, a, b, output };
            }
        }
        private class Instruction : Code
        {


            public Instruction(string id, string a, string b, string output)
            {
                OpcodeID = Convert.ToInt32(id);
                InputA = Convert.ToInt32(a);
                InputB = Convert.ToInt32(b);
                OutputC = Convert.ToInt32(output);
            }
        }



        public override string Part1()
        {
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
                var countEndInstructions = 0;
                foreach (var line in allLines)
                {
                    var firstChar = line.FirstOrDefault();

                    if (line.Length > 0)
                    {
                        countEndInstructions = 0;
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
                    else
                    {
                        countEndInstructions++;
                    }

                    if (countEndInstructions > 2) break;
                }

                if (sample.Before != null)
                {
                    samples.Add(sample);
                };
            }


            var opcodes = new List<Opcodes>();
            opcodes.Add(Addr);
            opcodes.Add(Addi);
            opcodes.Add(Mulr);
            opcodes.Add(Muli);
            opcodes.Add(Banr);
            opcodes.Add(Bani);
            opcodes.Add(Borr);
            opcodes.Add(Bori);
            opcodes.Add(Setr);
            opcodes.Add(Seti);
            opcodes.Add(Gtir);
            opcodes.Add(Gtri);
            opcodes.Add(Gtrr);

            opcodes.Add(Eqir);
            opcodes.Add(Eqri);
            opcodes.Add(Eqrr);

            foreach (var sample in samples)
            {
                //try opcodes
                foreach (var opcode in opcodes)
                {
                    if (opcode(sample))
                    {
                        sample.OpcodesString.Add((sample.Instruction.OpcodeID, opcode.Method.ToString()));
                    };
                }
                //if correct, add opcode to instruction

            }
            var part1 = samples.Where(c => c.Opcodes.Count() > 2).Count().ToString();
            return part1;
        }

        //addr(add register) stores into register C the result of adding register A and register B.
        private bool Addr(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] + check[B];

            return (check.SequenceEqual(sample.After.registers));

        }

        //addi(add immediate) stores into register C the result of adding register A and value B.
        private bool Addi(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] + B;

            return (check.SequenceEqual(sample.After.registers));
        }

        //mulr (multiply register) stores into register C the result of multiplying register A and register B.
        private bool Mulr(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] * check[B];

            return (check.SequenceEqual(sample.After.registers));
        }

        //muli (multiply immediate) stores into register C the result of multiplying register A and value B.
        private bool Muli(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] * B;

            return (check.SequenceEqual(sample.After.registers));
        }

        //banr (bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
        private bool Banr(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] & check[B];

            return (check.SequenceEqual(sample.After.registers));
        }

        //bani(bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
        private bool Bani(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] & B;

            return (check.SequenceEqual(sample.After.registers));
        }

        //borr (bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
        private bool Borr(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] | check[B];

            return (check.SequenceEqual(sample.After.registers));
        }

        //bori(bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
        private bool Bori(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] | B;

            return (check.SequenceEqual(sample.After.registers));
        }

        //setr (set register) copies the contents of register A into register C. (Input B is ignored.)
        private bool Setr(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var C = sample.Instruction.OutputC;
            check[C] = check[A];

            return (check.SequenceEqual(sample.After.registers));
        }

        //seti(set immediate) stores value A into register C. (Input B is ignored.)
        private bool Seti(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var C = sample.Instruction.OutputC;
            check[C] = A;

            return (check.SequenceEqual(sample.After.registers));
        }


        //gtir(greater-than immediate/register) sets register C to 1 if value A is greater than register B.Otherwise, register C is set to 0.

        private bool Gtir(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = A > check[B] ? 1 : 0;

            return (check.SequenceEqual(sample.After.registers));
        }

        //gtrr (greater-than register/register) sets register C to 1 if register A is greater than register B.Otherwise, register C is set to 0.
        private bool Gtrr(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] > check[B] ? 1 : 0;

            return (check.SequenceEqual(sample.After.registers));
        }

        //gtri (greater-than register/immediate) sets register C to 1 if register A is greater than value B.Otherwise, register C is set to 0.
        private bool Gtri(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] > B ? 1 : 0;

            return (check.SequenceEqual(sample.After.registers));
        }

        //eqir (equal immediate/register) sets register C to 1 if value A is equal to register B.Otherwise, register C is set to 0.
        private bool Eqir(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = A == check[B] ? 1 : 0;

            return (check.SequenceEqual(sample.After.registers));
        }

        //eqri (equal register/immediate) sets register C to 1 if register A is equal to value B.Otherwise, register C is set to 0.
        private bool Eqri(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] == B ? 1 : 0;

            return (check.SequenceEqual(sample.After.registers));
        }

        //eqrr (equal register/register) sets register C to 1 if register A is equal to register B.Otherwise, register C is set to 0.
        private bool Eqrr(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] == check[B] ? 1 : 0;

            return (check.SequenceEqual(sample.After.registers));
        }

        delegate bool Opcodes(Sample sample);


        #region Part2

        delegate void Opcodes2(Sample sample);



        public override string Part2()
        {
            var lines = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None);
            var spacesAndNumbers = new Regex("[^0-9 \n]+");
            List<Sample> samples = new List<Sample>();
            List<Instruction> instructions = new List<Instruction>();
            var takeInstructions = false;
            var y = 0;
            var takeLines = true;
            while (takeLines)
            {

                var sample = new Sample();
                var allLines = lines.Skip(y * 4).Take(4).ToList();
                y++;

                if (!allLines.Any()) takeLines = false;
                var countEndInstructions = 0;
                foreach (var line in allLines)
                {
                    var firstChar = line.FirstOrDefault();

                    if (line.Length > 0)
                    {
                        countEndInstructions = 0;
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
                    else
                    {
                        countEndInstructions++;
                    }

                    if (countEndInstructions > 2) break;
                }

                if (sample.Before != null)
                {
                    samples.Add(sample);
                };
            }

            var countEmptyLines = 0;
            foreach (var line in lines)
            {
                if (!takeInstructions)
                {
                    if (line.Length == 0)
                        countEmptyLines++;

                    else { countEmptyLines = 0; }
                    if (countEmptyLines > 2)
                    {
                        takeInstructions = true;
                    }
                }
                else
                {
                    var register = spacesAndNumbers.Replace(line, "").Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                    instructions.Add(new Instruction(register[0], register[1], register[2], register[3]));

                }
            }


            var opcodes = new List<(int, Opcodes)>();
            opcodes.Add((0, Addr));
            opcodes.Add((1, Addi));
            opcodes.Add((2, Mulr));
            opcodes.Add((3, Muli));
            opcodes.Add((4, Banr));
            opcodes.Add((5, Bani));
            opcodes.Add((6, Borr));
            opcodes.Add((7, Bori));
            opcodes.Add((8, Setr));
            opcodes.Add((9, Seti));
            opcodes.Add((10, Gtir));
            opcodes.Add((11, Gtri));
            opcodes.Add((12, Gtrr));

            opcodes.Add((13, Eqir));
            opcodes.Add((14, Eqri));
            opcodes.Add((15, Eqrr));


            var opcodeNUmbers = new Dictionary<int, int>();
            foreach (var sample in samples)
            {
                //try opcodes
                foreach (var opcode in opcodes)
                {
                    if (opcode.Item2(sample))
                    {
                        sample.Opcodes.Add((opcode.Item1, opcode.Item2));

                    };
                }
            }

            var combinations = new Dictionary<int, List<int>>();
            var Originalpossibilities = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            foreach (var i in Originalpossibilities.Select(c => c))

            {
                var removed = new List<int>();

                var possibilities = new int[Originalpossibilities.Length];
                Originalpossibilities.CopyTo(possibilities, 0);

                var codes = samples.Where(c => c.Instruction.OpcodeID == i).ToList();
                codes.ToList().ForEach(c => Debug.WriteLine($"{i}: {string.Join(',', string.Join(',', c.Opcodes.Select(f => f.Item1)))}"));

                foreach (var code in codes.Select(c => c.UsedCodes.Where(e => !removed.Contains(e))))
                {
                    possibilities = possibilities.Select(c => c).Intersect(code.Select(d => d)).ToArray();
                    possibilities = possibilities.Select(c => c).Intersect(code.Select(d => d)).ToArray();
                }
                if (combinations.ContainsKey(i))
                {
                    //calculate new list and add
                    var newlist = combinations[i].Intersect(possibilities).ToList();
                    combinations[i] = newlist;
                }
                else
                {
                    combinations.Add(i, possibilities.ToList());
                }
            }
            Originalpossibilities = Originalpossibilities.Where(d => !opcodeNUmbers.Select(c => c.Key).Contains(d)).ToArray();
            //combinations.ToList().ForEach(c => Debug.WriteLine($"{c.Key}: {string.Join(',', c.Value)}"));

            //TODO calculate Sudoku way!
            var sudokus = @"0:4
1:0
2:14
3:8
4:12
5:7
6:10
7:9
8:6
9:5
10:13
11:15
12:11
13:1
14:3
15:2";


            var opcodes2 = new List<(int, Opcodes2)>();
            opcodes2.Add((0, Addr2));
            opcodes2.Add((1, Addi2));
            opcodes2.Add((2, Mulr2));
            opcodes2.Add((3, Muli2));
            opcodes2.Add((4, Banr2));
            opcodes2.Add((5, Bani2));
            opcodes2.Add((6, Borr2));
            opcodes2.Add((7, Bori2));
            opcodes2.Add((8, Setr2));
            opcodes2.Add((9, Seti2));
            opcodes2.Add((10, Gtir2));
            opcodes2.Add((11, Gtri2));
            opcodes2.Add((12, Gtrr2));
            opcodes2.Add((13, Eqir2));
            opcodes2.Add((14, Eqri2));
            opcodes2.Add((15, Eqrr2));

            var sudokuCodes = sudokus.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Split(":")).Select(d => new { originalID = d[0], ID = d[1] });
            var baseInstruction = new Register(0, 0, 0, 0);

            foreach (var instruction in instructions)
            {
                var method = sudokuCodes.First(c => c.originalID == instruction.OpcodeID.ToString());
                var opcode = opcodes2.First(c => c.Item1.ToString() == method.ID);
                var sample = new Sample() { Before = baseInstruction, Instruction = instruction };
                opcode.Item2(sample);
                baseInstruction = sample.After;
            }

            return baseInstruction.registers[0].ToString();
        }

        private void Addr2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] + check[B];
            sample.After.registers = check;

            // return (check.SequenceEqual(sample.After.registers));

        }

        //addi(add immediate) stores into register C the result of adding register A and value B.
        private void Addi2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] + B;
            sample.After.registers = check;
        }

        //mulr (multiply register) stores into register C the result of multiplying register A and register B.
        private void Mulr2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] * check[B];
            sample.After.registers = check;
        }

        //muli (multiply immediate) stores into register C the result of multiplying register A and value B.
        private void Muli2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] * B;
            sample.After.registers = check;
        }

        //banr (bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
        private void Banr2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] & check[B];
            sample.After.registers = check;
        }

        //bani(bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
        private void Bani2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] & B;
            sample.After.registers = check;
        }

        //borr (bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
        private void Borr2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] | check[B];
            sample.After.registers = check;
        }

        //bori(bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
        private void Bori2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] | B;
            sample.After.registers = check;
        }

        //setr (set register) copies the contents of register A into register C. (Input B is ignored.)
        private void Setr2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var C = sample.Instruction.OutputC;
            check[C] = check[A];
            sample.After.registers = check;
        }

        //seti(set immediate) stores value A into register C. (Input B is ignored.)
        private void Seti2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var C = sample.Instruction.OutputC;
            check[C] = A;
            sample.After.registers = check;
        }


        //gtir(greater-than immediate/register) sets register C to 1 if value A is greater than register B.Otherwise, register C is set to 0.

        private void Gtir2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = A > check[B] ? 1 : 0;
            sample.After.registers = check;
        }

        //gtrr (greater-than register/register) sets register C to 1 if register A is greater than register B.Otherwise, register C is set to 0.
        private void Gtrr2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] > check[B] ? 1 : 0;
            sample.After.registers = check;
        }

        //gtri (greater-than register/immediate) sets register C to 1 if register A is greater than value B.Otherwise, register C is set to 0.
        private void Gtri2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] > B ? 1 : 0;
            sample.After.registers = check;
        }

        //eqir (equal immediate/register) sets register C to 1 if value A is equal to register B.Otherwise, register C is set to 0.
        private void Eqir2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = A == check[B] ? 1 : 0;
            sample.After.registers = check;
        }

        //eqri (equal register/immediate) sets register C to 1 if register A is equal to value B.Otherwise, register C is set to 0.
        private void Eqri2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] == B ? 1 : 0;
            sample.After.registers = check;
        }

        //eqrr (equal register/register) sets register C to 1 if register A is equal to register B.Otherwise, register C is set to 0.
        private void Eqrr2(Sample sample)
        {
            int[] check = new int[4];
            sample.Before.registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] == check[B] ? 1 : 0;
            sample.After.registers = check;
        }
        #endregion

    }
}

