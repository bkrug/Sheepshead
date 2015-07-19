using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sheepshead.Tests.RandomNumberGame
{
    class Turn
    {
        private IPointMaker _pointMaker;
        public int Clue { get; private set; }
        public int? Point { get; private set; }
        public bool? Passed { get; private set; }

        private Turn() { }

        public Turn(int clue, IPointMaker maker)
        {
            Clue = clue;
            _pointMaker = maker;
        }

        public void Pass()
        {
            Passed = true;
            Point = null;
        }

        public void Pick()
        {
            Passed = false;
            Point = _pointMaker.GetPoint(Clue);
        }
    }

    class Game
    {
        private Random _rnd;
        private IPointMaker _pointMaker;

        private Game() {}

        public Game(Random rnd, IPointMaker maker)
        {
            _rnd = rnd;
            _pointMaker = maker;
        }

        public Turn GetTurn()
        {
            return new Turn(_rnd.Next(1, 100), _pointMaker);
        }
    }

    interface IPointMaker
    {
        int GetPoint(int clue);
    }

    class HardToMakePositive : IPointMaker
    {
        private Random _rnd;

        public HardToMakePositive(Random rnd)
        {
            _rnd = rnd;
        }

        public int GetPoint(int clue) {
            var seedNum = -_rnd.NextDouble() - 0.25 + clue / 100.0;
            return seedNum < 0 ? -1 : 1;
        }
    }

    class EasyToMakePositive : IPointMaker
    {
        private Random _rnd;

        public EasyToMakePositive(Random rnd)
        {
            _rnd = rnd;
        }

        public int GetPoint(int clue) {
            var seedNum = -_rnd.NextDouble() + 0.25 + clue / 100.0;
            return seedNum < 0 ? -1 : 1;
        }
    }

    class LinearPositive : IPointMaker
    {
        private Random _rnd;

        public LinearPositive(Random rnd)
        {
            _rnd = rnd;
        }

        public int GetPoint(int clue) {
            var seedNum = -_rnd.NextDouble() + clue / 100.0;
            return seedNum < 0 ? -1 : 1;
        }
    }

    class GamePlayer
    {
        private Game _game;
        private Dictionary<int, double> assumptions = new Dictionary<int, double>() {
            { -100, 0 },
            { -75, 12.5 },
            { -50, 25 },
            { -25, 37.5 }, //Assumes that loses will outnumber wins by 25 over 100 turns if the clue is 37 or 38.
            { 0, 50 },     //Assumes that wins and loses cancel each other out if the clue is 50.
            { 25, 62.5 },  //Assumes that wins will outnumber loses by 25 over 100 turns if the clue is 62 or 63.
            { 50, 75 },
            { 75, 87.5 },
            { 100, 101 }
        };

        private Dictionary<int, int> wins = new Dictionary<int, int>();
        private Dictionary<int, int> tries = new Dictionary<int,int>();

        private GamePlayer() { }

        public GamePlayer(Game game)
        {
            _game = game;
        }

        public void Play()
        {
            using (var writer = new StreamWriter(@"c:\temp\randomNumberGame.txt", false))
            {
                for (var i = 0; i < 10 * 1000; ++i)
                {
                    var turn = _game.GetTurn();
                    var assumption = assumptions.LastOrDefault(a => a.Value < turn.Clue);
                    if (assumptions.Count(a => a.Value == assumption.Value) > 1)
                    { }
                    assumption = assumptions.FirstOrDefault(a => a.Value == assumption.Value);
                    if (assumptions == null)
                        throw new Exception("Clue was higher than 100");
                    if (assumption.Key < 0)
                    {
                        turn.Pass();
                        foreach (var a in assumptions)
                            writer.Write(a.Value.ToString("000.000") + " ");
                        writer.WriteLine("clue:" + turn.Clue.ToString("000") + "  pass");
                    }
                    else
                    {
                        turn.Pick();
                        var points = turn.Point.Value;
                        var normalKey = assumption.Key / 100.0;
                        var winPercent = (1.0 - normalKey) / 2.0 + normalKey;
                        var loosePercent = 1 - winPercent;
                        var changeAmount = (points > 0 ? -loosePercent : winPercent);
                        assumptions[assumption.Key] += changeAmount;
                        if (assumption.Key + 25 < 100)
                            assumptions[assumption.Key + 25] += changeAmount;
                        DropPrevious(assumption.Key);
                        assumption = RecordRealStats(turn, assumption, points);
                        foreach (var a in assumptions)
                            writer.Write(a.Value.ToString("000.000") + " ");
                        writer.WriteLine("clue:" + turn.Clue.ToString("000") + " points:" + points + " Change Amount:" + changeAmount.ToString("0.000"));
                    }
                    writer.Flush();
                }
                foreach (var kvp in tries.OrderBy(t => t.Key))
                    writer.WriteLine(kvp.Key + ": " + wins[kvp.Key] + " out of " + tries[kvp.Key] + " = " + ((double)(wins[kvp.Key]) / tries[kvp.Key]).ToString("0.000") + " ");
                writer.WriteLine();
            }
        }

        private void DropPrevious(int key)
        {
            if (key - 25 > -100 && assumptions[key - 25] > assumptions[key])
            {
                assumptions[key - 25] = assumptions[key];
                DropPrevious(key - 25);
            }
            else
                return;
        }

        private KeyValuePair<int, double> RecordRealStats(Turn turn, KeyValuePair<int, double> assumption, int points)
        {
            if (!wins.ContainsKey(turn.Clue))
                wins[turn.Clue] = 0;
            if (!tries.ContainsKey(turn.Clue))
                tries[turn.Clue] = 0;
            if (points > 0)
                wins[turn.Clue] += 1;
            tries[turn.Clue] += 1;
            return assumption;
        }
    }
}
