using AdventCode;
using AdventCode.Properties;
using Logic.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AdventCode2017
{
    public class Dag24 : AdventBase, AdventInterface
    {
        public static bool test = false;
        public string puzzleText = test ? Resources.dag24_2017_test : Resources.dag24_2017;
        private List<Component> components;
        public struct DeadBridge
        {
            public int HighScore;
            public List<Component> Components;
        }
        public List<DeadBridge> DeadBridges = new List<DeadBridge>();

        public Dag24()
        {
            CalculateAnswerA();
            WriteDebugAnswers(this);
        }

        public void CalculateAnswerA()
        {
            //Build list of components
            components = puzzleText.Split('\r').Select(c => c.Split('/')).Select(c => new Component(c[0], c[1])).ToList();
            BuildBridge(components, 0);
            var highScore = DeadBridges.Select(c => c.HighScore).Max();
            Answer1 = highScore;
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

        private void BuildBridge(List<Component> previousMatchList, int matchValue)
        {
            var matchList = previousMatchList.Select(x => new Component()
            {
                Status = x.Status,
                Values = x.Values
            }).ToList();

            var listMatches = matchList.Where(c => c.Status == Status.unMatched && c.Values.Contains(matchValue)).ToHashSet();

            if (listMatches.Any())
            {
                foreach (var match in listMatches)
                {
                    //Map previousMatchList to new object
                    var matchListToPass = previousMatchList.Select(x => new Component()
                    {
                        Status = x.Status,
                        Values = x.Values
                    }).ToList();

                    //Set status matched to current match
                    var matching = matchListToPass.Where(c => c.Values == match.Values).FirstOrDefault();
                    matching.Status = Status.matched;

                    //Based on previous value(s) delete this value and pass value to match
                    var valuesToMatchNext = new List<int>();
                    valuesToMatchNext.AddRange(matching.Values);
                    valuesToMatchNext.Remove(matchValue);

                    //Recursive build bridges
                    BuildBridge(matchListToPass, valuesToMatchNext.First());
                }
            }
            else
            {
                //DEAD!
                var deadList = matchList.Where(c => c.Status == Status.matched).ToList();
                // matchList.Dump();
                DeadBridges.Add(new DeadBridge() { Components = deadList, HighScore = deadList.Sum(c => c.GetTotalScore()) });
            }
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
            public int GetTotalScore()
            {
                return Values.Sum();
            }
        }

        public enum Status
        {
            unMatched = 0,
            matched = 1
        }
    }
}
