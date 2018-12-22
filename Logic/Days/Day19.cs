using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Logic.Days
{
    public class Day19 : AdventBase
    {
        public Day19()
        {
            //Test = true;
            PuzzleInput = Test ? Resources.Day19Example : Resources.Day19;
            ID = 19;
            Name = "Day 19: Go With The Flow";
        }

        public override string[] Solution()
        {
            return new string[] { "1464", ""
            };
        }

        public override string Part1()
        {
            InitializeOpcodeList();
            var puzzleLines = PuzzleInput.Split("\r\n");
            var boundRegister = Convert.ToInt32(puzzleLines.First().Replace("#ip ", ""));
            var samples = new List<Sample>();
            foreach (var puzzleLine in puzzleLines.Skip(1))
            {
                var instructions = puzzleLine.Split(" ");
                var opcode = GetOpcodeByName(instructions[0]);
                samples.Add(new Sample() { Instruction = new Instruction(opcode, instructions[1], instructions[2], instructions[3]) });
            }

            Register register = new Register(0,0,0,0,0,0);

            var instructionPointer = (long)0;
            var reg0 = (long)0;
            var difference = (long)0;
            for (int i = 0; i < samples.Count(); i++)
            {
                samples[i].Before = register;
                samples[i].Before.Registers[boundRegister] = instructionPointer;
                samples[i].Instruction.Opcode(samples[i]);
                register = samples[i].After;
                //instructionPointer = register.registers[boundRegister] + 1;
                difference = register.Registers[boundRegister] - instructionPointer;
                instructionPointer = register.Registers[boundRegister];
                //Print(samples[i], instructionPointer);
                instructionPointer++;
                if (instructionPointer >= samples.Count()) { reg0 = register.Registers[0]; break; }
                i = i + (int)difference;
            }

            return reg0.ToString();
        }

        #region Logic
        private List<Opcode> opcodes = new List<Opcode>();

        private void Print(int i, Sample sample, int pointer)
        {
            var sb = new StringBuilder();
            sb.Append($"sample {i}, pointerRegister2= {pointer} [{string.Join(", ", sample.Before.Registers)}] {sample.Instruction.Opcode.Method.Name} {sample.Instruction.InputA} {sample.Instruction.InputB} {sample.Instruction.OutputC} [{string.Join(", ", sample.After.Registers)}]");
            Debug.WriteLine(sb.ToString());
        }

        private void InitializeOpcodeList()
        {
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
        }

        private Opcode GetOpcodeByName(string opcode)
        {
            return opcodes.First(c => c.Method.Name.ToLower() == opcode.ToLower());
        }

        delegate void Opcode(Sample sample);

        private class Sample
        {
            public Register Before { get; set; } = new Register(0, 0, 0, 0, 0, 0);
            public Instruction Instruction { get; set; }
            public Register After { get; set; } = new Register(0, 0, 0, 0, 0, 0);
        }

        private class Register
        {
            public long[] Registers { get; set; }

            public Register(string a, string b, string c, string d, string e, string f)
            {
                Registers = new[] { Convert.ToInt64(a), Convert.ToInt64(b), Convert.ToInt64(c), Convert.ToInt64(d), Convert.ToInt64(e), Convert.ToInt64(f) };
            }
            public Register(long a, long b, long c, long d, long e, long f)
            {
                Registers = new [] { a, b, c, d, e, f };
            }
        }
        private class Instruction
        {
            public Opcode Opcode { get; set; }
            public int InputA { get; set; }
            public int InputB { get; set; }
            public int OutputC { get; set; }

            public Instruction(Opcode opcode, string a, string b, string output)
            {
                Opcode = opcode;
                InputA = Convert.ToInt32(a);
                InputB = Convert.ToInt32(b);
                OutputC = Convert.ToInt32(output);
            }
        }
        #endregion

        #region PuzzleMethods
        private void Addr(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] + check[B];

            sample.After.Registers = check;
        }

        //addi(add immediate) stores into register C the result of adding register A and value B.
        private void Addi(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] + B;
            sample.After.Registers = check;
        }

        //mulr (multiply register) stores into register C the result of multiplying register A and register B.
        private void Mulr(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] * check[B];
            sample.After.Registers = check;
        }

        //muli (multiply immediate) stores into register C the result of multiplying register A and value B.
        private void Muli(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] * B;
            sample.After.Registers = check;
        }

        //banr (bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
        private void Banr(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] & check[B];
            sample.After.Registers = check;
        }

        //bani(bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
        private void Bani(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] & B;
            sample.After.Registers = check;
        }

        //borr (bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
        private void Borr(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] | check[B];
            sample.After.Registers = check;
        }

        //bori(bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
        private void Bori(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] | B;
            sample.After.Registers = check;
        }

        //setr (set register) copies the contents of register A into register C. (Input B is ignored.)
        private void Setr(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var C = sample.Instruction.OutputC;
            check[C] = check[A];
            sample.After.Registers = check;
        }

        //seti(set immediate) stores value A into register C. (Input B is ignored.)
        private void Seti(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var C = sample.Instruction.OutputC;
            check[C] = A;
            sample.After.Registers = check;
        }


        //gtir(greater-than immediate/register) sets register C to 1 if value A is greater than register B.Otherwise, register C is set to 0.

        private void Gtir(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = A > check[B] ? 1 : 0;
            sample.After.Registers = check;
        }

        //gtrr (greater-than register/register) sets register C to 1 if register A is greater than register B.Otherwise, register C is set to 0.
        private void Gtrr(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] > check[B] ? 1 : 0;
            sample.After.Registers = check;
        }

        //gtri (greater-than register/immediate) sets register C to 1 if register A is greater than value B.Otherwise, register C is set to 0.
        private void Gtri(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] > B ? 1 : 0;
            sample.After.Registers = check;
        }

        //eqir (equal immediate/register) sets register C to 1 if value A is equal to register B.Otherwise, register C is set to 0.
        private void Eqir(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = A == check[B] ? 1 : 0;
            sample.After.Registers = check;
        }

        //eqri (equal register/immediate) sets register C to 1 if register A is equal to value B.Otherwise, register C is set to 0.
        private void Eqri(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] == B ? 1 : 0;
            sample.After.Registers = check;
        }

        //eqrr (equal register/register) sets register C to 1 if register A is equal to register B.Otherwise, register C is set to 0.
        private void Eqrr(Sample sample)
        {
            long[] check = new long[6];
            sample.Before.Registers.CopyTo(check, 0);
            var A = sample.Instruction.InputA;
            var B = sample.Instruction.InputB;
            var C = sample.Instruction.OutputC;
            check[C] = check[A] == check[B] ? 1 : 0;
            sample.After.Registers = check;
        }
        #endregion

        #region Part2
        public override string Part2()
        {
             //long reg0 = InvestigatePattern(new Register(1, 0, 0, 0, 0, 0), (long)0);
            //long reg0 = InvestigatePattern(new Register(0, 0, 10, 10551374, 1, 14007), (long)10);
            long reg0 = InvestigatePattern(new Register(1, 3, 14, 10551374, 2, 10551375), (long)14);
            return reg0.ToString();
        }

        private long InvestigatePattern(Register register, long instructionPointer, bool startPrint = false)
        {
            InitializeOpcodeList();
            var puzzleLines = PuzzleInput.Split("\r\n");
            var boundRegister = Convert.ToInt32(puzzleLines.First().Replace("#ip ", ""));


            var samples = new List<Sample>();
            foreach (var puzzleLine in puzzleLines.Skip(1))
            {
                var instructions = puzzleLine.Split(" ");
                var opcode = GetOpcodeByName(instructions[0]);
                samples.Add(new Sample() { Instruction = new Instruction(opcode, instructions[1], instructions[2], instructions[3]) });
            }

            var reg0 = (long)0;
            var difference = (long)0;
            for (int i = (int)instructionPointer; i < samples.Count(); i++)
            {
                samples[i].Before = register;
                samples[i].Before.Registers[boundRegister] = instructionPointer;
                samples[i].Instruction.Opcode(samples[i]);
                register = samples[i].After;
                difference = register.Registers[boundRegister] - instructionPointer;
                instructionPointer = register.Registers[boundRegister];

                Print(i, samples[i], (int)instructionPointer);
                // if ((int)instructionPointer > 10) Print(samples[i], (int)instructionPointer);
                //if (startPrint) Print(i, samples[i], (int)instructionPointer);
                //reg0 = register.Registers[0];
                //if (instructionPointer == 14)
                //{
                //    Print(i, samples[i], (int)instructionPointer);
                //    startPrint = true;
                //}
                //if (instructionPointer == 33)
                //{
                //    startPrint = true;
                //    Print(samples[i], (int)instructionPointer);
                //}

                instructionPointer++;

                if (instructionPointer >= samples.Count()) {
                    reg0 = samples[i].After.Registers[0];
                    break; }
                i = i + (int)difference;
            }

            return reg0;
        }
        #endregion
    }
}

