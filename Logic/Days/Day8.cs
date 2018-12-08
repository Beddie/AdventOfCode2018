using Logic.Interface;
using Logic.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day8: AdventBase, AdventInterface
    {
        public Day8()
        {
            // Test = true;
            PuzzleInput = Test ? Resources.Day8Example : Resources.Day8;
        }

        public string[] Solution()
        {
            return new string[] {
                "",
                ""
            };
        }

        private class Node {
            public int AmountOfChild { get; set; }
            public int AmountOfMetaData { get; set; }
            public List<int> Data { get; set; }
            public List<Node> ChildNodes { get; set; } = new List<Node>();
            public IEnumerable<int> MetaData => Data.TakeLast(AmountOfMetaData);
            public int NodeLength => 2 + Data.Count();
            public int DataLength => Data.Count() - AmountOfMetaData;
            public int Layer { get; set; }
            public string PrintData => string.Format("{0}  {1}  {2}", AmountOfChild, AmountOfMetaData, String.Join("  ", Data));
            public int Index { get; set; }
        }

        public string Part1()
        {
            var rootNode =  new Node() {AmountOfChild = Convert.ToInt32(PuzzleInput.Substring(0,1)), AmountOfMetaData = Convert.ToInt32(PuzzleInput.Substring(2, 1)), Data = PuzzleInput.Substring(4, PuzzleInput.Length -4).Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(c=> Convert.ToInt32(c)).ToList() };

            CreateChildNodes(rootNode);
            Print(rootNode);
            return metaDataSum.Sum().ToString();
        }

        private List<int> metaDataSum = new List<int>();

        private void CreateChildNodes(Node rootNode)
        {
            metaDataSum.AddRange(rootNode.MetaData);
            var index = 0;
            var averageDataLength = rootNode.AmountOfChild > 0 ? rootNode.DataLength / rootNode.AmountOfChild : 0;
            for (int i = 0; i < rootNode.AmountOfChild; i++)
            {
                var isLastNode = rootNode.AmountOfChild == i + 1;
                //var childNode = new Node() { AmountOfChild = rootNode.Data.Skip(index).Take(1).First(), AmountOfMetaData = rootNode.Data.Skip(index + 1).Take(1).First(), Data = rootNode.Data.Skip(index + 2).Take(isLastNode ? rootNode.DataLength - index - 2 : rootNode.Data.Skip(index + 1).Take(1).First()).ToList() };
                var childNode = new Node() { Index = rootNode.Index+ 2+ index,  Layer = rootNode.Layer + 1,  AmountOfChild = rootNode.Data.Skip(index).Take(1).First(), AmountOfMetaData = rootNode.Data.Skip(index + 1).Take(1).First(), Data = rootNode.Data.Skip(index + 2).Take(isLastNode ? rootNode.DataLength - index - 2 : averageDataLength - 2).ToList() };
                rootNode.ChildNodes.Add(childNode);
               Print(rootNode);
                index += childNode.NodeLength;
            }

            foreach (var childNode in rootNode.ChildNodes)
            {
               
                CreateChildNodes(childNode);
            }
        }

        private void Print(Node rootNode) {
            var sb = new StringBuilder();
            sb.Append("".PadLeft(rootNode.Index * 3));
            sb.AppendLine(rootNode.PrintData);
            sb.Append("".PadRight(2));
            PrintNested(rootNode, sb);
            Debug.WriteLine(sb.ToString());
            
        }
        private void PrintNested(Node rootNode, StringBuilder sb)
        {
            var once1 = false;
            var once2 = false;
            var once3 = false;
            sb.Append("".PadLeft(rootNode.Index * 3));
            sb.Append("".PadRight(2));
            foreach (var childNode in rootNode.ChildNodes)
            {
                if (!once2) sb.Append("".PadLeft(childNode.Index * 3));
                sb.Append(childNode.PrintData);
                if (!once2)  sb.Append("".PadRight(2));
                once2 = true;
            }
            sb.AppendLine();
            
            foreach (var childNode in rootNode.ChildNodes)
            {
                
                foreach (var childchildNode in childNode.ChildNodes)
                {
                    if (!once3) sb.Append("".PadLeft(childchildNode.Index * 3));
                    sb.Append(childchildNode.PrintData);
                    if (!once3) sb.Append("".PadRight(2));
                    once3 = true;
                }
            }
            sb.AppendLine();
        }

        public string Part2()
        {
            return "";
        }


        public string GetListName()
        {
            return "Day 8: Memory Maneuver";
        }

        public int GetID()
        {
            return 8;
        }
    }
}

