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
    public class Day9 : AdventBase
    {
        public Day9()
        {
            ID = 9;
            Name = "Day 9: Marble Mania";
            //Test = true;
        }

        private class Game
        {
            public int Index { get; set; }
            public int MarbleCount { get; set; } = 1;
            public List<int> Marbles { get; set; } = new List<int>() { 0 };
            public int CurrentMarble { get; set; }
            public int MaxMarbleNumber { get; set; } = Test ? 6111 : 70950;
            public Player[] Players { get; set; } = Test ? Enumerable.Range(1, 21).ToList().Select(c => new Player() { ID = c }).ToArray() : Enumerable.Range(1, 431).ToList().Select(c => new Player() { ID = c }).ToArray();
            public bool GameOver => CurrentMarble >= MaxMarbleNumber;
            public bool CountFromBeginningOfCircle => Index > MarbleCount;
            public bool CounterClockwiseFromEndOfCircle => Index < 0;
            public void Print(Player player) => Debug.WriteLine($"[{player.ID}] {String.Join("   ", Marbles).Replace($" {CurrentMarble}", $"({CurrentMarble})")}");

            public void PlaceMarble(Player player)
            {
                if (++CurrentMarble % 23 == 0)
                {
                    Index -= 7;
                    if (CounterClockwiseFromEndOfCircle) Index += MarbleCount;
                    player.Score += (Marbles[Index] + CurrentMarble);
                    Marbles.RemoveAt(Index);
                    MarbleCount--;
                }
                else
                {
                    Index += 2;
                    if (CountFromBeginningOfCircle) Index -= MarbleCount;

                    //TODO refactor
                    Marbles.Insert(Index, CurrentMarble);
                    MarbleCount++;
                }
            }
        }

        public class Player
        {
            public int ID { get; set; }
            public long Score { get; set; }
        }

        public override string[] Solution()
        {
            return new string[] {
                "404611",
                "3350093681"
            };
        }

        public override string Part1()
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

        //TODO refactor performance
        public override string Part2()
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
    }
}

