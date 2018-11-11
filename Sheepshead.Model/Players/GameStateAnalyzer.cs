using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Models.Players
{
    public interface IGameStateAnalyzer
    {
        bool? AllOpponentsHavePlayed(IPlayer thisPlayer, ITrick trick);
        bool MySideWinning(IPlayer thisPlayer, ITrick trick);
        List<SheepCard> MyCardsThatCanWin(IPlayer thisPlayer, ITrick trick);
        bool UnplayedCardsBeatPlayedCards(IPlayer thisPlayer, ITrick trick);
        bool UnplayedCardsBeatMyCards(List<SheepCard> myStrongCards, ITrick trick);
    }

    public class GameStateAnalyzer : IGameStateAnalyzer
    {
        public bool? AllOpponentsHavePlayed(IPlayer thisPlayer, ITrick trick)
        {
            if (trick.CardsPlayed.Count == trick.Hand.Deck.Game.PlayerCount - 1)
                return true;
            var playerIsPartner = trick.Hand.Partner == thisPlayer 
                || trick.Hand.PartnerCard.HasValue && thisPlayer.Cards.Contains(trick.Hand.PartnerCard.Value);
            var playerIsOffense = trick.Hand.Picker == thisPlayer 
                || playerIsPartner;
            if (playerIsOffense) {
                var opponentCount = trick.Hand.PartnerCard.HasValue
                    ? trick.Hand.Deck.Game.PlayerCount - 2
                    : trick.Hand.Deck.Game.PlayerCount - 1;
                var opponentsWithTurns = trick.CardsPlayed.Keys.Count(p => trick.Hand.Picker != p && trick.Hand.PresumedParnter != p);
                if (opponentsWithTurns < opponentCount)
                    return false;
                if (!playerIsPartner && trick.Hand.PartnerCard.HasValue && trick.Hand.PresumedParnter == null)
                    return null;
                return true;
            }
            else
            {
                if (!trick.CardsPlayed.ContainsKey(trick.Hand.Picker))
                    return false;
                if (trick.Hand.PresumedParnter != null && !trick.CardsPlayed.ContainsKey(trick.Hand.PresumedParnter))
                    return false;
                if (trick.Hand.PartnerCard.HasValue && trick.Hand.PresumedParnter == null)
                    return null;
                return true;
            }
        }

        public List<SheepCard> MyCardsThatCanWin(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool MySideWinning(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool UnplayedCardsBeatPlayedCards(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public bool UnplayedCardsBeatMyCards(List<SheepCard> myStrongCards, ITrick trick)
        {
            throw new System.NotImplementedException();
        }
    }
}