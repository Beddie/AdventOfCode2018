using Logic.Interface;
using Logic.Properties;
using Logic.Service.Pathfinder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day15 : AdventBase
    {
        public Day15()
        {
            Test = true;
            //PuzzleInput = @"#######
            //                #.G...#
            //                #.....#
            //                #.#.###
            //                #...#E#
            //                #.....#
            //                #######";

            //PuzzleInput = @"#######
            //                #....G#
            //                #..G..#
            //                #.#.###
            //                #...#E#
            //                #.....#
            //                #######";
            //PuzzleInput = @"#######
            //          #G..#E#
            //          #E#E.E#
            //          #G.##.#
            //          #...#E#
            //          #...E.#
            //          #######";

            PuzzleInput = @"#######
                            #E.G#.#
                            #.#G..#
                            #G.#.G#
                            #G..#.#
                            #...E.#
                            #######";

            //PuzzleInput = Test ? Resources.Day15Example : Resources.Day15;
            ID = 15;
            Name = "Day 15: Beverage Bandits";
        }

        public override string[] Solution()
        {
            return new string[] {
            };
        }


        public override string Part1()
        {
            var game = new Game(PuzzleInput);
            while (!game.GameOver)
            {
                game.FullRoundNumber++;
                game.Round();
                game.PrintGame();
            }
            Debug.WriteLine(game.Part1);

            return game.Part1.ToString();
        }



        private class Game
        {
            public Game(string puzzleInput)
            {
                Stride = puzzleInput.Substring(0, puzzleInput.IndexOf("\r\n")).Count();
                GameObjects = puzzleInput.Replace("\r\n", "").Replace(" ", "").Select((c, index) => CreateGameObject(c, index)).ToList();
            }

            public bool GameOver => !GameObjects.Where(whereUnitIsGoblin()).Any() || !GameObjects.Where(whereUnitIsElf()).Any();
            public double Part1 => (FullRoundNumber) * TotalUnitHp;
            public static int Stride { get; set; }
            public static List<GameObject> GameObjects { get; set; }
            public void PrintGame() { Print(); }
            public delegate Func<GameObject, bool> WhereGameObject();
            public delegate Func<GameObject, Unit> SelectUnit();

            public static Func<GameObject, Unit> selectUnit() => (o) => (o as Unit);
            public static Func<GameObject, bool> whereUnitObjects = (o => o is Unit);
            public static Func<GameObject, bool> whereUnitIsElf() => c => (c is Unit) && (c as Unit).Type == UnitType.Elf;
            public static Func<GameObject, bool> whereUnitIsGoblin() => c => (c is Unit) && (c as Unit).Type == UnitType.Goblin;
            public static Func<GameObject, bool> whereAdjacentOpenCavern(Point index) => c => IsAdjacent(c.Index, index) && (c is Structure) && (c as Structure).Type == StructureType.OpenCavern;
            public static Func<GameObject, bool> whereOpenCavern() => c => c is Structure && (c as Structure).Type == StructureType.OpenCavern;
            public static sbyte[,] direction = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

            public int FullRoundNumber { get; set; }
            private int TotalUnitHp => GameObjects.Where(whereUnitObjects).Select(selectUnit()).Sum(e => e.HP);

            public void Round()
            {
                foreach (var unit in GameObjects.Where(whereUnitObjects).OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X).Select(selectUnit()))
                {
                    unit.Act();
                }
                Debug.WriteLine(FullRoundNumber);
            }

            public static bool IsAdjacent(Point fromindex, Point relatedIndex) =>
                    (relatedIndex.X == fromindex.X && relatedIndex.Y == fromindex.Y + 1) ||
                    (relatedIndex.X == fromindex.X && relatedIndex.Y == fromindex.Y - 1) ||
                    (relatedIndex.X == fromindex.X + 1 && relatedIndex.Y == fromindex.Y) ||
                    (relatedIndex.X == fromindex.X - 1 && relatedIndex.Y == fromindex.Y);

            private GameObject CreateGameObject(char gameObject, int index)
            {
                var x = index % Stride;
                var y = (int)(index / Stride);
                var newIndex = new Point(x, y);
                switch (gameObject)
                {
                    case '#':
                        return new Structure() { Type = StructureType.Wall, Index = newIndex };
                    case '.':
                        return new Structure() { Type = StructureType.OpenCavern, Index = newIndex };
                    case 'E':
                        return new Unit(UnitType.Elf, whereUnitIsGoblin) { Index = newIndex };
                    case 'G':
                        return new Unit(UnitType.Goblin, whereUnitIsElf) { Index = newIndex };
                    default:
                        throw new FormatException("Unknown gameObject");
                }
            }

            public class GameObject
            {
                public Point Index { get; set; }
            }

            public enum UnitType
            {
                Elf = 1,
                Goblin = 2
            }

            public enum StructureType
            {
                OpenCavern = 1,
                Wall = 2
            }

            private class Structure : GameObject
            {
                public StructureType Type { get; set; }
            }

            public class Unit : GameObject
            {
                public Unit(UnitType type, WhereGameObject enemies)
                {
                    Enemies = enemies;
                    Type = type;
                }

                public UnitType Type { get; set; }
                public WhereGameObject Enemies { get; set; }
                public int HP { get; set; } = 200;
                private int Power { get; set; } = 3;

                public List<Point> IndexRanges => GetAdjacentOpenCavernForUnit();
                public bool Dead => HP < 0;

                public void Act()
                {
                    //find enemies
                    var enemies = GameObjects.Where(Enemies()).Select(selectUnit());
                    //FIndclosest Enemy
                    FightOrWalkToFirstEnemyBattleGround(enemies.ToList());
                }

                public List<Unit> GetAdjacentBattleUnitsForUnit(Point? index = null)
                {
                    var battleUnit = GameObjects.Where(Enemies()).Where(c => IsAdjacent(Index, c.Index)).Select(selectUnit()).ToList();
                    return battleUnit.Any() ? battleUnit : new List<Unit>();
                }

                private List<Point> GetAdjacentOpenCavernForUnit(Point? index = null)
                {
                    var openCavern = GameObjects.Where(whereAdjacentOpenCavern(index.HasValue ? index.Value : Index)).Select(c => c.Index).ToList();
                    return openCavern.Any() ? openCavern : new List<Point>();
                }

                private (Point, Unit) FindPathsAndDetermineFastestPathToEnemy(List<Point> adjacentSquares, List<Unit> enemyList)
                {
                    //build grid
                    var maxX = GameObjects.Max(c => c.Index.X);
                    var maxY = GameObjects.Max(c => c.Index.Y);
                    var grid = new byte[maxX + 1, maxY + 1];

                    GameObjects.ForEach((go) =>
                    {
                        grid[go.Index.X, go.Index.Y] = (enemyList.Contains(go) || (((go is Structure) && (go as Structure).Type == StructureType.OpenCavern))) ? (byte)1 : (byte)0;

                    });
                    //PrintGrid(grid);
                    List<PathFinderNode> shortestPath = null;
                    Unit choosenEnemy = null;
                    foreach (var square in adjacentSquares)
                    {
                        foreach (var enemy in enemyList)
                        {
                            var mPathFinder = new PathFinder(grid);
                            List<PathFinderNode> path = mPathFinder.FindPath(square, enemy.Index); //Dont use last column! or last row!

                            if (path != null)
                            {
                                //PrintPath(grid, path);
                                if (shortestPath == null || shortestPath.Count() > path.Count())
                                {
                                    shortestPath = path;
                                    choosenEnemy = enemy;
                                }
                            }
                        }
                    }
                    return choosenEnemy == null ? (new Point(), null) : (new Point(shortestPath.First().X, shortestPath.First().Y), choosenEnemy);
                }

                private void FightOrWalkToFirstEnemyBattleGround(List<Unit> enemyList)
                {
                    var openQuares = GetAdjacentOpenCavernForUnit().OrderBy(c => c.Y).ThenBy(c => c.X).ToList(); // => new Path(index + 1, c));
                    var battleUnits = GetAdjacentBattleUnitsForUnit().OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X); // => new Path(index + 1, c));
                    if (battleUnits.Any()) Fight(battleUnits);
                    else
                    {
                        var PointUnit = FindPathsAndDetermineFastestPathToEnemy(openQuares.ToList(), enemyList);
                        //Swap by ref
                        if (PointUnit.Item2 != null)
                        {
                            var opensquare = GameObjects.First(c => c.Index == PointUnit.Item1);
                            opensquare.Index = Index;
                            Index = PointUnit.Item1;
                            battleUnits = GetAdjacentBattleUnitsForUnit().OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X);
                            if (battleUnits.Any()) Fight(battleUnits);
                        }
                    }
                }

                private void Fight(IOrderedEnumerable<Unit> battleUnits)
                {
                    var enemyWithLowestHP = battleUnits.Where(c => c.HP == battleUnits.Min(d => d.HP)).First();
                    enemyWithLowestHP.HP -= Power;

                    if (enemyWithLowestHP.Dead)
                    {
                        var openCavern = new Structure() { Index = enemyWithLowestHP.Index, Type = StructureType.OpenCavern };
                        GameObjects.Remove(GameObjects.First(c => c.Index == enemyWithLowestHP.Index));
                        GameObjects.Add(openCavern);
                    }
                }

                #region Print            
                private void PrintGrid(byte[,] grid)
                {
                    var sb = new StringBuilder();
                    var maxX = 6;
                    var maxY = 6;
                    for (int y = 0; y < maxY; y++)
                    {
                        for (int x = 0; x < maxX; x++)
                        {
                            sb.Append(grid[x, y]);
                        }
                        sb.AppendLine();
                    }
                    Debug.WriteLine(sb.ToString());
                }

                private void PrintPath(byte[,] grid, List<PathFinderNode> points)
                {
                    var sb = new StringBuilder();
                    var maxX = GameObjects.Max(c => c.Index.X);
                    var maxY = GameObjects.Max(c => c.Index.Y);
                    var y = 0;
                    foreach (var item in GameObjects.OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X))
                    {
                        var printChar = '/';
                        if (item is Structure && (item as Structure).Type == StructureType.Wall) printChar = '#';
                        else if (item is Structure && (item as Structure).Type == StructureType.OpenCavern)
                        {
                            if (points.Where(c => c.X == item.Index.X && c.Y == item.Index.Y).Any()) printChar = 'o';
                            else printChar = '.';

                        }
                        else if (item is Unit && (item as Unit).Type == UnitType.Elf) printChar = 'E';
                        else if (item is Unit && (item as Unit).Type == UnitType.Goblin) printChar = 'G';


                        if (y != item.Index.Y) { sb.AppendLine(); y++; };
                        sb.Append(printChar);

                    }
                    sb.AppendLine();
                    Debug.WriteLine(sb.ToString());
                }
            }

            private void Print()
            {
                var sb = new StringBuilder();
                var maxX = GameObjects.Max(c => c.Index.X);
                var maxY = GameObjects.Max(c => c.Index.Y);
                var y = 0;
                foreach (var item in GameObjects.OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X))
                {
                    //count++;
                    //if (item.Index.X == 0 && item.Index.Y > 0) sb.AppendLine();
                    var printChar = '/';
                    if (item is Structure && (item as Structure).Type == StructureType.Wall) printChar = '#';
                    else if (item is Structure && (item as Structure).Type == StructureType.OpenCavern) printChar = '.';
                    else if (item is Unit && (item as Unit).Type == UnitType.Elf) printChar = 'E';
                    else if (item is Unit && (item as Unit).Type == UnitType.Goblin) printChar = 'G';


                    if (y != item.Index.Y) { sb.AppendLine(); y++; };
                    sb.Append(printChar);

                }
                Debug.WriteLine(sb.ToString());
            }
            #endregion
        }

        public override string Part2()
        {
            return "";
        }
    }
}

