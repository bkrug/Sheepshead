using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Model
{
    public interface IGameStateDescriber
    {
        List<IHand> Decks { get; }
        IHand CurrentDeck { get; }
        ITrick CurrentTrick { get; }
        TurnType GetTurnType();
        bool LastDeckIsComplete();
    }

    public class GameStateDescriber : IGameStateDescriber
    {
        public List<IHand> Decks { get; } = new List<IHand>();

        public IHand CurrentDeck => LastDeckIsComplete() ? null : Decks.Last();

        public ITrick CurrentTrick {
            get {
                var trick = Decks.LastOrDefault()?.Tricks?.LastOrDefault();
                if (trick == null || trick.IsComplete() && !Decks.LastOrDefault().IsComplete())
                    trick = new Trick(Decks.LastOrDefault());
                return trick;
            }
        }

        public TurnType GetTurnType()
        {
            var deck = Decks.LastOrDefault();
            if (!Decks.Any() || LastDeckIsComplete())
                return TurnType.BeginDeck;
            else if (!deck.PickPhaseComplete)
                return TurnType.Pick;
            else if (!deck.Buried.Any() && !deck.Leasters)
                return TurnType.Bury;
            else
                return TurnType.PlayTrick;
        }

        public bool LastDeckIsComplete()
        {
            var lastDeck = Decks.LastOrDefault();
            return lastDeck == null || lastDeck.IsComplete();
        }
    }
}
