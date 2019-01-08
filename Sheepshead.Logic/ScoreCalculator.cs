using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Logic.Models;
using Sheepshead.Logic.Players;

namespace Sheepshead.Logic
{
    //TODO: Consider making this static.
    public class ScoreCalculator
    {
        IHand _hand;

        public ScoreCalculator(IHand hand)
        {
            _hand = hand;
        }

        //Only public for unit testing.
        public HandScores InternalScores()
        {
            if (!_hand.Leasters)
                return GetNonLeasterScores();
            else
                return GetLeasterScores();
        }

        private HandScores GetNonLeasterScores()
        {
            var handScores = new HandScores
            {
                Points = AssignNonLeasterPoints(out int defensePoints, out bool challengersWonOneTrick, out bool defenseWonOneTrick)
            };
            int defensiveCoins = CalculateDefensiveCoins(defensePoints, challengersWonOneTrick, defenseWonOneTrick);
            handScores.Coins = AssignNonLeasterCoins(challengersWonOneTrick, defensiveCoins);
            return handScores;
        }

        private Dictionary<IPlayer, int> AssignNonLeasterPoints(out int defensePoints, out bool challengersWonOneTrick, out bool defenseWonOneTrick)
        {
            var handPoints = new Dictionary<IPlayer, int>
            {
                { _hand.Picker, _hand.Buried.Sum(c => CardUtil.GetPoints(c)) }
            };
            defensePoints = 0;
            challengersWonOneTrick = false;
            defenseWonOneTrick = false;
            foreach (var trick in _hand.ITricks)
            {
                var winnerData = trick.Winner();
                if (winnerData?.Player == _hand.Picker || winnerData?.Player == _hand.Partner)
                    challengersWonOneTrick = true;
                else
                {
                    defensePoints += winnerData.Points;
                    defenseWonOneTrick = true;
                }
                if (winnerData?.Player != null)
                {
                    if (handPoints.ContainsKey(winnerData.Player))
                        handPoints[winnerData.Player] += winnerData.Points;
                    else
                        handPoints.Add(winnerData.Player, winnerData.Points);
                }
            }
            return handPoints;
        }

        private static int CalculateDefensiveCoins(int defensePoints, bool challengersWonOneTrick, bool defenseWonOneTrick)
        {
            int defensiveCoins;
            if (!challengersWonOneTrick)
                defensiveCoins = 3;
            else if (defensePoints >= 90)
                defensiveCoins = 2;
            else if (defensePoints >= 60)
                defensiveCoins = 1;
            else if (defensePoints >= 30)
                defensiveCoins = -1;
            else if (defenseWonOneTrick)
                defensiveCoins = -2;
            else
                defensiveCoins = -3;
            return defensiveCoins;
        }

        private Dictionary<IPlayer, int> AssignNonLeasterCoins(bool challengersWonOneTrick, int defensiveCoins)
        {
            var handCoins = new Dictionary<IPlayer, int>();
            if (_hand.Partner == _hand.Picker)
            {
                var partnerCard = _hand.PartnerCardEnum.HasValue ? Enum.GetName(typeof(SheepCard), _hand.PartnerCardEnum.Value) : "no parter card.";
                throw new Exception("Picker and Partner are the same person! " + partnerCard);
            }
            _hand.Players
                .Except(new List<IPlayer>() { _hand.Partner, _hand.Picker })
                .ToList()
                .ForEach(p => handCoins.Add(p, defensiveCoins));
            var totalDefensiveCoins = handCoins.Sum(c => c.Value);
            if (_hand.Partner == null)
                handCoins.Add(_hand.Picker, -totalDefensiveCoins);
            else if (!challengersWonOneTrick)
            {
                handCoins.Add(_hand.Picker, -totalDefensiveCoins);
                handCoins.Add(_hand.Partner, 0);
            }
            else
            {
                handCoins.Add(_hand.Picker, -totalDefensiveCoins * 2 / 3);
                handCoins.Add(_hand.Partner, -totalDefensiveCoins / 3);
            }
            return handCoins;
        }

        private HandScores GetLeasterScores()
        {
            var trickPoints = _hand.ITricks.Select(t => t.Winner())
                                    .GroupBy(t => t.Player)
                                    .ToDictionary(g => g.Key, g => g.Sum(wd => wd.Points));

            var leasterWinner = trickPoints.OrderBy(c => c.Value).First().Key;
            var trickCoins = _hand.Players.ToDictionary(p => p, p => p == leasterWinner ? _hand.PlayerCount - 1 : -1);

            return new HandScores()
            {
                Coins = trickCoins,
                Points = trickPoints
            };
        }
    }
}