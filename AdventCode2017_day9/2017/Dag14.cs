using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventCode2017
{
    public class Dag14 : AdventBase, AdventInterface
    {
        private static bool test = true;
        private Dictionary<int, int> baseList = new Dictionary<int, int>();
        private string text = test ? "flqrgnkx" : Resources.dag14_2017;
        private int totalRows = 128;

        public Dag14()
        {
            InitializeBaseList();
            Calculate();
            WriteDebugAnswers(this);
        }
        
        private void InitializeBaseList()
        {
            var knotStringList = new List<string>();
            var knotHexList = new List<string>();
            for (int i = 0; i < totalRows; i++)
            {
                knotStringList.Add($"{text}-{i}");
            }

            foreach (var knotString in knotStringList)
            {
                var binaryString = ToBinary(ConvertToByteArray(knotString, Encoding.UTF8));
                var test = binaryString.Length;
            }

        }

        public byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public String ToBinary(Byte[] data)
        {
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }

        // Use any sort of encoding you like. 
       


        private void UpdateSeverityScore(KeyValuePair<int, int> layerPair, int currentPicosecond)
        {
            //var range = layerPair.Value;

            ////isRootLevel?
            //var isRootLevel = ((currentPicosecond % ((range - 1 ) * 2)) == 0) ;
            //severityScore += isRootLevel ? range * layerPair.Key : 0;
            //answer2HelperScore = severityScore;
        }

        public void Calculate()
        {
            //var LastPicosecond = baseList.Max(c => c.Key) + delayAmount;
            //for (int picosecond = 0 + delayAmount; picosecond <= LastPicosecond; picosecond++)
            //{
            //    var layer = picosecond - delayAmount;
            //    var layerFromDictionary = baseList.Where(c => c.Key == layer);
            //    if (layerFromDictionary.Any())  UpdateSeverityScore(layerFromDictionary.First(), picosecond);
            //    if (delayAmount > 0 && answer2HelperScore > 0) return;
            //}
            //if (delayAmount == 0) Answer1 = severityScore;
        }

        private void CalculateAnswer2()
        {
            //delayAmount = -1;
            //while (answer2HelperScore != 0) {
            //    severityScore = 0;
            //    delayAmount++;
            //    Calculate();
                
            //}

            //Answer2 = delayAmount;
        }


    }
}
