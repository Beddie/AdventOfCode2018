using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventCode2017
{
    public class Dag18 : AdventBase, AdventInterface
    {
        public Dag18()
        {

          //  CalculateAnswerA();
            CalculateAnswerB();

        }

        public void CalculateAnswerA()
        {
            new Dag18Program().CalculateAnswerA();
            //WriteDebugAnswers(this);
        }

        public void CalculateAnswerB()
        {
            new Dag18Program(0, true).CalculateAnswerB();
        }
    }
}

