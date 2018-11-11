using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventCode
{
    public class AdventBase
    {
        public object Answer1 { get; set; }
        public object Answer2 { get; set; }
        public bool Test { get; set; }

        public void WriteDebugAnswers(object className)
        {
            Debug.WriteLine($"{className.GetType().Name} Antwoord 1: {Answer1}");
            Debug.WriteLine($"{className.GetType().Name} Antwoord 2: {Answer2}");
        }

        public void WriteDebug<T>(T write, object className)
        {
            Debug.WriteLine($"{className.GetType().Name}: {write}");
        }
    }
}
