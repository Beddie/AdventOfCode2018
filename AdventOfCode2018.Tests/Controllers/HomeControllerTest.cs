using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdventOfCode2018.Controllers;
using Logic;

namespace AdventOfCode2018.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void DayRender()
        {
            // Arrange
            var day = RenderDay.GetDay(1);
            Assert.AreEqual(day.Part1(), day.GetSolutions()[0]);
            Assert.AreEqual(day.Part2(), day.GetSolutions()[1]);
        }

        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

    }
}
