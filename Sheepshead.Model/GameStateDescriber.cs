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
        ITrick CurrentTrick { get; }
        List<IDeck> Decks { get; }
    }

    public class GameStateDescriber : IGameStateDescriber
    {
        public List<IDeck> Decks { get; } = new List<IDeck>();

        public ITrick CurrentTrick {
            get {
                var trick = Decks.LastOrDefault()?.Hand.Tricks.LastOrDefault();
                if (trick == null || trick.IsComplete() && !Decks.LastOrDefault().Hand.IsComplete())
                    trick = new Trick(Decks.LastOrDefault().Hand);
                return trick;
            }
        }
    }
}
