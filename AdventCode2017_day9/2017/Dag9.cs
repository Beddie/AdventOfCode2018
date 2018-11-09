using AdventCode;
using AdventCode.Enum;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdventCode.AdventBase;

namespace AdventCode2017
{
    public class Dag9 : AdventBase, AdventInterface
    {
        public Dag9()
        {
            Calculate();
            WriteDebug();
        }

        public void Calculate()
        {
            string text = Resources.dag9_2017;
            var deleteNext = false;
            var newCharList = new List<Char>();
            var insideGarbage = false;
            var groupScore = 0;
            var score = 0;
            var garbageScore = 0;
            foreach (char character in text.ToCharArray())
            {
                if (deleteNext)
                {
                    deleteNext = false;
                    continue;
                }
                switch (character)
                {
                    case '!':
                        deleteNext = true;
                        break;
                    case '<':
                        if (!insideGarbage) { insideGarbage = true; } else { garbageScore++; }
                        break;
                    case '>':
                        insideGarbage = false;
                        break;
                    case '{':
                        if (!insideGarbage) { groupScore++; } else { garbageScore++; }
                        break;
                    case '}':
                        if (!insideGarbage)
                        {
                            score = score + groupScore;
                            groupScore--;
                        }
                        else
                        {
                            garbageScore++;
                        }
                        break;
                    default:
                        if (insideGarbage)
                        {
                            garbageScore++;
                        }
                        else
                        {
                            newCharList.Add(character);
                        }
                        break;
                }
            }

            Answer1 = score;
            Answer2 = garbageScore;
        }

    }
}
