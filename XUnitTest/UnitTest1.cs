using Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            for (int i = 0; i < 30; i++)
            {
                var sw = new Stopwatch();
                sw.Start();
                var day = RenderDay.GetDay(17);
                var check = day.Part2();
                if (check != day.Solution()[0]) throw new Exception("Niet get goede antwoord!");
                sw.Stop();
                averageRunTime.Add(sw.ElapsedMilliseconds);
            }
            Debug.WriteLine($"Average complete in {averageRunTime.Average()} ms");
        }
    }
}
