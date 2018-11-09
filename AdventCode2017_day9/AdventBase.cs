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
        public int Answer1 { get; set; }
        public int Answer2 { get; set; }

        public void WriteDebug()
        {
            Debug.WriteLine($"Antwoord 1: {Answer1}");
            Debug.WriteLine($"Antwoord 2: {Answer2}");
        }
    }
}
