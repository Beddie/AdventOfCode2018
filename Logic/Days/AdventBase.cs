using Logic.ExtensionMethods;
using Logic.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class AdventBase : AdventInterface
    {
        public bool Active => Solution().Any();
        public int ID { get; set; }
        public string Name { get; set; }
        public object Answer1 { get; set; }
        public object Answer2 { get; set; }
        public static bool Test { get; set; }
        public string PuzzleInput {get; set;}

        public void WriteDebugAnswers(object className)
        {
            Debug.WriteLine($"{className.GetType().Name} Antwoord 1: {Answer1}");
            Debug.WriteLine($"{className.GetType().Name} Antwoord 2: {Answer2}");
        }

        public void WriteDebug<T>(T write, object className)
        {
            Debug.WriteLine($"{className.GetType().Name}: {write}");
        }

        public static Dictionary<int, string> EnumDictionary<T>()
        {

            if (!typeof(T).IsEnum)
                throw new ArgumentException("Type must be an enum");
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .ToDictionary(t => (int)(object)t, t => t.ToDescriptionString());
        }

        public virtual string[] Solution()
        {
            throw new NotImplementedException();
        }

        public virtual string Part1()
        {
            throw new NotImplementedException();
        }

        public virtual string Part2()
        {
            throw new NotImplementedException();
        }
    }
}
