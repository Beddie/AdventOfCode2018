using Logic.Properties;
using Logic.Service.Pathfinder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Logic.Days
{
    public class Day15 : AdventBase
    {
        public Day15()
        {
            Test = true;
            PuzzleInput = Test ? Resources.Day15Example : Resources.Day15;
            ID = 15;
            Name = "Day 15: Beverage Bandits";
        }

        public override string[] Solution()
        {
            return new string[] { "269430", "55160" };
        }

        public override string Part1()
        {
            var game = new Game(PuzzleInput);
            while (!game.GameOver)
            {
                game.Round();
                if (!game.GameOver) game.FullRoundNumber++;
                if (Test) game.PrintGame(true);
            }
            return game.Part1and2.ToString();
        }

        public override string Part2()
        {
            var attackPower = 3;
            var elfsSurvive = false;
            var score = 0;
            while (!elfsSurvive)
            {
                attackPower++;
                var game = new Game(PuzzleInput, attackPower);

                while (!game.GameOver)
                {
                    game.Round();
                    if (!game.GameOver) game.FullRoundNumber++;
                }

                if (game.AllElfsLeft)
                {
                    elfsSurvive = true;
                    score = (int)game.Part1and2;
                }
            }
            return score.ToString();
        }

        private class Game
        {
            public Game(string puzzleInput, int? attackPowerPart2 = null)
            {
                ElfAttackPower = attackPowerPart2.HasValue ? attackPowerPart2.Value : 3;
                Stride = puzzleInput.Substring(0, puzzleInput.IndexOf("\r\n")).Count();
                GameObjects = puzzleInput.Replace("\r\n", "").Replace(" ", "").Select((c, index) => CreateGameObject(c, index)).ToList();
                AmountElfsStart = GameObjects.Where(WhereUnitIsElf()).Count();
            }

            public int AmountElfsStart { get; set; }
            public int ElfAttackPower { get; set; }
            public int FullRoundNumber { get; set; }
            public bool GameOver => !GameObjects.Where(WhereUnitIsGoblin()).Any() || !GameObjects.Where(WhereUnitIsElf()).Any();
            public bool AllElfsLeft => GameObjects.Where(WhereUnitIsElf()).Count() == AmountElfsStart;
            public double Part1and2 => (FullRoundNumber) * TotalUnitHp;
            public void PrintGame(bool scores) { Print(scores); }

            public static int Stride { get; set; }
            public static List<GameObject> GameObjects { get; set; }
            public static Func<GameObject, Unit> SelectUnits() => (o) => (o as Unit);
            public static Func<GameObject, bool> WhereUnitObjects = (o => o is Unit);
            public static Func<GameObject, bool> WhereUnitIsElf() => c => (c is Unit) && (c as Unit).Type == UnitType.Elf;
            public static Func<GameObject, bool> WhereUnitIsGoblin() => c => (c is Unit) && (c as Unit).Type == UnitType.Goblin;
            public static Func<GameObject, bool> WhereAdjacentOpenCavern(Point index) => c => IsAdjacent(c.Index, index) && (((c is Structure) && (c as Structure).Type == StructureType.OpenCavern) || ((c is Unit) && (c as Unit).Dead));
            public static sbyte[,] direction = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

            public delegate Func<GameObject, bool> WhereGameObject();

            private int TotalUnitHp => GameObjects.Where(WhereUnitObjects).Select(SelectUnits()).Sum(e => e.HP);

            public void Round()
            {
                foreach (var unit in GameObjects.Where(WhereUnitObjects).OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X).Select(SelectUnits()).Where(c => !c.Dead))
                {
                    unit.Act();
                }
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
                        return new Unit(UnitType.Elf, WhereUnitIsGoblin) { Index = newIndex, Power = ElfAttackPower };
                    case 'G':
                        return new Unit(UnitType.Goblin, WhereUnitIsElf) { Index = newIndex };
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
                [Description("Elf")]
                Elf = 1,
                [Description("Goblin")]
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
                public int Power { get; set; } = 3;
                public List<Point> IndexRanges => GetAdjacentOpenCavernForUnit();
                public bool Dead => HP <= 0;

                public void Act()
                {
                    if (!Dead)
                    {
                        //find enemies
                        var enemies = GameObjects.Where(Enemies()).Select(SelectUnits()).Where(c => !c.Dead);
                        //Find closest enemy and walk or battle
                        FightOrWalkToFirstEnemyBattleGround(enemies.ToList());
                    }
                }

                private List<Unit> GetAdjacentBattleUnitsForUnit(Point? index = null)
                {
                    var battleUnit = GameObjects.Where(Enemies()).Where(c => IsAdjacent(Index, c.Index)).Select(SelectUnits()).Where(c => !c.Dead).ToList();
                    return battleUnit.Any() ? battleUnit : new List<Unit>();
                }

                private List<Point> GetAdjacentOpenCavernForUnit(Point? index = null)
                {
                    var openCavern = GameObjects.Where(WhereAdjacentOpenCavern(index.HasValue ? index.Value : Index)).Select(c => c.Index).ToList();
                    return openCavern.Any() ? openCavern : new List<Point>();
                }

                private class EnemyPath
                {
                    public EnemyPath(Point point, Unit enemy, List<PathFinderNode> path)
                    {
                        Point = point;
                        Enemy = enemy;
                        Path = path;
                    }

                    public Point Point { get; set; }
                    public Unit Enemy { get; set; }
                    public List<PathFinderNode> Path { get; set; }
                }

                private List<EnemyPath> FindPathsAndDetermineFastestPathToEnemy(List<Point> adjacentSquares, List<Unit> enemyList)
                {
                    //build grid
                    var maxX = GameObjects.Max(c => c.Index.X);
                    var maxY = GameObjects.Max(c => c.Index.Y);
                    var grid = new byte[maxX + 1, maxY + 1];

                    GameObjects.ForEach((go) =>
                    {
                        grid[go.Index.X, go.Index.Y] = ((((go is Structure) && (go as Structure).Type == StructureType.OpenCavern)) || (((go is Unit) && (go as Unit).Dead))) ? (byte)1 : (byte)0;

                    });
                    List<EnemyPath> enemyPaths = new List<EnemyPath>();
                    foreach (var square in adjacentSquares)
                    {
                        foreach (var enemy in enemyList)
                        {
                            foreach (var indexInRange in enemy.IndexRanges)
                            {
                                var mPathFinder = new PathFinder(grid);
                                List<PathFinderNode> path = mPathFinder.FindPath(square, indexInRange);
                                if (path != null) enemyPaths.Add(new EnemyPath(new Point(path.First().X, path.First().Y), enemy, path));
                            }
                        }
                    }
                    return enemyPaths?? null;
                }

                private void FightOrWalkToFirstEnemyBattleGround(List<Unit> enemyList)
                {
                    var openQuares = GetAdjacentOpenCavernForUnit().OrderBy(c => c.Y).ThenBy(c => c.X).ToList();
                    var battleUnits = GetAdjacentBattleUnitsForUnit().Where(c => !c.Dead).OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X);
                    if (battleUnits.Any()) Fight(battleUnits.ToList());
                    else
                    {
                        var enemyPaths = FindPathsAndDetermineFastestPathToEnemy(openQuares.ToList(), enemyList);
                        //Swap by ref
                        if (enemyPaths != null && enemyPaths.Any())
                        {
                            var shortestPaths = enemyPaths.Where(c => c.Path.Count == enemyPaths.Min(d => d.Path.Count())).ToList();
                            var shortestPathToUnit = shortestPaths.OrderBy(c => c.Path.Last().Y).ThenBy(c => c.Path.Last().X).First();
                            var opensquare = GameObjects.First(c => c.Index == shortestPathToUnit.Point);
                            opensquare.Index = new Point(Index.X, Index.Y);
                            Index = shortestPathToUnit.Point;
                            battleUnits = GetAdjacentBattleUnitsForUnit().Where(c => !c.Dead).OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X);
                            if (battleUnits.Any()) Fight(battleUnits.ToList());
                        }
                    }
                }

                private void Fight(List<Unit> battleUnits)
                {
                    Unit enemyWithLowestHP;
                    var enemyWithLowestHPs = battleUnits.Where(c => c.HP == battleUnits.Min(d => d.HP)).ToList();

                    enemyWithLowestHP = enemyWithLowestHPs.OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X).First();
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

            private void Print(bool scores)
            {
                var sb = new StringBuilder();
                var maxX = GameObjects.Max(c => c.Index.X);
                var maxY = GameObjects.Max(c => c.Index.Y);
                var y = 0;

                var units = GameObjects.Where(WhereUnitObjects).Select(SelectUnits()).OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X).ToList();
                foreach (var item in GameObjects.OrderBy(c => c.Index.Y).ThenBy(c => c.Index.X))
                {
                    var printChar = '/';
                    if (item is Structure && (item as Structure).Type == StructureType.Wall) printChar = '#';
                    else if (item is Structure && (item as Structure).Type == StructureType.OpenCavern) printChar = '.';
                    else if (item is Unit && (item as Unit).Type == UnitType.Elf) printChar = 'E';
                    else if (item is Unit && (item as Unit).Type == UnitType.Goblin) printChar = 'G';


                    if (y != item.Index.Y)
                    {
                        if (units.Count() > y) sb.Append($"    {EnumUnits[(int)units[y].Type]} - {units[y].HP}");
                        sb.AppendLine();
                        y++;
                    };
                    sb.Append(printChar);
                }

                while (units.Count() > y)
                {
                    sb.AppendLine($"    {EnumUnits[(int)units[y].Type]} - {units[y].HP}");
                    y++;
                }
                sb.AppendLine();

                Debug.WriteLine(sb.ToString());
            }

            private Dictionary<int, string> EnumUnits = EnumDictionary<UnitType>();

            #endregion
        }
    }
}