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

            //Execute puzzle 100 times
            for (int i = 0; i < 1; i++)
            {
                var sw = new Stopwatch();
                sw.Start();
                var day = RenderDay.GetDay(9);
                var check = day.Part1();
                //var day = RenderDay.GetDay(2);
                //var check = day.Part2();
                if (check != day.Solution()[0]) throw new Exception("Niet get goede antwoord!");
                //Assert.AreEqual(check, day.Solution()[1]);
                sw.Stop();
                averageRunTime.Add(sw.ElapsedMilliseconds);
                //Debug.WriteLine($"Completed in {sw.ElapsedMilliseconds} ms");
            }
            Debug.WriteLine($"Average complete in {averageRunTime.Average()} ms");
            //Assert.AreEqual(day.Part1(), day.GetSolutions()[0]);
            //Assert.AreEqual(day.Part2(), day.GetSolutions()[1]);
        }
    }
}
