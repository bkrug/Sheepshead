using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public class Hand : IHand
    {
        public IDeck Deck { get; private set; }
        public IPlayer Picker { get; private set; }
        public IPlayer Partner { set; get; }
        public ICard PartnerCard { get; private set; }

        public Hand(IDeck deck, IPlayer picker, ICard partnerCard)
        {
            Deck = deck;
            Picker = picker;
            PartnerCard = partnerCard;
        }

        public void AddTrick(ITrick trick)
        {
        }

        public Dictionary<IPlayer, int> Scores()
        {
            throw new NotImplementedException();
        }
    }

    public interface IHand
    {
        IDeck Deck { get; }
        IPlayer Picker { get; }
        IPlayer Partner { set; get; }
        ICard PartnerCard { get; }
        void AddTrick(ITrick trick);
        Dictionary<IPlayer, int> Scores();
    }
}