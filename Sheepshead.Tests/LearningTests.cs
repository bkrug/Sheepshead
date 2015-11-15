using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Tests
{
    [TestClass]
    public class LearningTests
    {
        [TestMethod]
        public void LearningHelper_FromSummary() {
            var summary = "7HAC,2ASTC,8H7SAHJDKC,8DQS8SAD9H,KSACJH9CTH,QDKH7C7HJS,7DJC9DKDTS,9SQCTD8CQH";
            IHand hand = SummaryReader.FromSummary(summary);
            var players = hand.Players;
            Assert.AreEqual(CardRepository.Instance[StandardSuite.HEARTS, CardType.N7], hand.Deck.Blinds[0]);
            Assert.AreEqual(CardRepository.Instance[StandardSuite.CLUBS, CardType.ACE], hand.Deck.Blinds[1]);
            Assert.AreSame(hand.Players[2 - 1], hand.Picker, "Second player is picker.");
            Assert.AreSame(hand.Players[4 - 1], hand.Partner, "Forth player is partner.");
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

        [TestMethod]
        public void LearningPlayer_GetMove()
        {
            var cardList = new List<ICard>() { new Mock<ICard>().Object, new Mock<ICard>().Object, new Mock<ICard>().Object };
            var keyList = new List<MoveStatUniqueKey>()
            {
                new MoveStatUniqueKey() { CardPoints = 11 },
                new MoveStatUniqueKey() { CardPoints = 10 },
                new MoveStatUniqueKey() { CardPoints = 4 }
            };
            var statList = new List<MoveStat>() 
            {
                new MoveStat() { HandsWon = 72, HandsTried = 100, TricksWon = 63, TricksTried = 100 },
                new MoveStat() { HandsWon = 74, HandsTried = 100, TricksWon = 69, TricksTried = 100 },
                new MoveStat() { HandsWon = 49, HandsTried = 100, TricksWon = 49, TricksTried = 100 }
            };
            var moveKeyGenMock = new Mock<IMoveKeyGenerator>();
            moveKeyGenMock
                .Setup(m => m.GenerateKey(It.IsAny<ITrick>(), It.IsAny<IPlayer>(), It.IsAny<ICard>()))
                .Returns((ITrick t, IPlayer p, ICard c) => keyList[cardList.IndexOf(c)]);
            var pickKeyGenMock = new Mock<IPickKeyGenerator>();
            var buryKeyGenMock = new Mock<IBuryKeyGenerator>();
            var leasterKeyGenMock = new Mock<ILeasterKeyGenerator>();
            var predictorMock = new Mock<IStatResultPredictor>();
            predictorMock
                .Setup(m => m.GetWeightedStat(It.IsAny<MoveStatUniqueKey>()))
                .Returns((MoveStatUniqueKey key) => statList[keyList.IndexOf(key)]);
            var pickPredictorMock = new Mock<IPickResultPredictor>();
            var buryPredictorMock = new Mock<IBuryResultPredictor>();
            var leasterPredictorMock = new Mock<ILeasterResultPredictor>();
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.IsLegalAddition(It.IsAny<ICard>(), It.IsAny<IPlayer>())).Returns(true);
            var player = new LearningPlayer(moveKeyGenMock.Object, predictorMock.Object, pickKeyGenMock.Object, pickPredictorMock.Object, buryKeyGenMock.Object, buryPredictorMock.Object, 
                leasterKeyGenMock.Object, leasterPredictorMock.Object);
            player.Cards.AddRange(cardList);

            var actualCard = player.GetMove(trickMock.Object);
            Assert.AreSame(cardList[1], actualCard, "We should see the get the second card in the list back because the second move stat has the best trick result. Actual result: " + (cardList.IndexOf(actualCard) + 1));

            statList = new List<MoveStat>() 
            {
                new MoveStat() { HandsWon = 74, HandsTried = 100, TricksWon = 63, TricksTried = 100 },
                new MoveStat() { HandsWon = 72, HandsTried = 100, TricksWon = 69, TricksTried = 100 },
                new MoveStat() { HandsWon = 49, HandsTried = 100, TricksWon = 87, TricksTried = 100 }
            };
            predictorMock
                .Setup(m => m.GetWeightedStat(It.IsAny<MoveStatUniqueKey>()))
                .Returns((MoveStatUniqueKey key) => statList[keyList.IndexOf(key)]); 
            actualCard = player.GetMove(trickMock.Object);
            Assert.AreSame(cardList[1], actualCard, "We should see the get the second card in the list back because the first and second move stats have similar hand results, but the trick results are better for the second hand. Actual result: " + (cardList.IndexOf(actualCard) + 1));

            statList[1] = null;
            actualCard = player.GetMove(trickMock.Object);
            Assert.AreSame(cardList[0], actualCard, "If the predictor returns null for one of the moves, that shouldn't hurt anything.");

            statList[0] = null;
            statList[2] = null;
            trickMock.Setup(m => m.Hand).Returns(new Mock<IHand>().Object);
            trickMock.Setup(m => m.Players).Returns(new List<IPlayer>() { player, new Mock<IPlayer>().Object });
            trickMock.Setup(m => m.PlayerCount).Returns(5);
            actualCard = player.GetMove(trickMock.Object);
            Assert.IsTrue(actualCard is ICard, "If the predictor returns null for all of the moves, just guess.");
        }

        [TestMethod]
        public void LearningPlayer_WillPick()
        {
            var keyGeneratorMock = new Mock<IMoveKeyGenerator>();
            var pickKeyGeneratorMock = new Mock<IPickKeyGenerator>();
            var buryKeyGeneratorMock = new Mock<IBuryKeyGenerator>();
            var leasterKeyGeneratorMock = new Mock<ILeasterKeyGenerator>();
            var movePredictorMock = new Mock<IStatResultPredictor>();
            var pickPredictorMock = new Mock<IPickResultPredictor>();
            var buryPredictorMock = new Mock<IBuryResultPredictor>();
            var leasterPredictorMock = new Mock<ILeasterResultPredictor>();
            var player = new LearningPlayer(keyGeneratorMock.Object, movePredictorMock.Object, pickKeyGeneratorMock.Object, pickPredictorMock.Object, buryKeyGeneratorMock.Object, buryPredictorMock.Object,
                leasterKeyGeneratorMock.Object, leasterPredictorMock.Object);

            var deckMock = new Mock<IDeck>();
            var handMock = new Mock<IHand>();
            deckMock.Setup(m => m.Hand).Returns(handMock.Object);

            var key = new PickStatUniqueKey() { TotalCardsWithPoints = 4 };
            pickKeyGeneratorMock.Setup(m => m.GenerateKey(It.IsAny<IHand>(), It.IsAny<IPlayer>())).Returns(key);

            var stat = new PickStat()
            {
                TotalPickPoints = 10,
                HandsPicked = 30,
                TotalPassedPoints = -20,
                HandsPassed = 50
            };
            pickPredictorMock.Setup(m => m.GetWeightedStat(It.IsAny<PickStatUniqueKey>())).Returns(stat);
            var willPick = player.WillPick(deckMock.Object);
            Assert.IsTrue(willPick, "If the (average) pick points are higher than the passed points, the player should pick.");

            stat = new PickStat()
            {
                TotalPickPoints = 10,
                HandsPicked = 30,
                TotalPassedPoints = 20,
                HandsPassed = 200
            };
            pickPredictorMock.Setup(m => m.GetWeightedStat(It.IsAny<PickStatUniqueKey>())).Returns(stat);
            willPick = player.WillPick(deckMock.Object);
            Assert.IsTrue(willPick, "If the (average) pick points are higher than the passed points, the player should pick.");

            stat = new PickStat()
            {
                TotalPickPoints = 10,
                HandsPicked = 30,
                TotalPassedPoints = 100,
                HandsPassed = 200
            };
            pickPredictorMock.Setup(m => m.GetWeightedStat(It.IsAny<PickStatUniqueKey>())).Returns(stat);
            willPick = player.WillPick(deckMock.Object);
            Assert.IsFalse(willPick, "If the (average) pick points are lower than the passed points, the player should pass.");
        }

        [TestMethod]
        public void LearningPlayer_BuryCards()
        {
            var keyGeneratorMock = new Mock<IMoveKeyGenerator>();
            var pickKeyGeneratorMock = new Mock<IPickKeyGenerator>();
            var buryKeyGeneratorMock = new Mock<IBuryKeyGenerator>();
            var leasterKeyGeneratorMock = new Mock<ILeasterKeyGenerator>();
            var movePredictorMock = new Mock<IStatResultPredictor>();
            var pickPredictorMock = new Mock<IPickResultPredictor>();
            var buryPredictorMock = new Mock<IBuryResultPredictor>();
            var leasterPredictorMock = new Mock<ILeasterResultPredictor>();
            var player = new LearningPlayer(keyGeneratorMock.Object, movePredictorMock.Object, pickKeyGeneratorMock.Object, pickPredictorMock.Object, buryKeyGeneratorMock.Object, buryPredictorMock.Object,
                leasterKeyGeneratorMock.Object, leasterPredictorMock.Object);
            var deckMock = new Mock<IDeck>();

            var buried1 = CardRepository.Instance[StandardSuite.HEARTS, CardType.N10];
            var buried2 = CardRepository.Instance[StandardSuite.HEARTS, CardType.ACE];
            deckMock.Setup(m => m.Blinds).Returns(new List<ICard>() {
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.JACK], //Rank: 8
                buried1
            });
            player.Cards.AddRange(new List<ICard>() {
                buried2,
                CardRepository.Instance[StandardSuite.DIAMONDS, CardType.N9], //Rank: 18
                CardRepository.Instance[StandardSuite.HEARTS, CardType.QUEEN], //Rank: 3
                CardRepository.Instance[StandardSuite.SPADES, CardType.QUEEN], //Rank: 2
                CardRepository.Instance[StandardSuite.CLUBS, CardType.KING], //Rank: 17
                CardRepository.Instance[StandardSuite.CLUBS, CardType.JACK] //Rank: 5
            });
            var bestKey = new BuryStatUniqueKey()
            {
                BuriedPoints = 10 + 11,
                AvgPointsInHand = (int)(Math.Round((4 + 3 + 3 + 2 + 2 + 0) / (double)6)),
                AvgRankInHand = (int)(Math.Round((8 + 18 + 3 + 2 + 17 + 5) / (double)6)),
                SuitsInHand = 2
            };
            buryKeyGeneratorMock
                .Setup(m => m.GenerateKey(It.IsAny<List<ICard>>(), It.IsAny<List<ICard>>()))
                .Returns((List<ICard> held, List<ICard> buried) =>
                    buried.Any(c => CardRepository.Equals(c, buried1)) && buried.Any(c => CardRepository.Equals(c, buried2)) ? bestKey : new BuryStatUniqueKey());
            var expectedResult = new BuryStat() { TotalPoints = -300, HandsPicked = 300 };
            var combosTested = 0;
            buryPredictorMock
                .Setup(m => m.GetWeightedStat(It.IsAny<BuryStatUniqueKey>()))
                .Callback(() => { ++combosTested; })
                .Returns(new BuryStat() { TotalPoints = -200, HandsPicked = 100 });
            buryPredictorMock
                .Setup(m => m.GetWeightedStat(bestKey))
                .Callback(() => { ++combosTested; })
                .Returns(expectedResult);

            var actualCards = player.DropCardsForPick(deckMock.Object);
            //28 because that is the total number of combinations of 2 that you can pull from a list of 8 cards.
            Assert.AreEqual(28, combosTested, "Should test every possible combination of cards");
            Assert.IsTrue(actualCards.Any(c => CardRepository.Equals(c, buried1)));
            Assert.IsTrue(actualCards.Any(c => CardRepository.Equals(c, buried2)));
        }
    }
}
