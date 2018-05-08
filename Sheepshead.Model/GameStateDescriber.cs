using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sheepshead.Models;

namespace Sheepshead.Model
{
    public interface IGameStateDescriber
    {
        List<IDeck> Decks { get; }
        IDeck CurrentDeck { get; }
        ITrick CurrentTrick { get; }
        TurnType GetTurnType();
        bool LastDeckIsComplete();
    }

    public class GameStateDescriber : IGameStateDescriber
    {
        public List<IDeck> Decks { get; } = new List<IDeck>();

        public IDeck CurrentDeck => LastDeckIsComplete() ? null : Decks.Last();

        public ITrick CurrentTrick {
            get {
                var trick = Decks.LastOrDefault()?.Hand.Tricks.LastOrDefault();
                if (trick == null || trick.IsComplete() && !Decks.LastOrDefault().Hand.IsComplete())
                    trick = new Trick(Decks.LastOrDefault().Hand);
                return trick;
            }
        }

        public TurnType GetTurnType()
        {
            var deck = Decks.LastOrDefault();
            if (!Decks.Any() || LastDeckIsComplete())
                return TurnType.BeginDeck;
            else if (deck.Hand == null)
                return TurnType.Pick;
            else if (!deck.Buried.Any() && !deck.Hand.Leasters)
                return TurnType.Bury;
            else
                return TurnType.PlayTrick;
        }

        public bool LastDeckIsComplete()
        {
            var lastDeck = Decks.LastOrDefault();
            return lastDeck == null || lastDeck.Hand != null && lastDeck.Hand.IsComplete();
        }
    }
}
