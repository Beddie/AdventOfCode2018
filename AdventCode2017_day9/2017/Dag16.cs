using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventCode2017
{
    public class Dag16 : AdventBase, AdventInterface
    {
        private static bool test = false;
        private string puzzleText = test ? "s1,x3/4,pe/b" : Resources.dag16_2017;
        private class PuzzleString { public string Puzzle { get; set;  } }
        public void CalculateAnswerA()
        {
            var loopCOunt = 0;
            var puzzleString = string.Empty;

            for (char i = 'a'; i <= (test ? 'e' : 'p'); i++)
            {
                loopCOunt++;
                puzzleString += i;
            }
            var puzzleLength = puzzleString.Length;
            var puzzleSequence = puzzleText.Split(',').ToList();

            foreach (var danceMove in puzzleSequence)
            {
                var danceAct = danceMove.First();
                var danceActors = danceMove.Substring(1);
                switch (danceAct)
                {
                    case 's':
                        //spin
                        var spinAmount = Convert.ToByte(danceActors);
                        var splitIndex = puzzleLength - spinAmount;
                        puzzleString = puzzleString.Substring(splitIndex, spinAmount) + puzzleString.Substring(0, splitIndex);
                        break;
                    case 'x':
                        //Exchange
                        var Exchanges = danceActors.Split('/').Select(c => Convert.ToByte(c));
                        puzzleString = SwapTwoCharactersByIndexInString(puzzleString, Exchanges.First(), Exchanges.Skip(1).First());
                        break;
                    case 'p':
                        //Partner
                        var Partners = danceActors.Split('/').Select(c => Convert.ToChar(c));
                        puzzleString = SwapTwoCharactersByLetterInString(puzzleString, Partners.First(), Partners.Skip(1).First());
                        break;
                    default:
                        throw new KeyNotFoundException();
                }
            }

            Debug.WriteLine(puzzleString);
        }

        private static string SwapTwoCharactersByIndexInString(string value, byte position1, byte position2)
        {
            char[] array = value.ToCharArray(); // Get characters
            char temp = array[position1]; // Get temporary copy of character
            array[position1] = array[position2]; // Assign element
            array[position2] = temp; // Assign element
            return new string(array); // Return string
        }

        private static string SwapTwoCharactersByLetterInString(string value, char letter1, char letter2)
        {
            char[] array = value.ToCharArray(); // Get characters
            var position1 = value.IndexOf(letter1);
            var position2 = value.IndexOf(letter2);
            char temp = array[position1]; // Get temporary copy of character
            array[position1] = array[position2]; // Assign element
            array[position2] = temp; // Assign element
            return new string(array); // Return string
            //return SwapTwoCharactersByIndexInString(value, position1, position2);
        }

        public Dag16()
        {
            CalculateAnswerA();
            CalculateAnswerB();
            WriteDebugAnswers(this);
        }

        public delegate string MyActionWrapper(string dancePuzzleString);

        private class DanceMove
        {
            public MyActionWrapper Dance { get; set; }

            public DanceMove(char danceAct, string danceActors, int puzzleLength)
            {
                switch (danceAct)
                {
                    case 's':
                        //DanceAct = danceAct;
                        var spinAMount = Convert.ToByte(danceActors);
                        var splitIndex = puzzleLength - spinAMount;
                        Dance = (puzzle) => puzzle.Substring(splitIndex, spinAMount) + puzzle.Substring(0, splitIndex); //Spin(puzzle, spinAMount, splitIndex);                        
                        break;
                    case 'x':
                        //Exchange
                        var splitActors = danceActors.Split('/').Select(c => Convert.ToByte(c)).ToArray();
                        Dance = (puzzle) => SwapTwoCharactersByIndexInString(puzzle, splitActors[0], splitActors[1]);
                        break;
                    case 'p':
                        //Partner
                        var splitStringActors = danceActors.Split('/').Select(c => Convert.ToChar(c)).ToArray(); //TODO: Zou handig zijn als dit ook bytes zijn
                        Dance = (puzzle) => SwapTwoCharactersByLetterInString(puzzle, splitStringActors[0], splitStringActors[1]);
                        break;
                }
            }
        }


        public void CalculateAnswerB()
        {
            var loopCOunt = 0;
            var puzzleString = string.Empty;

            for (char i = 'a'; i <= (test ? 'e' : 'p'); i++)
            {
                loopCOunt++;
                puzzleString += i;
            }
            var puzzleLength = puzzleString.Length;
            var puzzleSequence = puzzleText.Split(',').ToList();

            var danceMoves = new List<DanceMove>();
            foreach (var danceMove in puzzleSequence)
            {
                danceMoves.Add(new DanceMove(danceMove.First(), danceMove.Substring(1), puzzleLength));
            }

            var danceMovesArray = danceMoves.ToArray();
            var puzz = new PuzzleString() { Puzzle = puzzleString };

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000000000; i++)
            {
                foreach (var danceMove in danceMovesArray)
                {
                    puzzleString = danceMove.Dance(puzzleString);
                }
                if (i % 1000 == 0) Debug.WriteLine($"{i} - {sw.ElapsedMilliseconds} - {puzzleString} ");
            }
            sw.Stop();
            Debug.WriteLine($"{puzzleString} - {sw.ElapsedMilliseconds}");
        }
    }
}
