using System;
using System.Linq;
using System.Collections.Generic;

namespace Sheepshead.Model
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
        QUEEN_CLUBS, QUEEN_SPADES, QUEEN_HEARTS, QUEEN_DIAMONDS,
        JACK_CLUBS,  JACK_SPADES,  JACK_HEARTS,  JACK_DIAMONDS,
        ACE_CLUBS,   ACE_SPADES,   ACE_HEARTS,   ACE_DIAMONDS,
        N10_CLUBS,   N10_SPADES,   N10_HEARTS,   N10_DIAMONDS,
        KING_CLUBS,  KING_SPADES,  KING_HEARTS,  KING_DIAMONDS,
        N9_CLUBS, N9_SPADES, N9_HEARTS, N9_DIAMONDS,
        N8_CLUBS, N8_SPADES, N8_HEARTS, N8_DIAMONDS,
        N7_CLUBS, N7_SPADES, N7_HEARTS, N7_DIAMONDS
    }

    public class CardUtil
    {
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

        public static List<SheepCard> UnshuffledList()
        {
            var list = new List<SheepCard>();
            foreach (var ss in Enum.GetValues(typeof(SheepCard)))
                list.Add((SheepCard)ss);
            return list;
        }

        private static Dictionary<SheepCard, string> _filenamesByCard = new Dictionary<SheepCard, string>() {
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

        //♣♠♥♦
        private static Dictionary<SheepCard, string> _abbreviationsByCard = new Dictionary<SheepCard, string>() {
            { SheepCard.ACE_CLUBS, "A♣" },
            { SheepCard.ACE_SPADES, "A♠" },
            { SheepCard.ACE_HEARTS, "A♥" },
            { SheepCard.ACE_DIAMONDS, "A♦" },
            { SheepCard.KING_CLUBS, "K♣" },
            { SheepCard.KING_SPADES, "K♠" },
            { SheepCard.KING_HEARTS, "K♥" },
            { SheepCard.KING_DIAMONDS, "K♦" },
            { SheepCard.QUEEN_CLUBS, "Q♣" },
            { SheepCard.QUEEN_SPADES, "Q♠" },
            { SheepCard.QUEEN_HEARTS, "Q♥" },
            { SheepCard.QUEEN_DIAMONDS, "Q♦" },
            { SheepCard.JACK_CLUBS, "J♣" },
            { SheepCard.JACK_SPADES, "J♠" },
            { SheepCard.JACK_HEARTS, "J♥" },
            { SheepCard.JACK_DIAMONDS, "J♦" },
            { SheepCard.N10_CLUBS, "10♣" },
            { SheepCard.N10_SPADES, "10♠" },
            { SheepCard.N10_HEARTS, "10♥" },
            { SheepCard.N10_DIAMONDS, "10♦" },
            { SheepCard.N9_CLUBS, "9♣" },
            { SheepCard.N9_SPADES, "9♠" },
            { SheepCard.N9_HEARTS, "9♥" },
            { SheepCard.N9_DIAMONDS, "9♦" },
            { SheepCard.N8_CLUBS, "8♣" },
            { SheepCard.N8_SPADES, "8♠" },
            { SheepCard.N8_HEARTS, "8♥" },
            { SheepCard.N8_DIAMONDS, "8♦" },
            { SheepCard.N7_CLUBS, "7♣" },
            { SheepCard.N7_SPADES, "7♠" },
            { SheepCard.N7_HEARTS, "7♥" },
            { SheepCard.N7_DIAMONDS, "7♥" }
        };

        public static Dictionary<StandardSuite, string> SuiteLetter { get; } = new Dictionary<StandardSuite, string>()
        {
            { StandardSuite.CLUBS, "♣" }, { StandardSuite.DIAMONDS, "♦" }, { StandardSuite.HEARTS, "♥" }, { StandardSuite.SPADES, "♠" }
        };

        public static Dictionary<CardType, string> CardTypeLetter { get; } = new Dictionary<CardType, string>()
        {
            { CardType.ACE, "A" }, { CardType.JACK, "J" }, { CardType.KING, "K" }, { CardType.QUEEN, "Q" },
            { CardType.N10, "T" }, { CardType.N9, "9" }, { CardType.N8, "8" }, { CardType.N7, "7" }
        };

        public static string GetPictureFilename(SheepCard card)
        {
            return _filenamesByCard[card];
        }

        public static string GetAbbreviation(SheepCard card)
        {
            return _abbreviationsByCard[card];
        }

        public static SheepCard GetCardFromFilename(string filename)
        {
            return _filenamesByCard.First(l => l.Value == filename).Key;
        }

        public static SheepCard? GetCardFromAbbreviation(string abbr)
        {
            if (_abbreviationsByCard.Any(l => l.Value == abbr))
                return _abbreviationsByCard.First(l => l.Value == abbr).Key;
            return null;
        }

        public static string ToAbbr(SheepCard card)
        {
            var cardType = GetFace(card);
            var suit = GetStandardSuit(card);
            return CardTypeLetter[cardType] + SuiteLetter[suit];
        }

        public static string CardListToString(List<SheepCard> cardList)
        {
            return String.Join(";", cardList.Select(bc => GetAbbreviation(bc)));
        }

        public static List<SheepCard> StringToCardList(string cards)
        {
            return (cards ?? string.Empty).Split(';')
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c => GetCardFromAbbreviation(c).Value)
                .ToList();
        }

        public static CardSummary GetCardSummary(SheepCard card, bool? legalMove = null)
        {
            return new CardSummary()
            {
                Name = card.ToString(),
                Filename = GetPictureFilename(card),
                LegalMove = legalMove,
                Abbreviation = GetAbbreviation(card)
            };
        }
    }

    public struct CardSummary
    {
        public string Name { get; set; }
        public string Filename { get; set; }
        public bool? LegalMove { get; set; }
        public string Abbreviation { get; set; }
    }
}
