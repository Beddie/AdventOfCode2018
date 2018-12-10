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
    public class Day9 :  AdventBase, AdventInterface
    {
        public Day9()
        {
            //Test = true;
        }

        private class Game
        {
            public int Index { get; set; }
            public List<int> Marbles { get; set; } = new List<int>() { 0 };
            public int CurrentMarble { get; set; }
            public int MaxMarbleNumber { get; set; } = Test ? 6111 : 70950;
            public Player[] Players { get; set; } = Test ? Enumerable.Range(1, 21).ToList().Select(c => new Player() { ID = c }).ToArray() : Enumerable.Range(1, 431).ToList().Select(c => new Player() { ID = c }).ToArray();
            public bool GameOver => CurrentMarble >= MaxMarbleNumber;
            public bool CountFromBeginningOfCircle => Index > Marbles.Count();
            public bool CounterClockwiseFromEndOfCircle => Index < 0;
            public void Print(Player player) => Debug.WriteLine($"[{player.ID}] {String.Join("   ", Marbles).Replace($" {CurrentMarble}", $"({CurrentMarble})")}");

            public void PlaceMarble(Player player)
            {
                if (++CurrentMarble % 23 == 0)
                {
                    Index -= 7;
                    if (CounterClockwiseFromEndOfCircle) Index += Marbles.Count();
                    player.Score += Marbles[Index] + CurrentMarble;
                    Marbles.RemoveAt(Index);
                }
                else
                {
                    Index += 2;
                    if (CountFromBeginningOfCircle) Index -= Marbles.Count();

                    //TODO refactor
                    Marbles.Insert(Index, CurrentMarble);
                }
                //if (Test) Print(player);
            }
        }

        public class Player
        {
            public int ID { get; set; }
            public long Score { get; set; }
        }

        public string[] Solution()
        {
            return new string[] {
                "404611",
                "3350093681"
            };
        }

        public string Part1()
        {
            var game = new Game();

            while (!game.GameOver)
            {
                foreach (var player in game.Players)
                {
                    game.PlaceMarble(player);
                    if (game.GameOver) break;
                }
            }
            var highScore = game.Players.Select(c => c.Score).Max();
            return highScore.ToString();
        }

        public string Part2()
        {
            var game = new Game();

            //100 times larger
            game.MaxMarbleNumber *= 100;

            while (!game.GameOver)
            {
                foreach (var player in game.Players)
                {
                    game.PlaceMarble(player);
                    if (game.GameOver) break;
                }
            }
            var highScore = game.Players.Select(c => c.Score).Max();
            return highScore.ToString();
        }

        public string GetListName()
        {
            return "Day 9: Marble Mania";
        }

        public int GetID()
        {
            return 9;
        }
    }
}

