using System;
using System.Collections.Generic;
using System.Linq;



namespace Sheepshead.Models.Players
{
    public class Player : IPlayer
    {
        private List<SheepCard> _hand = new List<SheepCard>();

        public virtual string Name { get { return String.Empty; } }
        public List<SheepCard> Cards { get { return _hand; } }

        public int QueueRankInTrick(ITrick trick)
        {
            var indexOfMe = trick.Players.IndexOf(this);
            var indexOfStartingPlayer = trick.Players.IndexOf(trick.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += trick.PlayerCount;
            return rank + 1;
        }

        public int QueueRankInDeck(IDeck deck)
        {
            var indexOfMe = deck.Players.IndexOf(this);
            var indexOfStartingPlayer = deck.Players.IndexOf(deck.StartingPlayer);
            var rank = indexOfMe - indexOfStartingPlayer;
            if (rank < 0) rank += deck.PlayerCount;
            return rank + 1;
        }
    }

    public interface IPlayer
    {
        string Name { get; }
        List<SheepCard> Cards { get; }
        int QueueRankInTrick(ITrick trick);
        int QueueRankInDeck(IDeck deck);
    }
}