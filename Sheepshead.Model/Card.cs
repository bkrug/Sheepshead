﻿using System;
using System.Collections.Generic;
using System.Linq;

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

    public enum SheepCard
    {
        QUEEN_CLUBS, QUEEN_SPADES, QUEEN_HEARTS, QUEEN_DIAMONDS, JACK_CLUBS, JACK_SPADES, JACK_HEARTS, JACK_DIAMONDS,
        ACE_CLUBS, ACE_SPADES, ACE_HEARTS, ACE_DIAMONDS, N10_CLUBS, N10_SPADES, N10_HEARTS, N10_DIAMONDS,
        KING_CLUBS, KING_SPADES, KING_HEARTS, KING_DIAMONDS, N9_CLUBS, N9_SPADES, N9_HEARTS, N9_DIAMONDS,
        N8_CLUBS, N8_SPADES, N8_HEARTS, N8_DIAMONDS, N7_CLUBS, N7_SPADES, N7_HEARTS, N7_DIAMONDS
    }

    public class CardRepository
    {
        private static CardRepository _instance = new CardRepository();
        private TempCard[,] _cs = new TempCard[4, 8];
        private Card[,] _cards = new Card[4, 8];

        public static CardRepository Instance { get { return _instance; } }

        public static CardType GetFace(SheepCard card)
        {
            Int32 cardVal = Convert.ToInt32(card) / 4;
            return (CardType)cardVal;
        }

        public static StandardSuite GetStandardSuit(SheepCard card)
        {
            Int32 cardVal = Convert.ToInt32(card) % 4;
            return (StandardSuite)cardVal;
        }

        public static Suit GetSuit(SheepCard card)
        {
            if (card <= SheepCard.JACK_DIAMONDS)
                return Suit.TRUMP;
            var standardSuit = GetStandardSuit(card);
            switch(standardSuit)
            {
                case StandardSuite.DIAMONDS:
                    return Suit.TRUMP;
                case StandardSuite.CLUBS:
                    return Suit.CLUBS;
                case StandardSuite.HEARTS:
                    return Suit.HEARTS;
                default:
                    return Suit.SPADES;
            }
        }

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

        public static int GetPoints(SheepCard card)
        {
            var cardType = GetFace(card);
            switch (cardType)
            {
                case CardType.ACE:
                    return 11;
                case CardType.N10:
                    return 10;
                case CardType.KING:
                    return 4;
                case CardType.QUEEN:
                    return 3;
                case CardType.JACK:
                    return 2;
                default:
                    return 0;
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

        public static int GetRank(SheepCard card)
        {
            if (card <= SheepCard.JACK_DIAMONDS)
                return Convert.ToInt32(card) + 1;

            var standardSuit = GetStandardSuit(card);
            var face = GetFace(card);
            if (standardSuit == StandardSuite.DIAMONDS)
                switch (face)
                {
                    case CardType.ACE:
                        return 9;
                    case CardType.N10:
                        return 10;
                    case CardType.KING:
                        return 11;
                    case CardType.N9:
                        return 12;
                    case CardType.N8:
                        return 13;
                    case CardType.N7:
                        return 14;
                    default:
                        return -1;
                }
            else
                switch (face)
                {
                    case CardType.ACE:
                        return 15;
                    case CardType.N10:
                        return 16;
                    case CardType.KING:
                        return 17;
                    case CardType.N9:
                        return 18;
                    case CardType.N8:
                        return 19;
                    case CardType.N7:
                        return 20;
                    default:
                        return -1;
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

        public List<SheepCard> UnshuffledList()
        {
            var list = new List<SheepCard>();
            foreach (var ss in Enum.GetValues(typeof(SheepCard)))
                list.Add((SheepCard)ss);
            return list;
        }

        private static Dictionary<SheepCard, string> list1 = new Dictionary<SheepCard, string>() {
            { SheepCard.ACE_CLUBS, "1" },
            { SheepCard.ACE_SPADES, "2" },
            { SheepCard.ACE_HEARTS, "3" },
            { SheepCard.ACE_DIAMONDS, "4" },
            { SheepCard.KING_CLUBS, "5" },
            { SheepCard.KING_SPADES, "6" },
            { SheepCard.KING_HEARTS, "7" },
            { SheepCard.KING_DIAMONDS, "8" },
            { SheepCard.QUEEN_CLUBS, "9" },
            { SheepCard.QUEEN_SPADES, "10" },
            { SheepCard.QUEEN_HEARTS, "11" },
            { SheepCard.QUEEN_DIAMONDS, "12" },
            { SheepCard.JACK_CLUBS, "13" },
            { SheepCard.JACK_SPADES, "14" },
            { SheepCard.JACK_HEARTS, "15" },
            { SheepCard.JACK_DIAMONDS, "16" },
            { SheepCard.N10_CLUBS, "17" },
            { SheepCard.N10_SPADES, "18" },
            { SheepCard.N10_HEARTS, "19" },
            { SheepCard.N10_DIAMONDS, "20" },
            { SheepCard.N9_CLUBS, "21" },
            { SheepCard.N9_SPADES, "22" },
            { SheepCard.N9_HEARTS, "23" },
            { SheepCard.N9_DIAMONDS, "24" },
            { SheepCard.N8_CLUBS, "25" },
            { SheepCard.N8_SPADES, "26" },
            { SheepCard.N8_HEARTS, "27" },
            { SheepCard.N8_DIAMONDS, "28" },
            { SheepCard.N7_CLUBS, "29" },
            { SheepCard.N7_SPADES, "30" },
            { SheepCard.N7_HEARTS, "31" },
            { SheepCard.N7_DIAMONDS, "32" }
        };
        private static Dictionary<ICard, SheepCard> list2 = new Dictionary<ICard, SheepCard>() {
            { Instance[StandardSuite.CLUBS, CardType.ACE], SheepCard.ACE_CLUBS },
            { Instance[StandardSuite.SPADES, CardType.ACE], SheepCard.ACE_SPADES },
            { Instance[StandardSuite.HEARTS, CardType.ACE], SheepCard.ACE_HEARTS },
            { Instance[StandardSuite.DIAMONDS, CardType.ACE],SheepCard.ACE_DIAMONDS },
            { Instance[StandardSuite.CLUBS, CardType.KING], SheepCard.KING_CLUBS },
            { Instance[StandardSuite.SPADES, CardType.KING], SheepCard.KING_SPADES },
            { Instance[StandardSuite.HEARTS, CardType.KING], SheepCard.KING_HEARTS },
            { Instance[StandardSuite.DIAMONDS, CardType.KING], SheepCard.KING_DIAMONDS },
            { Instance[StandardSuite.CLUBS, CardType.QUEEN], SheepCard.QUEEN_CLUBS },
            { Instance[StandardSuite.SPADES, CardType.QUEEN], SheepCard.QUEEN_SPADES },
            { Instance[StandardSuite.HEARTS, CardType.QUEEN], SheepCard.QUEEN_HEARTS },
            { Instance[StandardSuite.DIAMONDS, CardType.QUEEN], SheepCard.QUEEN_DIAMONDS },
            { Instance[StandardSuite.CLUBS, CardType.JACK], SheepCard.JACK_CLUBS },
            { Instance[StandardSuite.SPADES, CardType.JACK], SheepCard.JACK_SPADES },
            { Instance[StandardSuite.HEARTS, CardType.JACK], SheepCard.JACK_HEARTS },
            { Instance[StandardSuite.DIAMONDS, CardType.JACK], SheepCard.JACK_DIAMONDS },
            { Instance[StandardSuite.CLUBS, CardType.N10], SheepCard.N10_CLUBS },
            { Instance[StandardSuite.SPADES, CardType.N10], SheepCard.N10_SPADES },
            { Instance[StandardSuite.HEARTS, CardType.N10], SheepCard.N10_HEARTS },
            { Instance[StandardSuite.DIAMONDS, CardType.N10], SheepCard.N10_DIAMONDS },
            { Instance[StandardSuite.CLUBS, CardType.N9], SheepCard.N9_CLUBS },
            { Instance[StandardSuite.SPADES, CardType.N9], SheepCard.N9_SPADES },
            { Instance[StandardSuite.HEARTS, CardType.N9], SheepCard.N9_HEARTS },
            { Instance[StandardSuite.DIAMONDS, CardType.N9], SheepCard.N9_DIAMONDS },
            { Instance[StandardSuite.CLUBS, CardType.N8], SheepCard.N8_CLUBS },
            { Instance[StandardSuite.SPADES, CardType.N8], SheepCard.N8_SPADES },
            { Instance[StandardSuite.HEARTS, CardType.N8], SheepCard.N8_HEARTS },
            { Instance[StandardSuite.DIAMONDS, CardType.N8], SheepCard.N8_DIAMONDS },
            { Instance[StandardSuite.CLUBS, CardType.N7], SheepCard.N7_CLUBS },
            { Instance[StandardSuite.SPADES, CardType.N7], SheepCard.N7_SPADES },
            { Instance[StandardSuite.HEARTS, CardType.N7], SheepCard.N7_HEARTS },
            { Instance[StandardSuite.DIAMONDS, CardType.N7], SheepCard.N7_DIAMONDS }
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
            return GetPictureFilename(GetSheepCard(card));
        }
        public static string GetPictureFilename(SheepCard card)
        {
            return list1[card];
        }

        public static SheepCard GetSheepCard(ICard card)
        {
            return list2[card];
        }

        public static ICard GetICard(SheepCard card)
        {
            return list2.Single(kvp => kvp.Value == card).Key;
        }

        private struct TempCard
        {
            public Int32 Points;
            public Int32 Rank;
        }

        public static string ToAbbr(SheepCard card)
        {
            var cardType = GetFace(card);
            var suit = GetStandardSuit(card);
            return CardTypeLetter[cardType] + SuiteLetter[suit];
        }
    }
    
    public struct Card : ICard
    {
        private StandardSuite _StandardSuite;
        private CardType _CardType;

        public Card(StandardSuite ss, CardType ct, int points, int rank)
        {
            _StandardSuite = ss;
            _CardType = ct;
        }
        public StandardSuite StandardSuite { get { return _StandardSuite; }}
        public CardType CardType { get { return _CardType; } }
        public override string ToString()
        {
            return _CardType + " " + _StandardSuite.ToString();
        }
    }

    public interface ICard
    {
        StandardSuite StandardSuite { get; }
        CardType CardType { get; }
        string ToString();
    }
}
