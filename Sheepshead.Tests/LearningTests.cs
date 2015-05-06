using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Tests
{
    [TestClass]
    public class LearningTests
    {
        [TestMethod]
        public void Learning_GenerateKey()
        {
            var expectedKey = new MoveStatUniqueKey()
            {
                Picker = 2,
                Partner = 3,
                Trick = 3,
                MoveWithinTrick = 4,
                PointsAlreadyInTrick = 32,
                TotalPointsInPreviousTricks = 16,
                PointsInThisCard = 10,
                RankOfThisCard = 9,
                PartnerCard = false,
                HigherRankingCardsPlayedPreviousTricks = 8,
                HigherRankingCardsPlayedThisTrick = 1
            };

            var cardMock = new Mock<ICard>();
            cardMock.Setup(m => m.Points).Returns(expectedKey.PointsInThisCard);
            cardMock.Setup(m => m.Rank).Returns(expectedKey.RankOfThisCard);
            //The following three cards include one higher ranking card.  Also notice that they add up to the PointsAlreadyInTrick value.
            var prevCard1Mock = new Mock<ICard>();
            prevCard1Mock.Setup(m => m.Points).Returns(expectedKey.PointsAlreadyInTrick - 7);
            prevCard1Mock.Setup(m => m.Rank).Returns(cardMock.Object.Rank - 1);
            var prevCard2Mock = new Mock<ICard>();
            prevCard2Mock.Setup(m => m.Points).Returns(3);
            prevCard2Mock.Setup(m => m.Rank).Returns(cardMock.Object.Rank + 1);
            var prevCard3Mock = new Mock<ICard>();
            prevCard3Mock.Setup(m => m.Points).Returns(4);
            prevCard3Mock.Setup(m => m.Rank).Returns(cardMock.Object.Rank + 1);
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.QueueRankOfPicker).Returns(expectedKey.Picker);
            trickMock.Setup(m => m.QueueRankOfPartner).Returns(expectedKey.Partner.Value);
            trickMock.Setup(m => m.IndexInHand).Returns(expectedKey.Trick);
            trickMock.Setup(m => m.CardsPlayed).Returns(new Dictionary<IPlayer, ICard>()
            {
                 { new Mock<IPlayer>().Object, prevCard1Mock.Object },
                 { new Mock<IPlayer>().Object, prevCard2Mock.Object },
                 { new Mock<IPlayer>().Object, prevCard3Mock.Object },
            });
            trickMock.Setup(m => m.PartnerCard).Returns(new Mock<ICard>().Object);
            trickMock.Setup(m => m.Hand).Returns(GetHand(trickMock.Object, expectedKey.Trick - 1, expectedKey.TotalPointsInPreviousTricks, expectedKey.HigherRankingCardsPlayedPreviousTricks));
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.QueueRankInTrick(trickMock.Object)).Returns(expectedKey.MoveWithinTrick);

            var actualKey = LearningHelper.GenerateKey(trickMock.Object, playerMock.Object, cardMock.Object);

            Assert.AreEqual(expectedKey.Picker, actualKey.Picker, "Picker");
            Assert.AreEqual(expectedKey.Partner, actualKey.Partner, "Partner");
            Assert.AreEqual(expectedKey.Trick, actualKey.Trick, "Trick Index");
            Assert.AreEqual(expectedKey.MoveWithinTrick, actualKey.MoveWithinTrick, "MoveWithinTrick");
            Assert.AreEqual(expectedKey.PointsAlreadyInTrick, actualKey.PointsAlreadyInTrick, "PointsAlreadyInTrick");
            Assert.AreEqual(expectedKey.TotalPointsInPreviousTricks, actualKey.TotalPointsInPreviousTricks, "TotalPointsInPreviousTricks");
            Assert.AreEqual(expectedKey.PointsInThisCard, actualKey.PointsInThisCard, "PointsInThisCard");
            Assert.AreEqual(expectedKey.RankOfThisCard, actualKey.RankOfThisCard, "RankOfThisCard");
            Assert.AreEqual(expectedKey.PartnerCard, actualKey.PartnerCard, "ParnerCard");
            Assert.AreEqual(expectedKey.HigherRankingCardsPlayedPreviousTricks, actualKey.HigherRankingCardsPlayedPreviousTricks, "HigherRankingCardsPlayedPreviousTricks");
            Assert.AreEqual(expectedKey.HigherRankingCardsPlayedThisTrick, actualKey.HigherRankingCardsPlayedThisTrick, "HigherRankingCardsPlayedThisTrick");
        }

        private IHand GetHand(ITrick thisTrick, int previousTricks, int pointsInPreviousTricks, int higherRankingCards)
        {
            var handMock = new Mock<IHand>();
            var tricks = new List<ITrick>();
            for (var i = 0; i < previousTricks; ++i)
            {
                var trickMock = new Mock<ITrick>();
                trickMock.Setup(m => m.IndexInHand).Returns(i + 1);
                var dict = new Dictionary<IPlayer, ICard>();
                for (var j = 0; j < 5; ++j)
                {
                    int points = 0;
                    if (i == 0 && j == 0) points = pointsInPreviousTricks - 1;
                    if (i == 0 && j == 1) points = 1;
                    int rank = 100;
                    if (higherRankingCards > 0)
                    {
                        rank = 0;
                        higherRankingCards--;
                    }
                    var cardMock = new Mock<ICard>();
                    cardMock.Setup(m => m.Points).Returns(points);
                    cardMock.Setup(m => m.Rank).Returns(rank);
                    dict.Add(new Mock<IPlayer>().Object, cardMock.Object);
                }
                trickMock.Setup(m => m.CardsPlayed).Returns(dict);
                tricks.Add(trickMock.Object);
            }
            if (higherRankingCards > 0)
                throw new ApplicationException("Invalid Test");
            tricks.Add(thisTrick);
            for (var i = previousTricks + 1; i < 5; ++i)
            {
                var trickMock = new Mock<ITrick>();
                trickMock.Setup(m => m.IndexInHand).Returns(i + 1);
                var dict = new Dictionary<IPlayer, ICard>();
                for (var j = 0; j < 5; ++j)
                {
                    int points = 0;
                    if (i == previousTricks + 1 && j == 0) points = 10;
                    if (i == previousTricks + 1 && j == 1) points = 1;
                    int rank = points > 0 ? 2 : 100;
                    var cardMock = new Mock<ICard>();
                    cardMock.Setup(m => m.Points).Returns(points);
                    cardMock.Setup(m => m.Rank).Returns(rank);
                    dict.Add(new Mock<IPlayer>().Object, cardMock.Object);
                }
                trickMock.Setup(m => m.CardsPlayed).Returns(dict);
                tricks.Add(trickMock.Object);
            }

            handMock.Setup(m => m.Tricks).Returns(tricks);
            return handMock.Object;
        }

        [TestMethod]
        public void LearningHelper_FromSummary() {
            var summary = "7HJD,2ASTC,8H7SAHJDKC,8DQS8SAD9H,KSACJH9CTH,QDKH7C7HJS,7DJC9DKDTS,9SQCTD8CQH";
            IHand hand = SummaryReader.FromSummary(summary);
            var players = hand.Players;
            Assert.AreEqual(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], hand.Deck.Blinds[0]);
            Assert.AreEqual(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.JACK], hand.Deck.Blinds[1]);
            Assert.AreSame(hand.Players[2 - 1], hand.Picker, "Second player is picker.");
            Assert.AreEqual(CardRepository.Instance[StandardSuite.CLUBS, CardType.N10], hand.Deck.Buried[1]);
            Assert.AreEqual(CardRepository.Instance[StandardSuite.HEARTS, CardType.N8], hand.Tricks[0].CardsPlayed[players[0]]);
            Assert.AreEqual(CardRepository.Instance[StandardSuite.CLUBS, CardType.KING], hand.Tricks[0].CardsPlayed[players[4]]);
            Assert.AreEqual(CardRepository.Instance[StandardSuite.CLUBS, CardType.N7], hand.Tricks[3].CardsPlayed[players[2]]);
            Assert.AreEqual(CardRepository.Instance[StandardSuite.DIAMONDS, CardType.KING], hand.Tricks[4].CardsPlayed[players[3]]);
            
            var leastersSummary = "7HJD,,8H7SAHJDKC,8DQS8SAD9H,KSACJH9CTH,QDKH7C7HJS,7DJC9DKDTS,9SQCTD8CQH";
            IHand leasterHand = SummaryReader.FromSummary(leastersSummary);
            Assert.IsTrue(leasterHand.Leasters);
            Assert.IsTrue(leasterHand.Deck.Buried == null || !leasterHand.Deck.Buried.Any());
            Assert.IsNull(leasterHand.Picker);
        }
    }
}
