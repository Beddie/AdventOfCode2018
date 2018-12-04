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
    public class Day4 : AdventBase, AdventInterface
    {
        public Day4()
        {
            // Test = true;
            PuzzleInput = Test ? Resources.Day4Example : Resources.Day4;
        }

        public string[] Solution()
        {
            return new string[] {
                "12504",
                "139543"
            };
        }
        public class Period
        {
            public DateTime startTime { get; set; }
            public DateTime? endTime { get; set; }
        }

        private class Guard
        {
            public int ID { get; set; }
            public HashSet<Period> SleepPeriods { get; set; }
        }

        private enum ShiftEvent
        {
            Start,
            WakesUp,
            FallsAsleep
        }

        private class Shift
        {
            public Guard GuardOnDuty { get; set; }
            public DateTime TimeOfEvent { get; set; }
            public ShiftEvent ShiftEvent { get; set; }
        }

        public string Part1()
        {
            var shifts = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.ToCharArray()).Select(c =>
                new Shift()
                {
                    TimeOfEvent = Convert.ToDateTime(new string(c.Skip(1).Take(16).ToArray())),
                    ShiftEvent = GetShiftEvent(c.Skip(19).First()),
                    GuardOnDuty = GetShiftEvent(c.Skip(19).First()) == ShiftEvent.Start ? new Guard() { ID = Convert.ToInt32(new string(c.SkipWhile(d => d != '#').Skip(1).TakeWhile(d => d != ' ').ToArray())) } : null
                });

            //Order shifts
            shifts = shifts.OrderBy(c => c.TimeOfEvent);

            var allGuardsShifts = new HashSet<Guard>();
            Guard guard = null;

            foreach (var shift in shifts)
            {
                switch (shift.ShiftEvent)
                {
                    case ShiftEvent.Start:
                        if (guard != null) allGuardsShifts.Add(guard);
                        guard = shift.GuardOnDuty;
                        guard.SleepPeriods = new HashSet<Period>();
                        break;
                    case ShiftEvent.WakesUp:
                        guard.SleepPeriods.First(c => !c.endTime.HasValue).endTime = shift.TimeOfEvent.AddMinutes(0);
                        break;
                    case ShiftEvent.FallsAsleep:
                        guard.SleepPeriods.Add(new Period() { startTime = shift.TimeOfEvent });
                        break;
                    default:
                        throw new NotSupportedException("Event does not exist");
                }
            }

            var groupedGuards = allGuardsShifts.GroupBy(c => c.ID).Select(c => new Guard() { ID = c.Key,  SleepPeriods = c.SelectMany(d => d.SleepPeriods).ToHashSet() }).ToHashSet();
            var aslaap = groupedGuards.Select(c => new { guardID = c.ID, minutesAsleep = AmountMinutes(c.SleepPeriods) });
            var guardMostAsleep = aslaap.Where(c => c.minutesAsleep == aslaap.Max(d => d.minutesAsleep)).First();

            //which minute?
            var guardToEvaluate = groupedGuards.Where(c => c.ID == guardMostAsleep.guardID).First();
            var peakMinute = peakMinuteAsleep(guardToEvaluate.SleepPeriods);
            return (peakMinute * guardToEvaluate.ID).ToString();
        }

        private int peakMinuteAsleep(HashSet<Period> sleepPeriods)
        {
            var minutes = new List<int>();
            foreach (var period in sleepPeriods)
            {
                minutes.AddRange(Enumerable.Range(period.startTime.Minute, (period.endTime.Value - period.startTime).Minutes));
            }
            return minutes.GroupBy(i => i).OrderByDescending(g => g.Count()).Take(1).Select(g => g.Key).First();
        }

        private int AmountMinutes(HashSet<Period> sleepPeriods)
        {
            var minutes = 0;
            foreach (var period in sleepPeriods)
            {
                minutes += (period.endTime.Value - period.startTime).Minutes;
            }
            return minutes;
        }

        private ShiftEvent GetShiftEvent(char charIndicator)
        {
            switch (charIndicator)
            {
                case 'G':
                    return ShiftEvent.Start;
                case 'w':
                    return ShiftEvent.WakesUp;
                case 'f':
                    return ShiftEvent.FallsAsleep;
                default:
                    throw new NotImplementedException("Event does not exist");
            }
        }

        public string Part2()
        {
            //Strategy 2: Of all guards, which guard is most frequently asleep on the same minute?
            //In the example above, Guard #99 spent minute 45 asleep more than any other guard or minute - three times in total. 
            //(In all other cases, any guard spent any minute asleep at most twice.)
            //What is the ID of the guard you chose multiplied by the minute you chose ? (In the above example, the answer would be 99 * 45 = 4455.)
            var shifts = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(c => c.ToCharArray()).Select(c =>
               new Shift()
               {
                   TimeOfEvent = Convert.ToDateTime(new string(c.Skip(1).Take(16).ToArray())),
                   ShiftEvent = GetShiftEvent(c.Skip(19).First()),
                   GuardOnDuty = GetShiftEvent(c.Skip(19).First()) == ShiftEvent.Start ? new Guard() { ID = Convert.ToInt32(new string(c.SkipWhile(d => d != '#').Skip(1).TakeWhile(d => d != ' ').ToArray())) } : null
               });

            //Order shifts
            shifts = shifts.OrderBy(c => c.TimeOfEvent);

            var allGuardsShifts = new HashSet<Guard>();
            Guard guard = null;

            foreach (var shift in shifts)
            {
                switch (shift.ShiftEvent)
                {
                    case ShiftEvent.Start:
                        if (guard != null) allGuardsShifts.Add(guard);
                        guard = shift.GuardOnDuty;
                        guard.SleepPeriods = new HashSet<Period>();
                        break;
                    case ShiftEvent.WakesUp:
                        guard.SleepPeriods.First(c => !c.endTime.HasValue).endTime = shift.TimeOfEvent.AddMinutes(0);
                        break;
                    case ShiftEvent.FallsAsleep:
                        guard.SleepPeriods.Add(new Period() { startTime = shift.TimeOfEvent });
                        break;
                    default:
                        throw new NotSupportedException("Event does not exist");
                }
            }
            allGuardsShifts.Add(guard);

            //Group periods per Guard
            var guardsMostOccuentMinuteAsleep = allGuardsShifts
                .GroupBy(c => c.ID)
                .Select(c => new { ID = c.Key, SleepPeriods = c.SelectMany(d => d.SleepPeriods).ToHashSet() })
                .Select(c => new { guardID = c.ID, sleepMinutes = MostOccurentMinute(c.SleepPeriods) }).ToHashSet();
            var guardMostAsleep = guardsMostOccuentMinuteAsleep
                .Where(c => c.sleepMinutes.Item2 == guardsMostOccuentMinuteAsleep.Max(d => d.sleepMinutes.Item2))
                .Select(c => new { sleepMinute = c.sleepMinutes.Item1, c.guardID }).First();

            return (guardMostAsleep.guardID * guardMostAsleep.sleepMinute).ToString();
        }

        private (int, int) MostOccurentMinute(HashSet<Period> sleepPeriods)
        {
            if (sleepPeriods.Any())
            {
                var minutes = new List<int>();
                foreach (var period in sleepPeriods)
                {
                    minutes.AddRange(Enumerable.Range(period.startTime.Minute, (period.endTime.Value - period.startTime).Minutes));
                }
                var allMinutes = minutes.GroupBy(i => i).Select(g => (g.Key, g.Count()));

                var maxMinute = allMinutes.Where(c => c.Item2 == allMinutes.Select(d => d.Item2).Max());
                return maxMinute.First();
            }
            else
            {
                return (0, 0);
            }
        }

        public string GetListName()
        {
            return "Day 4: Repose Record";
        }
        public int GetID()
        {
            return 4;
        }
    }
}

