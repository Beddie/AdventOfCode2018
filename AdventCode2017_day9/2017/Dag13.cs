using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCode2017
{
    public class Dag13 : AdventBase, AdventInterface
    {
        private static bool test = false;
        private Dictionary<int, int> baseList = new Dictionary<int, int>();
        private string text = test ? Resources.dag13_2017_test : Resources.dag13_2017;

        private int severityScore = 0;
        private int delayAmount = 0;
        private int answer2HelperScore = 0;

        public Dag13()
        {
            InitializeBaseList();
            CalculateAnswerA();
            CalculateAnswer2();
            WriteDebugAnswers(this);
        }
        
        private void InitializeBaseList()
        {
            var finishAlmostJSONreaadyStringToJSONstring = "{" + text.Replace("\r", ",").Replace("\n", "") + "}";
            baseList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(finishAlmostJSONreaadyStringToJSONstring);
        }

        private void UpdateSeverityScore(KeyValuePair<int, int> layerPair, int currentPicosecond)
        {
            var range = layerPair.Value;

            //isRootLevel?
            var isRootLevel = ((currentPicosecond % ((range - 1 ) * 2)) == 0) ;
            severityScore += isRootLevel ? range * layerPair.Key : 0;
            answer2HelperScore = isRootLevel && layerPair.Key == 0 ? 9999 : severityScore;
        }

        public void CalculateAnswerA()
        {
            var LastPicosecond = baseList.Max(c => c.Key) + delayAmount;
            for (int picosecond = 0 + delayAmount; picosecond <= LastPicosecond; picosecond++)
            {
                var layer = picosecond - delayAmount;
                var layerFromDictionary = baseList.Where(c => c.Key == layer);
                if (layerFromDictionary.Any())  UpdateSeverityScore(layerFromDictionary.First(), picosecond);
                if (delayAmount > 0 && answer2HelperScore > 0) return;
            }
            if (delayAmount == 0) Answer1 = severityScore;
        }

        private void CalculateAnswer2()
        {
            delayAmount = -1;
            while (answer2HelperScore != 0) {
                severityScore = 0;
                delayAmount++;
                CalculateAnswerA();
                
            }

            Answer2 = delayAmount;
        }

        public void CalculateAnswerB()
        {
            throw new NotImplementedException();
        }
    }
}
