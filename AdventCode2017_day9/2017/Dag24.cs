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

        public void CalculateAnswerA()
        {

            //Build list of components
            components = puzzleText.Split('\r').Select(c => c.Split('/')).Select(c => new Component(c[0], c[1])).ToList();
            var startComponent = new Component("0","0",components);
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

            public Component(string portA, string portB, List<Component> components)
            {
                Values.Add(Convert.ToInt16(portA));
                Values.Add(Convert.ToInt16(portB));
                matchList = components;
                BuildBridge(null);
            }

            public Component(List<Component> components)
            {
                matchList.AddRange(components);
            }

            private List<Component> matchList = new List<Component>();
            public List<int> Values = new List<int>();
            public List<int> MatchValues = new List<int>();
            public Status Status { get; set; } = Status.unMatched;
            public byte portNumberToBeMatched = 0;
            public byte? portNumberToMatch = (byte?)null;
            public int GetTotalScore()
            {
                return Values.Sum();
            }

            private void BuildBridge(Component previousMatch = null)
            {
                if (previousMatch == null)
                {
                    previousMatch = new Component();
                    previousMatch.MatchValues.Add(0);
                }
                var  bridgeMatchList  = new List<Component>();
                //Set status matched
                bridgeMatchList.AddRange(matchList);

                var matcher = bridgeMatchList.Where(c => c.Status == Status.unMatched && c.Values.All(x => previousMatch.Values.Contains(x))).FirstOrDefault();
                if (matcher != null) { matcher.Status = Status.matched; }


                var listMatches = new List<Component>();

                listMatches = bridgeMatchList.Where(c => c.Status == Status.unMatched && previousMatch.MatchValues.Intersect(c.Values).Any()).ToList();

                if (listMatches.Any())
                {
                    foreach (var match in listMatches)
                    {
                        //Values = match.Values;

                        var matching = matchList.Where(c => c == match).FirstOrDefault();
                        List<Component> newCompontents = new List<Component>();
                        newCompontents.AddRange(bridgeMatchList);
                        var buildnewCOmponent = new Component(newCompontents);

                        //Based on previous value(s) delete this value 
                        var deleteValuesList = new List<int>();
                        deleteValuesList.AddRange(matching.Values);
                        deleteValuesList.Remove(previousMatch.MatchValues.First());

                        matching.MatchValues.Add(deleteValuesList.First());
                        buildnewCOmponent.BuildBridge(matching);
                    }
                }
                else
                {
                    //DEAD!
                    var deadList = matchList.Where(c => c.Status == Status.matched).ToList();

                    DeadBridges.Add(new DeadBridge() { Components = deadList, HighScore = deadList.Sum(c => c.GetTotalScore()) });

                }

            }
        }
    }
}
