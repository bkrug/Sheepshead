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

    public enum Suit 
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

            rank = 9; //Should already be 9 by this point. Setting it to 9 to make the next loop more clear.
            ss = (int)StandardSuite.DIAMONDS;
            for (ct = (int)CardType.ACE; ct <= (int)CardType.N7; ++ct)
                _cs[ss, ct].Rank = rank++;

            for (ss = (int)StandardSuite.CLUBS; ss <= (int)StandardSuite.HEARTS; ++ss)
            {
                _cs[ss, (int)CardType.ACE].Rank = 15;
                _cs[ss, (int)CardType.N10].Rank = 16;
                _cs[ss, (int)CardType.KING].Rank = 17;
                _cs[ss, (int)CardType.N9].Rank = 18;
                _cs[ss, (int)CardType.N8].Rank = 19;
                _cs[ss, (int)CardType.N7].Rank = 20;
            }
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

        public static Suit GetSuit(ICard card)
        {
            if (card.CardType == CardType.QUEEN || card.CardType == CardType.JACK)
                return Suit.TRUMP;
            return (Suit)card.StandardSuite;
        }

        public List<ICard> UnshuffledList()
        {
            var list = new List<ICard>();
            foreach(var ss in Enum.GetValues(typeof(StandardSuite)))
                foreach (var ct in Enum.GetValues(typeof(CardType)))
                    list.Add(Instance[(StandardSuite)ss, (CardType)ct]);
            return list;
        }

        private static Dictionary<ICard, string> list = new Dictionary<ICard, string>() {
            { Instance[StandardSuite.CLUBS, CardType.ACE], "1" },
            { Instance[StandardSuite.SPADES, CardType.ACE], "2" },
            { Instance[StandardSuite.HEARTS, CardType.ACE], "3" },
            { Instance[StandardSuite.DIAMONDS, CardType.ACE], "4" },
            { Instance[StandardSuite.CLUBS, CardType.KING], "5" },
            { Instance[StandardSuite.SPADES, CardType.KING], "6" },
            { Instance[StandardSuite.HEARTS, CardType.KING], "7" },
            { Instance[StandardSuite.DIAMONDS, CardType.KING], "8" },
            { Instance[StandardSuite.CLUBS, CardType.QUEEN], "9" },
            { Instance[StandardSuite.SPADES, CardType.QUEEN], "10" },
            { Instance[StandardSuite.HEARTS, CardType.QUEEN], "11" },
            { Instance[StandardSuite.DIAMONDS, CardType.QUEEN], "12" },
            { Instance[StandardSuite.CLUBS, CardType.JACK], "13" },
            { Instance[StandardSuite.SPADES, CardType.JACK], "14" },
            { Instance[StandardSuite.HEARTS, CardType.JACK], "15" },
            { Instance[StandardSuite.DIAMONDS, CardType.JACK], "16" },
            { Instance[StandardSuite.CLUBS, CardType.N10], "17" },
            { Instance[StandardSuite.SPADES, CardType.N10], "18" },
            { Instance[StandardSuite.HEARTS, CardType.N10], "19" },
            { Instance[StandardSuite.DIAMONDS, CardType.N10], "20" },
            { Instance[StandardSuite.CLUBS, CardType.N9], "21" },
            { Instance[StandardSuite.SPADES, CardType.N9], "22" },
            { Instance[StandardSuite.HEARTS, CardType.N9], "23" },
            { Instance[StandardSuite.DIAMONDS, CardType.N9], "24" },
            { Instance[StandardSuite.CLUBS, CardType.N8], "25" },
            { Instance[StandardSuite.SPADES, CardType.N8], "26" },
            { Instance[StandardSuite.HEARTS, CardType.N8], "27" },
            { Instance[StandardSuite.DIAMONDS, CardType.N8], "28" },
            { Instance[StandardSuite.CLUBS, CardType.N7], "29" },
            { Instance[StandardSuite.SPADES, CardType.N7], "30" },
            { Instance[StandardSuite.HEARTS, CardType.N7], "31" },
            { Instance[StandardSuite.DIAMONDS, CardType.N7], "32" }
        };

        private static Dictionary<StandardSuite, string> _suiteLetter = new Dictionary<StandardSuite, string>()
        {
            { StandardSuite.CLUBS, "C" }, { StandardSuite.DIAMONDS, "D" }, { StandardSuite.HEARTS, "H" }, { StandardSuite.SPADES, "S" }
        };
        public static Dictionary<StandardSuite, string> SuiteLetter { get { return _suiteLetter; } }
        
        private static Dictionary<CardType, string> _cardTypeLetter = new Dictionary<CardType, string>()
        {
            { CardType.ACE, "A" }, { CardType.JACK, "J" }, { CardType.KING, "K" }, { CardType.QUEEN, "Q" }, 
            { CardType.N10, "T" }, { CardType.N9, "9" }, { CardType.N8, "8" }, { CardType.N7, "7" }
        };
        public static Dictionary<CardType, string> CardTypeLetter { get { return _cardTypeLetter; } }

        private static Dictionary<string, StandardSuite> _reverseSuiteLetter = new Dictionary<string, StandardSuite>()
        {
            { "C", StandardSuite.CLUBS }, { "D", StandardSuite.DIAMONDS }, { "H", StandardSuite.HEARTS }, { "S", StandardSuite.SPADES }
        };
        public static Dictionary<string, StandardSuite> ReverseSuiteLetter { get { return _reverseSuiteLetter; } }

        private static Dictionary<string, CardType> _reverseCardTypeLetter = new Dictionary<string, CardType>()
        {
            { "A", CardType.ACE}, { "J", CardType.JACK }, { "K", CardType.KING }, { "Q", CardType.QUEEN }, 
            { "T", CardType.N10 }, { "9", CardType.N9 }, { "8", CardType.N8 }, { "7", CardType.N7 }
        };
        public static Dictionary<string, CardType> ReverseCardTypeLetter { get { return _reverseCardTypeLetter; } }

        public static string GetPictureFilename(ICard card)
        {
            return list[card];
        }

        private struct TempCard
        {
            public Int32 Points;
            public Int32 Rank;
        }
    }
    
    public struct Card : ICard
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
        public int Id { get { return (int)StandardSuite * 4 + (int)CardType + 1; } }
        public StandardSuite StandardSuite { get { return _StandardSuite; }}
        public CardType CardType { get { return _CardType; } }
        public Int32 Points { get { return _Points; } }
        public Int32 Rank { get { return _Rank; } }
        public override string ToString()
        {
            return _CardType + " " + _StandardSuite.ToString();
        }
        public string ToAbbr()
        {
            return CardRepository.CardTypeLetter[_CardType] + CardRepository.SuiteLetter[_StandardSuite];
        }
    }

    public interface ICard
    {
        int Id { get; }
        StandardSuite StandardSuite { get; }
        CardType CardType { get; }
        Int32 Points { get; }
        Int32 Rank { get; }
        string ToString();
        string ToAbbr();
    }
}
