using System.Linq;
using Sheepshead.Models;

namespace Sheepshead.Model.TurnProcessors
{
    public interface ITurnTypeCalculator
    {
        TurnType GetTurnType(IGame game);
        bool LastDeckIsComplete(IGame game);
    }

    public class TurnTypeCalculator : ITurnTypeCalculator
    {
        public TurnType GetTurnType(IGame game)
        {
            var deck = game.Decks.LastOrDefault();
            if (!game.Decks.Any() || LastDeckIsComplete(game))
                return TurnType.BeginDeck;
            else if (deck.Hand == null)
                return TurnType.Pick;
            else if (!deck.Buried.Any() && !deck.Hand.Leasters)
                return TurnType.Bury;
            else
                return TurnType.PlayTrick;
        }

        public bool LastDeckIsComplete(IGame game)
        {
            var lastDeck = game.Decks.LastOrDefault();
            return lastDeck == null || lastDeck.Hand != null && lastDeck.Hand.IsComplete();
        }
    }
}
