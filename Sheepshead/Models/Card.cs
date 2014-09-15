using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models
{
    public enum StandardSuite
    {
        CLUBS, SPADES, HEARTS, DIAMONDS
    }

    public enum CardType
    {
        QUEEN, JACK, ACE, N10, KING, N9, N8, N7
    }

    public enum Suite 
    {
        CLUBS, SPADES, HEARTS, TRUMP
    }

    public class CardRepository
    {
        private static CardRepository _instance = new CardRepository();
        private TempCard[,] _cs = new TempCard[4, 8];
        private Card[,] _cards = new Card[4, 8];

        public static CardRepository Instance { get { return _instance; } }

        private CardRepository()
        {
            SetPoints();
            SetRanks();
            Copy();
        }

        private void SetPoints()
        {
            for (var ss = (int)StandardSuite.CLUBS; ss <= (int)StandardSuite.DIAMONDS; ++ss)
            {
                _cs[ss, (int)CardType.ACE].Points = 11;
                _cs[ss, (int)CardType.N10].Points = 10;
                _cs[ss, (int)CardType.KING].Points = 4;
                _cs[ss, (int)CardType.QUEEN].Points = 3;
                _cs[ss, (int)CardType.JACK].Points = 2;
                _cs[ss, (int)CardType.N9].Points = 0;
                _cs[ss, (int)CardType.N8].Points = 0;
                _cs[ss, (int)CardType.N7].Points = 0;
            }
        }

        private void SetRanks()
        {
            int ct, ss;
            var rank = 1;

            for (ct = (int)CardType.QUEEN; ct <= (int)CardType.JACK; ++ct)
                for (ss = (int)StandardSuite.CLUBS; ss <= (int)StandardSuite.DIAMONDS; ++ss)
                    _cs[ss, ct].Rank = rank++;

            ss = (int)StandardSuite.DIAMONDS;
            for (ct = (int)CardType.ACE; ct <= (int)CardType.N7; ++ct)
                _cs[ss, ct].Rank = rank++;

            for (ss = (int)StandardSuite.CLUBS; ss <= (int)StandardSuite.HEARTS; ++ss)
                for (ct = (int)CardType.ACE; ct <= (int)CardType.N7; ++ct)
                    _cs[ss, ct].Rank = rank++;
        }

        private void Copy()
        {
            for (var ss = (int)StandardSuite.CLUBS; ss <= (int)StandardSuite.DIAMONDS; ++ss)
                for (var ct = (int)CardType.QUEEN; ct <= (int)CardType.N7; ++ct) {
                    var cur = _cs[ss, ct];
                    _cards[ss, ct] = new Card((StandardSuite)ss, (CardType)ct, cur.Points, cur.Rank);
                }
        }

        public Card this[StandardSuite ss, CardType ct]
        {
            get { return _cards[(int)ss, (int)ct]; }
        }

        public static Suite GetSuite(Card card)
        {
            if (card.CardType == CardType.QUEEN || card.CardType == CardType.JACK)
                return Suite.TRUMP;
            return (Suite)card.StandardSuite;
        }

        private struct TempCard
        {
            public Int32 Points;
            public Int32 Rank;
        }
    }

    public struct Card
    {
        private StandardSuite _StandardSuite;
        private CardType _CardType;
        private Int32 _Points;
        private Int32 _Rank;

        public Card(StandardSuite ss, CardType ct, int points, int rank)
        {
            _StandardSuite = ss;
            _CardType = ct;
            _Points = points;
            _Rank = rank;
        }
        public StandardSuite StandardSuite { get { return _StandardSuite; }}
        public CardType CardType { get { return _CardType; } }
        public Int32 Points { get { return _Points; } }
        public Int32 Rank { get { return _Rank; } }
        public override string ToString()
        {
            return _StandardSuite.ToString() + " " + _CardType;
        }
    }
}
