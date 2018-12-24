using Logic.ExtensionMethods;
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
    public class Day24 : AdventBase
    {
        public Day24()
        {
            Test = true;
            PuzzleInput = Test ? Resources.Day24Example : Resources.Day24;
            ID = 24;
            Name = "Day 24: Immune System Simulator 20XX";
        }

        public override string[] Solution()
        {
            return new string[] {
            };
        }


        private class Game
        {
            public Game(int attackPowerBoost, List<UnitGroup> unitGroups)
            {
                AttackpowerBoost = attackPowerBoost;

                unitGroups.Where(c => c.UnitType == UnitType.Immune_System).ToList().ForEach(c => c.Attackpower += attackPowerBoost);
                UnitGroups = unitGroups;

            }

            public Game()
            {
            }

            public int AttackpowerBoost { get; set; }
            public List<UnitGroup> UnitGroups = new List<UnitGroup>();
            public bool GameOver => !UnitGroups.Where(c => c.UnitType == UnitType.Immune_System).Any() || !UnitGroups.Where(c => c.UnitType == UnitType.Infection).Any();
            public bool GameOverPart2 => UnitGroups.Where(c => c.UnitType == UnitType.Immune_System).Any() && !UnitGroups.Where(c => c.UnitType == UnitType.Infection).Any();

            internal string Part1()
            {
                return UnitGroups.Sum(c => c.AmountUnits).ToString();
            }
            internal string ImmuneUnitsAmount()
            {
                return UnitGroups.Where(c => c.UnitType == UnitType.Immune_System).Sum(c => c.AmountUnits).ToString();
            }
            internal string InfectionUnitAmount()
            {
                return UnitGroups.Where(c => c.UnitType == UnitType.Infection).Sum(c => c.AmountUnits).ToString();
            }
        }

        #region Part1


        public override string Part1()
        {
            //var testString = @"18 units each with 729 hit points (weak to fire; immune to cold, slashing) with an attack that does 8 radiation damage at initiative 10";
            var game = new Game();
            var digitRegex = new Regex(@"-?\d+");

            var puzzleLines = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None);

            string type = string.Empty;
            foreach (var line in puzzleLines)
            {
                if (line.Length > 0 && line.Length < 50)
                {
                    type = line;
                }
                else if (line.Length == 0)
                {
                    //do nothing
                }
                else
                {
                    var unitGroup = new UnitGroup(type, line);
                    game.UnitGroups.Add(unitGroup);
                }
            }

            while (!game.GameOver)
            {
                game.UnitGroups.ForEach((c) => { c.UnitToAttack = null; c.UnitToDefend = null; });
                //Choosing Phase

                var unitList = game.UnitGroups.OrderByDescending(c => c.EffectivePower).ThenByDescending(c => c.Initiative).ToList();
                for (int i = 0; i < unitList.Count; i++)
                {
                    if (!unitList[i].GameOver)
                    {
                        unitList[i].ChooseTarget(game.UnitGroups);
                    }
                }

                var attackUnitList = unitList.OrderByDescending(c => c.Initiative).ToList();
                for (int i = 0; i < attackUnitList.Count; i++)
                {
                    if (!attackUnitList[i].GameOver)
                    {
                        attackUnitList[i].Attack();
                    }
                }
                game.UnitGroups.RemoveAll(c => c.GameOver);
                //Print(game.UnitGroups);
            }
            return game.Part1();
        }
        #endregion

        public override string Part2()
        {

            var attackPowerBoost = 0; //23950
            var amountHp = "";
            while (string.IsNullOrEmpty(amountHp))
            {
                var hpInfection = "";
                var digitRegex = new Regex(@"-?\d+");

                var puzzleLines = PuzzleInput.Split(new[] { "\r\n" }, StringSplitOptions.None);

                var unitGroups = new List<UnitGroup>();
                string type = string.Empty;
                foreach (var line in puzzleLines)
                {
                    if (line.Length > 0 && line.Length < 50)
                    {
                        type = line;
                    }
                    else if (line.Length == 0)
                    {
                        //do nothing
                    }
                    else
                    {
                        var unitGroup = new UnitGroup(type, line);
                        unitGroups.Add(unitGroup);
                    }
                }

                var game = new Game(attackPowerBoost, unitGroups);


                while (!game.GameOver)
                {
                    var killedAny = false;
                    game.UnitGroups.ForEach((c) => { c.UnitToAttack = null; c.UnitToDefend = null; });
                    //Choosing Phase

                    var unitList = game.UnitGroups.OrderByDescending(c => c.EffectivePower).ThenByDescending(c => c.Initiative).ToList();
                    for (int i = 0; i < unitList.Count; i++)
                    {
                        if (!unitList[i].GameOver)
                        {
                            unitList[i].ChooseTarget(game.UnitGroups);
                        }
                    }

                    var attackUnitList = unitList.OrderByDescending(c => c.Initiative).ToList();
                    for (int i = 0; i < attackUnitList.Count; i++)
                    {
                        if (!attackUnitList[i].GameOver)
                        {
                            if (attackUnitList[i].Attack()) killedAny = true;
                        }
                    }
                    game.UnitGroups.RemoveAll(c => c.GameOver);
                    if (!killedAny) break;
                    //Print(game.UnitGroups);
                }
             
                if (game.GameOverPart2)
                {
                    amountHp = game.Part1();
                }

                Debug.WriteLine($"Roundover, Boost: {attackPowerBoost}, HpInfectionAmount: {game.InfectionUnitAmount()}, HpImmuneAmount: {game.ImmuneUnitsAmount()}, Stopped: {hpInfection == game.InfectionUnitAmount()}");
                attackPowerBoost += 1;
                //var bb = game.Part1();
            }
            return amountHp; //24237 TO HIGH
        }

        private enum AttackType
        {
            slashing = 1,
            cold = 2,
            radiation = 3,
            fire = 4,
            bludgeoning = 5,
            none = 6
        }

        public enum UnitType
        {
            Immune_System = 1,
            Infection = 2,
        }

        private class UnitGroup
        {
            public int Attackpower { get; set; }

            public int Initiative { get; set; }
            public AttackType AttackType { get; set; }
            public UnitType UnitType { get; set; }
            public UnitType EnemyUnitType { get; set; }
            public List<AttackType> ImmuneTo { get; set; } = new List<AttackType>();
            public List<AttackType> WeakTo { get; set; } = new List<AttackType>();
            public int AmountUnits => Units.Count();
            public int AmountHP => Units.Sum(c => c.HP);
            public List<Unit> Units { get; set; } = new List<Unit>();
            public bool GameOver => !Units.Any();


            public int EffectivePower => AmountUnits * Attackpower;

            public UnitGroup UnitToAttack { get; set; }
            public UnitGroup UnitToDefend { get; set; }


            public UnitGroup(string type, string line)
            {
                var digitRegex = new Regex(@"-?\d+");

                if (line.Contains("("))
                {
                    var character = new string(line.Skip(line.IndexOf('(') + 1).TakeWhile(c => c != ')').ToArray());

                    //., line.IndexOf(')')); // (immune to slashing; weak to bludgeoning, radiation)

                    var immuneIndexString = "immune to ";
                    var weakIndexString = "weak to ";


                    var characterSubstring = character.Split(';'); // line.Substring(line.IndexOf(immuneIndexString) + immuneIndexString.Length, line.IndexOf(")"));
                    foreach (var sub in characterSubstring)
                    {
                        switch (sub.Trim().First())
                        {
                            case 'i':
                                var mysub = sub.Trim().Replace(immuneIndexString, "").Split(',');
                                foreach (var s in mysub)
                                {
                                    ImmuneTo.Add(GetAttackType(s.Trim()));
                                }
                                break;
                            case 'w':
                                var myweakSyb = sub.Trim().Replace(weakIndexString, "").Split(',');
                                foreach (var s in myweakSyb)
                                {
                                    WeakTo.Add(GetAttackType(s.Trim()));
                                }
                                break;
                            default:
                                break;
                        }
                    }

                }
                var damageIndexTo = new string(" damage at initiative".Reverse().ToArray());// .ToString();

                var reverseString = new string(line.Reverse().ToArray());
                var attackType = new string(reverseString.Skip(reverseString.IndexOf(damageIndexTo) + damageIndexTo.Length).TakeWhile(c => c != ' ').Reverse().ToArray());
                var group = digitRegex.Matches(line);
                //var unitGroup = new UnitGroup(type, group[0].Value, group[1].Value, character, group[2].Value, attackType, group[3].Value);

                switch (type)
                {
                    case "Immune System:":
                        UnitType = UnitType.Immune_System;
                        EnemyUnitType = UnitType.Infection;
                        break;
                    case "Infection:":
                        UnitType = UnitType.Infection;
                        EnemyUnitType = UnitType.Immune_System;
                        break;
                    default:
                        break;
                }

                AttackType = GetAttackType(attackType);
                Attackpower = Convert.ToInt32(group[2].Value);
                Initiative = Convert.ToInt32(group[3].Value);

                for (int i = 0; i < Convert.ToInt32(group[0].Value); i++)
                {
                    Units.Add(new Unit(Convert.ToInt32(group[1].Value), Convert.ToInt32(group[2].Value), AttackType, Convert.ToInt32(group[3].Value)));
                }
            }



            private AttackType GetAttackType(string attackType)
            {
                switch (attackType)
                {
                    case "slashing":
                        return AttackType.slashing;
                    case "bludgeoning":
                        return AttackType.bludgeoning;
                    case "cold":
                        return AttackType.cold;
                    case "fire":
                        return AttackType.fire;
                    case "radiation":
                        return AttackType.radiation;
                    default:
                        throw new Exception("Does not exist");
                }
            }

            internal void ChooseTarget(List<UnitGroup> unitGroupList)
            {
                var damageDone = 0;
                List<UnitGroup> unitsToChooseFrom = new List<UnitGroup>();
                foreach (var unit in unitGroupList.Where(c => c.UnitType == EnemyUnitType && c.UnitToDefend == null)) //&& !c.ImmuneTo.Contains(AttackType)
                {
                    var unitDamage = CalculateAttackpower(unit);
                    if (unitDamage > damageDone)
                    {
                        unitsToChooseFrom = new List<UnitGroup>();
                        unitsToChooseFrom.Add(unit);
                        damageDone = unitDamage;
                    }
                    else if (unitDamage == damageDone)
                    {
                        unitsToChooseFrom.Add(unit);
                    }
                }

                //choose 1 target
                unitsToChooseFrom = unitsToChooseFrom.Where(c => c.EffectivePower == unitsToChooseFrom.Max(d => d.EffectivePower)).ToList();
                unitsToChooseFrom = unitsToChooseFrom.Where(c => c.Initiative == unitsToChooseFrom.Max(d => d.Initiative)).ToList();

                if (unitsToChooseFrom.Any())
                {
                    UnitToAttack = unitsToChooseFrom.First();
                    UnitToAttack.UnitToDefend = UnitToAttack;
                }
            }

            private int CalculateAttackpower(UnitGroup unitGroup)
            {
                var testAttackpower = Attackpower;
                if (unitGroup.ImmuneTo.Contains(AttackType))
                {
                    testAttackpower = 0;
                }
                else if (unitGroup.WeakTo.Contains(AttackType))
                {
                    testAttackpower *= 2;
                }

                return testAttackpower * Units.Count();
            }

            internal bool Attack()
            {
                if (UnitToAttack != null)
                {
                    if (UnitToAttack.GameOver) return false;

                    var hitPower = CalculateAttackpower(UnitToAttack);
                    return UnitToAttack.TakeHit(hitPower);
                }
                return false;
            }

            private bool TakeHit(int hitPower)
            {
                var amountHit = hitPower;
                var killUNits = hitPower / Units.First().HP;
                if (killUNits == 0) return false;
                //foreach (var unit in Units.OrderBy(c => c.HP))
                //{
                //    amountHit -= unit.Hit(amountHit);
                //}

                //var killed = Units.Any(c => c.Dead);
                if (killUNits > Units.Count) killUNits = Units.Count;
                Units.RemoveRange(0, killUNits);
                //Units.RemoveAll(c => c.Dead);

               

                return true;
                //if (amountHit == 0) break;
            }
        }



        private class Unit
        {
            public Unit(int hp, int attackpower, AttackType attacktype, int initiative)
            {
                HP = hp;
                Attackpower = attackpower;
                AttackType = attacktype;
                Initiative = initiative;
            }

            public int HP { get; set; }
            public int Attackpower { get; set; }
            public AttackType AttackType { get; set; }
            public int Initiative { get; set; }
            public bool Dead => HP <= 0;

            internal int Hit(int amountHit)
            {
                var takeDamage = HP - amountHit;
                int damageTaken = 0;

                if (takeDamage <= 0)
                {
                    damageTaken = HP;
                    HP = 0;
                }
                else
                {
                    //damageTaken = takeDamage;
                    //HP -= takeDamage;
                    // return HP;
                }
                return damageTaken;
            }
        }

        private void Print(List<UnitGroup> unitGroups)
        {
            var sb = new StringBuilder();
            foreach (var unit in unitGroups)
                sb.AppendLine($"{unit.UnitType.ToDescriptionString<UnitType>() } contains {unit.Units.Count}");

            Debug.WriteLine(sb.ToString());
        }

    }
}

