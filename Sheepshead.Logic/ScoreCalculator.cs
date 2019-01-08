using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Logic.Models;
using Sheepshead.Logic.Players;

namespace Sheepshead.Logic
{
    public class HandScores
    {
        public Dictionary<IPlayer, int> Coins { get; set; }
        public Dictionary<IPlayer, int> Points { get; set; }
    }

    public static class ScoreCalculator
    {
        public static HandScores GetScores(IHand hand)
        {
            if (!hand.Leasters)
                return GetNonLeasterScores(hand);
            else
                return GetLeasterScores(hand);
        }

        private static HandScores GetNonLeasterScores(IHand hand)
        {
            var handScores = new HandScores
            {
                Points = AssignNonLeasterPoints(hand, out int defensePoints, out bool challengersWonOneTrick, out bool defenseWonOneTrick)
            };
            int defensiveCoins = CalculateDefensiveCoins(defensePoints, challengersWonOneTrick, defenseWonOneTrick);
            handScores.Coins = AssignNonLeasterCoins(hand, challengersWonOneTrick, defensiveCoins);
            return handScores;
        }

        private static Dictionary<IPlayer, int> AssignNonLeasterPoints(IHand hand, out int defensePoints, out bool challengersWonOneTrick, out bool defenseWonOneTrick)
        {
            var handPoints = new Dictionary<IPlayer, int>
            {
                { hand.Picker, hand.Buried.Sum(c => CardUtil.GetPoints(c)) }
            };
            defensePoints = 0;
            challengersWonOneTrick = false;
            defenseWonOneTrick = false;
            foreach (var trick in hand.ITricks)
            {
                var winnerData = trick.Winner();
                if (winnerData?.Player == hand.Picker || winnerData?.Player == hand.Partner)
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

        private static Dictionary<IPlayer, int> AssignNonLeasterCoins(IHand hand, bool challengersWonOneTrick, int defensiveCoins)
        {
            var handCoins = new Dictionary<IPlayer, int>();
            if (hand.Partner == hand.Picker)
            {
                var partnerCard = hand.PartnerCardEnum.HasValue ? Enum.GetName(typeof(SheepCard), hand.PartnerCardEnum.Value) : "no parter card.";
                throw new Exception("Picker and Partner are the same person! " + partnerCard);
            }
            hand.Players
                .Except(new List<IPlayer>() { hand.Partner, hand.Picker })
                .ToList()
                .ForEach(p => handCoins.Add(p, defensiveCoins));
            var totalDefensiveCoins = handCoins.Sum(c => c.Value);
            if (hand.Partner == null)
                handCoins.Add(hand.Picker, -totalDefensiveCoins);
            else if (!challengersWonOneTrick)
            {
                handCoins.Add(hand.Picker, -totalDefensiveCoins);
                handCoins.Add(hand.Partner, 0);
            }
            else
            {
                handCoins.Add(hand.Picker, -totalDefensiveCoins * 2 / 3);
                handCoins.Add(hand.Partner, -totalDefensiveCoins / 3);
            }
            return handCoins;
        }

        private static HandScores GetLeasterScores(IHand hand)
        {
            var trickPoints = hand.ITricks.Select(t => t.Winner())
                                    .GroupBy(t => t.Player)
                                    .ToDictionary(g => g.Key, g => g.Sum(wd => wd.Points));

            var leasterWinner = trickPoints.OrderBy(c => c.Value).First().Key;
            var trickCoins = hand.Players.ToDictionary(p => p, p => p == leasterWinner ? hand.PlayerCount - 1 : -1);

            return new HandScores()
            {
                Coins = trickCoins,
                Points = trickPoints
            };
        }
    }
}