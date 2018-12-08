using System;
using System.Collections.Generic;
using System.Linq;

using Sheepshead.Model.Players;

namespace Sheepshead.Model
{
    //TODO: Consider making this static.
    public class ScoreCalculator
    {
        IHand _hand;
        IDeck Deck => _hand.Deck;
        bool Leasters => _hand.Leasters;
        IPlayer Picker => _hand.Picker;
        IPlayer Partner => _hand.Partner;
        List<ITrick> _tricks => _hand.Tricks;
        List<ITrick> Tricks => _hand.Tricks;
        SheepCard? PartnerCard => _hand.PartnerCard;

        public ScoreCalculator(IHand hand)
        {
            _hand = hand;
        }

        //Only public for unit testing.
        public HandScores InternalScores()
        {
            if (!Leasters)
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
                { Picker, Deck.Buried.Sum(c => CardUtil.GetPoints(c)) }
            };
            defensePoints = 0;
            challengersWonOneTrick = false;
            defenseWonOneTrick = false;
            foreach (var trick in _tricks)
            {
                var winnerData = trick.Winner();
                if (winnerData?.Player == Picker || winnerData?.Player == Partner)
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
            if (Partner == Picker)
            {
                var partnerCard = PartnerCard.HasValue ? Enum.GetName(typeof(SheepCard), PartnerCard.Value) : "no parter card.";
                throw new Exception("Picker and Partner are the same person! " + partnerCard);
            }
            Deck.Players
                .Except(new List<IPlayer>() { Partner, Picker })
                .ToList()
                .ForEach(p => handCoins.Add(p, defensiveCoins));
            var totalDefensiveCoins = handCoins.Sum(c => c.Value);
            if (Partner == null)
                handCoins.Add(Picker, -totalDefensiveCoins);
            else if (!challengersWonOneTrick)
            {
                handCoins.Add(Picker, -totalDefensiveCoins);
                handCoins.Add(Partner, 0);
            }
            else
            {
                handCoins.Add(Picker, -totalDefensiveCoins * 2 / 3);
                handCoins.Add(Partner, -totalDefensiveCoins / 3);
            }
            return handCoins;
        }

        private HandScores GetLeasterScores()
        {
            var trickPoints = Tricks.Select(t => t.Winner())
                                    .GroupBy(t => t.Player)
                                    .ToDictionary(g => g.Key, g => g.Sum(wd => wd.Points));

            var leasterWinner = trickPoints.OrderBy(c => c.Value).First().Key;
            var trickCoins = Deck.Players.ToDictionary(p => p, p => p == leasterWinner ? Deck.PlayerCount - 1 : -1);

            return new HandScores()
            {
                Coins = trickCoins,
                Points = trickPoints
            };
        }
    }
}