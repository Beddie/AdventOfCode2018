using System.Collections.Generic;
using System.Linq;

namespace AdventCode2017
{
    class Program
    {

        public class Component
        {
            public string check { get; set; }
        }

            static void Main(string[] args)
        {

            //List<Component> compList1 = new List<Component>();
            //List<Component> compList2 = new List<Component>();

            //compList1.Add(new Component() { check = "McDonald" });
            //compList1.Add(new Component() { check = "Harveys" });
            //compList1.Add(new Component() { check = "Wendys" });

            ////compList2.Add(new Component() { check = "Bessel" });

            //compList2.AddRange(compList1);
            ////var test = compList1.First();
            ////test.check = "Thom";
            ////var test2 = compList2.Skip(1).First().check;

            //compList2.First().check = "Fabienne";
            //var test3= compList2.First();
            //var test4 = compList1.First();

            var day24 = new Dag24();
        }

    }
}
