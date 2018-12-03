using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void DayRender()
        {
            var day3 = RenderDay.GetDay(3);
            var check3 = day3.Part2();

            var averageRunTime = new HashSet<long>();

            //Execute puzzle 100 times
            for (int i = 0; i < 1000; i++)
            {
                var sw = new Stopwatch();
                sw.Start();
                var day = RenderDay.GetDay(2);
                var check = day.Part2();
                Assert.AreEqual(check, day.Solution()[1]);
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
