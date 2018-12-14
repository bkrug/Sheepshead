using Sheepshead.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sheepshead.Model
{
    public interface IGameStateDescriber
    {
        IEnumerable<IHand> Hands { get; }
        IHand CurrentHand { get; }
        ITrick CurrentTrick { get; }
        TurnType GetTurnType();
        bool LastHandIsComplete();
    }

    public class GameStateDescriber : IGameStateDescriber
    {
        private IGame _game;

        public GameStateDescriber(IGame game)
        {
            _game = game;
        }

        public IEnumerable<IHand> Hands => _game.Hands.OfType<IHand>();

        public IHand CurrentHand => LastHandIsComplete() ? null : Hands.Last();

        public ITrick CurrentTrick {
            get {
                var trick = Hands.LastOrDefault()?.ITricks?.LastOrDefault();
                if (trick == null || trick.IsComplete() && !Hands.LastOrDefault().IsComplete())
                    trick = new Trick(Hands.LastOrDefault());
                return trick;
            }
        }

        public TurnType GetTurnType()
        {
            var hand = Hands.LastOrDefault();
            if (!Hands.Any() || LastHandIsComplete())
                return TurnType.BeginHand;
            else if (!hand.PickPhaseComplete)
                return TurnType.Pick;
            else if (!hand.Buried.Any() && !hand.Leasters)
                return TurnType.Bury;
            else
                return TurnType.PlayTrick;
        }

        public bool LastHandIsComplete()
        {
            var lastHand = Hands.LastOrDefault();
            return lastHand == null || lastHand.IsComplete();
        }
    }
}
