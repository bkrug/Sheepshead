using System;
using System.Collections.Generic;
using Sheepshead.Logic;
using Sheepshead.Logic.Models;
using Sheepshead.Logic.Players;

namespace Sheepshead.Tests.PlayerMocks
{
    public class MockPlayer : IPlayer
    {
        public string Name { get; set; }

        private List<SheepCard> _cards = new List<SheepCard>();
        IReadOnlyList<SheepCard> IPlayer.Cards => _cards;
        public Participant Participant { get; } = new Participant();

        public MockPlayer()
        {
        }

        public int QueueRankInTrick(ITrick trick)
        {
            throw new NotImplementedException();
        }

        public int QueueRankInHand(IHand hand)
        {
            throw new NotImplementedException();
        }

        public List<SheepCard> LegalCalledAces(IHand hand)
        {
            throw new NotImplementedException();
        }

        public void AddCard(SheepCard card)
        {
            _cards.Add(card);
        }

        public void RemoveCard(SheepCard card)
        {
            _cards.Remove(card);
        }

        public void RemoveAllCards()
        {
            _cards.Clear();
        }

        public void AddCardRange(List<SheepCard> cards)
        {
            _cards.AddRange(cards);
        }
    }
}
