using Logic.Interface;
using Logic.Properties;
using Newtonsoft.Json;
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
    public class Day8 : AdventBase, AdventInterface
    {
        public Day8()
        {
            //Test = true;
            PuzzleInput = Test ? Resources.Day8Example : Resources.Day8;
        }

        public string[] Solution()
        {
            return new string[] {
                "48443",
                "30063"
            };
        }

        private class Node
        {
            public int AmountOfChild => NodeData.Skip(Index).Take(1).First();
            public int AmountOfMetaData => NodeData.Skip(Index).Skip(1).Take(1).First();
            public List<int> Data => NodeData.Skip(Index).Take(NodeLength).ToList();
            public List<Node> ChildNodes { get; set; } = new List<Node>();
            public List<int> MetaData => Data.TakeLast(AmountOfMetaData).ToList();
            public int NodeLength => 2 + ChildNodes.Select(x => x.NodeLength).Sum() + AmountOfMetaData;
            public int MetaDataSum => MetaData.Sum();
            public string PrintData => string.Format("{0}", String.Join("  ", Data));
            public int Index { get; set; }
            public int Value { get; set; }
            public bool HasChildren() => ChildNodes.Any();
        }

        private static List<int> NodeData = new List<int>();

        public string Part1()
        {
            NodeData = PuzzleInput.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(c => Convert.ToInt32(c)).ToList();
            var rootNode = new Node() { };

            CreateChildNodes(rootNode);
            metaDataSum = SumMetaData(rootNode);
            if (Test) Print(rootNode);
            return metaDataSum.ToString();
        }

        private int SumMetaData(Node rootNode)
        {
            var sumMetaData = rootNode.MetaData.Sum();
            foreach (var childNode in rootNode.ChildNodes) sumMetaData += SumMetaData(childNode);
            return sumMetaData;
        }

        private int metaDataSum;

        private void CreateChildNodes(Node rootNode)
        {
            var childIndex = 2;
            for (int i = 0; i < rootNode.AmountOfChild; i++)
            {
                var newChildNode = new Node()
                {
                    Index = rootNode.Index + childIndex
                };
                CreateChildNodes(newChildNode);
                rootNode.ChildNodes.Add(newChildNode);
                childIndex += newChildNode.NodeLength;
            }
        }

        private void Print(Node rootNode)
        {
            var sb = new StringBuilder();
            sb.AppendLine(rootNode.PrintData);
            Debug.WriteLine(sb.ToString());
        }

        public string Part2()
        {
            NodeData = PuzzleInput.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(c => Convert.ToInt32(c)).ToList();
            var rootNode = new Node();

            CreateChildNodes(rootNode);
            metaDataSum = GetNodeValueByMetaData(rootNode);
            if (Test) Print(rootNode);
            return metaDataSum.ToString();
        }

        private int GetNodeValueByMetaData(Node node)
        {
            if (Test && node.Value > 0)  Debug.WriteLine(JsonConvert.SerializeObject(node));

            if (!node.HasChildren()) node.Value = node.MetaDataSum;
            else
            {
                var count = 0;
                foreach (var metaDataReference in node.MetaData)
                {
                    var existingNode = node.ChildNodes.ElementAtOrDefault(metaDataReference - 1);
                    if (existingNode != null) count += GetNodeValueByMetaData(existingNode);
                }
                node.Value = count;
            }
            return node.Value;
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

