using Logic.Days;
using Logic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class RenderDay
    {
        public static AdventInterface GetDay(int day)
        {
            switch (day)
            {
                case 1:
                    return new Day1();
                case 2:
                    return new Day2();
                case 3:
                    return new Day3();
                case 4:
                    return new Day4();
                case 5:
                    return new Day5();
                default:
                    return null;
            }
        }

        public static List<AdventInterface> GetOverview()
        {
            var overviewList = new List<AdventInterface>();
            for (int i = 0; i < 26; i++)
            {
                var day = GetDay(i);
                if (day != null) overviewList.Add(day);
            }
            //overviewList.Add(new Day1());
            //overviewList.Add(new Day2());
            //overviewList.Add(new Day3());
            //overviewList.Add(new Day4());

            return overviewList;
        }
              
    }
}
