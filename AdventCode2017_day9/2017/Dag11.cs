using AdventCode;
using AdventCode.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCode2017
{
    public class Dag11 : AdventBase, AdventInterface
    {
        private static bool test = true;
        private int x = 0;
        private int y = 0;
        private int z = 0;

        private enum Direction
        {
            North,
            NorthEast,
            SouthEast,
            South,
            SouthWest,
            NorthWest
        }

        private string text = test ? "se,sw,se,sw,sw" : Resources.dag11_2017;
        private List<Direction> baseList = new List<Direction>();
        private List<DirectionCondition> conditionList = new List<DirectionCondition>();

        private class DirectionCondition{
            public Direction Direction { get; set; }
            public Direction OppositeDirection { get; set; }
            public Direction NeighbourA { get; set; }
            public Direction NeighbourB { get; set; }
        }
        //ne,ne,ne is 3 steps away.
        //ne, ne, sw, sw is 0 steps away (back where you started).
        //ne,ne,s,s is 2 steps away(se, se).
        //se,sw,se,sw,sw is 3 steps away(s, s, sw).

        public Dag11()
        {
            InitializeBaseList();
            InitializeConditionList();
            Calculate();
            WriteDebugAnswers(this);
        }

        private Direction GetDirectionFromString(string direction)
        {
            switch (direction)
            {
                case "n":
                    return Direction.North;
                case "ne":
                    return Direction.NorthEast;
                case "nw":
                    return Direction.NorthWest;
                case "s":
                    return Direction.South;
                case "se":
                    return Direction.SouthEast;
                case "sw":
                    return Direction.SouthWest;

                default:
                    throw new NotSupportedException();

            }
        }

        private void InitializeBaseList()
        {

            text.Split(',').AsParallel().ForAll(c => CalcDirection(c));

            //baseList.Add(GetDirectionFromString(c)));
        }

        private void CalcDirection(string c) {

            switch (c)
            {
                case "n":
                    y++;
                    break;
                case "ne":
                    y++; x++;
                    break;
                case "nw":
                    y++; x--;
                    break;
                case "s":
                    y--;
                    break;
                case "se":
                    y--; x++;
                    break;
                case "sw":
                    y--; x--;
                    break;

                default:
                    throw new NotSupportedException();

            }
        }

        private void InitializeConditionList() {
            conditionList.Add(new DirectionCondition() { Direction = Direction.North, OppositeDirection = Direction.South, NeighbourA = Direction.NorthWest, NeighbourB = Direction.NorthEast });
            conditionList.Add(new DirectionCondition() { Direction = Direction.NorthEast, OppositeDirection = Direction.SouthWest, NeighbourA = Direction.North, NeighbourB = Direction.SouthEast });
            conditionList.Add(new DirectionCondition() { Direction = Direction.SouthEast, OppositeDirection = Direction.NorthWest, NeighbourA = Direction.NorthEast, NeighbourB = Direction.South });
            conditionList.Add(new DirectionCondition() { Direction = Direction.South, OppositeDirection = Direction.North, NeighbourA = Direction.SouthEast, NeighbourB = Direction.SouthWest });
            conditionList.Add(new DirectionCondition() { Direction = Direction.SouthWest, OppositeDirection = Direction.NorthEast, NeighbourA = Direction.South, NeighbourB = Direction.NorthWest });
            conditionList.Add(new DirectionCondition() { Direction = Direction.NorthWest, OppositeDirection = Direction.SouthEast, NeighbourA = Direction.SouthWest, NeighbourB = Direction.North });
        }

        public void Calculate()
        {
            foreach (var condition in conditionList)
            {
                //Eliminate directions, so 3 directions remain
                
                var conditionBase = baseList.Select(c => c == condition.Direction).ToList();
                var conditionOpposite = baseList.Select(c => c == condition.OppositeDirection).ToList();

                var baseDirLarger = conditionBase.Count() > conditionOpposite.Count();
                var minimumDirectionList = baseList.Where(c => c == (baseDirLarger ? condition.OppositeDirection : condition.Direction)).ToList();
                baseList.RemoveAll(c=> c == condition.Direction);
                baseList.RemoveAll(c => c == condition.OppositeDirection);
                baseList.AddRange(baseList.Where(c => c == (baseDirLarger ? condition.OppositeDirection : condition.Direction)).ToList());
            }

            //Answer1 = baseList.Take(1).First() * baseList.Skip(1).First();
            //Answer2 = garbageScore;
        }
    }
}
