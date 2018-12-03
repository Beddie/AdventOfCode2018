using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AdventCode2017
{
    public class Dag24 : AdventBase, AdventInterface
    {
        public static bool test = true;
        public string puzzleText = test ? Resources.dag24_2017_test : Resources.dag24_2017;
        private List<Component> components;
        public struct DeadBridge
        {
            public int HighScore;
            public List<Component> Components;
        }
        public static List<DeadBridge> DeadBridges = new List<DeadBridge>();



        public Dag24()
        {
            CalculateAnswerA();
            WriteDebugAnswers(this);
        }


        private void BuildBridge(Component previousMatch, List<Component> previousMatchList, int matchValue)
        {
            //Set status matched
            var matchList = previousMatchList.Select(x => new Component()
            {
                Status = x.Status,
                Values = x.Values
            }).ToList();

            var matcher = matchList.FirstOrDefault(c => c.Status == Status.unMatched && c.Values.All(x => previousMatch.Values.Contains(x)));
            if (matcher != null) { matcher.Status = Status.matched; }

            var listMatches = matchList.Where(c => c.Status == Status.unMatched && c.Values.Contains(matchValue)).ToList();

            if (listMatches.Any())
            {
                foreach (var match in listMatches)
                {
                    var matching = matchList.Where(c => c == match).FirstOrDefault();
                    //Based on previous value(s) delete this value 
                    var deleteValuesList = new List<int>();
                    deleteValuesList.AddRange(matching.Values);
                    deleteValuesList.Remove(matchValue);

                    //matching.MatchValues.Add(deleteValuesList.First());
                    BuildBridge(matching, matchList, deleteValuesList.First());
                }
            }
            else
            {
                //DEAD!
                var deadList = matchList.Where(c => c.Status == Status.matched).ToList();

                DeadBridges.Add(new DeadBridge() { Components = deadList, HighScore = deadList.Sum(c => c.GetTotalScore()) });

            }

        }

        public void CalculateAnswerA()
        {

            //Build list of components
            components = puzzleText.Split('\r').Select(c => c.Split('/')).Select(c => new Component(c[0], c[1])).ToList();
            BuildBridge(new Component("0", "0"), components, 0);
            //startComponent.BuildBridge(components, null);
            var highScore = DeadBridges.Select(c => c.HighScore).Max();



            //Build bridges, Jump into matches loop

            //Scaffolding dead bridges
            //FInd highscore



            //put puzzleinput into list of pairs
            using (StringReader reader = new StringReader(puzzleText)) { }

            //use each compponent only once, which makes the stongest bridge?
            //Must start with 0
            //Ports must match (side does not matter)

            //0 / 1
            //0 / 1--10 / 1
            //0 / 1--10 / 1--9 / 10    **** WINS ****
            //0 / 2
            //0 / 2--2 / 3
            //0 / 2--2 / 3--3 / 4
            //0 / 2--2 / 3--3 / 5
            //0 / 2--2 / 2
            //0 / 2--2 / 2--2 / 3
            //0 / 2--2 / 2--2 / 3--3 / 4
            //0 / 2--2 / 2--2 / 3--3 / 5

        }

        public void CalculateAnswerB()
        {

        }

        public enum Status
        {
            unMatched = 0,
            matched = 1
        }
        public class Component
        {
            public Component() { }
            public Component(string portA, string portB)
            {
                Values.Add(Convert.ToInt16(portA));
                Values.Add(Convert.ToInt16(portB));
            }
            
            public List<int> Values = new List<int>();
            public Status Status { get; set; } = Status.unMatched;
            //public byte portNumberToBeMatched = 0;
            //public byte? portNumberToMatch = (byte?)null;
            public int GetTotalScore()
            {
                return Values.Sum();
            }

        }
    }
}
