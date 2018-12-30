using Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace XUnitTest
{
    public class UnitTest1
    {
      
        [Fact]
        public void Test1()
        {
            var averageRunTime = new HashSet<long>();

            //Execute puzzle 30 times
            for (int i = 0; i < 1; i++)
            {
                var sw = new Stopwatch();
                sw.Start();
                var day = RenderDay.GetDay(21);
                var check = day.Part2();
               // if (check != day.Solution()[0]) throw new Exception("Niet get goede antwoord!");
                sw.Stop();
                averageRunTime.Add(sw.ElapsedMilliseconds);
            }
            Debug.WriteLine($"Average complete in {averageRunTime.Average()} ms");
        }

        public void TestNew() {
            var bb = new Regex(@"(?=\()(?=((?:(?=.*?\((?!.*?\2)(.*\)(?!.*\3).*))(?=.*?\)(?!.*?\3)(.*)).)+?.*?(?=\2)[^(]*(?=\3$)))");
            var cc = bb.GetGroupNames(); // 
            var dd = bb.Match("(=hi (how (are (you)test)))");
        }
    }
}
