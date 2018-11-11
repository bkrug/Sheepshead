using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;
using Sheepshead.Tests.PlayerMocks;

namespace Sheepshead.Tests
{
    [TestClass]
    public class GameStateAnalyzerTests
    {
        #region All Opponents Have Played
        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Defense_PartnerProbablyKnown_Yes()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { pickerMock.Object, SheepCard.QUEEN_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns(partnerMock.Object);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns(partnerMock.Object);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Defense_PartnerUnknown_Yes()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { pickerMock.Object, SheepCard.QUEEN_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.KING_CLUBS },
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns((IPlayer)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((IPlayer)null);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Defense_PartnerUnknown_Null()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { pickerMock.Object, SheepCard.QUEEN_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((IPlayer)null);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Defense_PartnerProbablyKnown_No()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { pickerMock.Object, SheepCard.QUEEN_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { new Mock<IPlayer>().Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns((IPlayer)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns(partnerMock.Object);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Defense_PartnerUnknown_No()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.QUEEN_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns(partnerMock.Object);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((IPlayer)null);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Offense_PartnerProbablyKnown_Yes()
        {
            var partnerCard = SheepCard.JACK_HEARTS;
            var pickerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>() { partnerCard });
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { new Mock<IPlayer>().Object, SheepCard.N8_SPADES },
                { new Mock<IPlayer>().Object, SheepCard.JACK_DIAMONDS },
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((IPlayer)null);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(partnerCard);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Offense_PartnerUnknown_Yes()
        {
            var partnerCard = SheepCard.JACK_HEARTS;
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { partnerMock.Object, SheepCard.N8_SPADES },
                { new Mock<IPlayer>().Object, SheepCard.JACK_DIAMONDS },
                { new Mock<IPlayer>().Object, SheepCard.ACE_HEARTS },
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(playerMock.Object);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(partnerCard);
            trickMock.Setup(m => m.Hand.Partner).Returns((IPlayer)null);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Offense_PartnerUnknown_Null()
        {
            var partnerCard = SheepCard.JACK_HEARTS;
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { partnerMock.Object, SheepCard.N8_SPADES },
                { new Mock<IPlayer>().Object, SheepCard.JACK_DIAMONDS },
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(playerMock.Object);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((IPlayer)null);
            trickMock.Setup(m => m.Hand.Partner).Returns(partnerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(partnerCard);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Offense_PartnerProbablyKnown_No()
        {
            var partnerCard = SheepCard.JACK_HEARTS;
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { partnerMock.Object, SheepCard.N8_SPADES },
                { new Mock<IPlayer>().Object, SheepCard.JACK_DIAMONDS },
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(playerMock.Object);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns(partnerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(partnerCard);
            trickMock.Setup(m => m.Hand.Partner).Returns((IPlayer)null);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_AllOpponentsHavePlayed_Offense_PartnerUnknown_No()
        {
            var partnerCard = SheepCard.JACK_HEARTS;
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N8_DIAMONDS },
                { new Mock<IPlayer>().Object, SheepCard.ACE_HEARTS },
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(playerMock.Object);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(partnerCard);
            trickMock.Setup(m => m.Hand.Deck.Game.PlayerCount).Returns(5);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.AllOpponentsHavePlayed(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }
        #endregion

        #region My Side Winning

        [TestMethod]
        public void GameStateAnalyzer_MySideWinning_Defense_PartnerProbablyKnown_Yes()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.QUEEN_HEARTS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns(partnerMock.Object);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MySideWinning(playerMock.Object, trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_MySideWinning_Defense_PartnerProbablyKnown_No()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.KING_HEARTS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns(partnerMock.Object);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MySideWinning(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_MySideWinning_Defense_PartnerUnknown_No()
        {
            var pickerMock = new Mock<IPlayer>();
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.QUEEN_HEARTS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((Player)null);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MySideWinning(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_MySideWinning_Offense_PartnerProbablyKnown_Yes()
        {
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.KING_HEARTS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(playerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns(partnerMock.Object);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MySideWinning(playerMock.Object, trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_MySideWinning_Offense_PartnerProbablyKnown_No()
        {
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.QUEEN_HEARTS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(playerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns(partnerMock.Object);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MySideWinning(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_MySideWinning_Offense_ThisIsPartner_Yes()
        {
            var partnerCard = SheepCard.JACK_HEARTS;
            var pickerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>() { partnerCard });
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.KING_HEARTS },
                { pickerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(partnerCard);
            trickMock.Setup(m => m.Hand.Partner).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((Player)null);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MySideWinning(playerMock.Object, trickMock.Object);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_MySideWinning_Offense_ThisIsPartner_No()
        {
            var partnerCard = SheepCard.JACK_HEARTS;
            var pickerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>() { partnerCard });
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.QUEEN_HEARTS },
                { pickerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(pickerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(partnerCard);
            trickMock.Setup(m => m.Hand.Partner).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((Player)null);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MySideWinning(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_MySideWinning_Offense_PartnerUnknown_No()
        {
            var partnerMock = new Mock<IPlayer>();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>());
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.KING_HEARTS },
                { partnerMock.Object, SheepCard.JACK_CLUBS }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);
            trickMock.Setup(m => m.Hand.Picker).Returns(playerMock.Object);
            trickMock.Setup(m => m.Hand.PartnerCard).Returns(SheepCard.JACK_DIAMONDS);
            trickMock.Setup(m => m.Hand.Partner).Returns((Player)null);
            trickMock.Setup(m => m.Hand.PresumedParnter).Returns((Player)null);
            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MySideWinning(playerMock.Object, trickMock.Object);
            Assert.AreEqual(false, actual);
        }

        #endregion

        #region My Cards That Can Win

        [TestMethod]
        public void GameStateAnalyzer_MyCardsThatCanWin_SomeResults()
        {
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>() {
                SheepCard.KING_HEARTS,
                SheepCard.N8_HEARTS,
                SheepCard.N8_DIAMONDS,
                SheepCard.KING_SPADES,
                SheepCard.ACE_DIAMONDS,
                SheepCard.ACE_CLUBS
            });
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N10_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N10_CLUBS },
                { new Mock<IPlayer>().Object, SheepCard.ACE_SPADES }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);

            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MyCardsThatCanWin(playerMock.Object, trickMock.Object);
            var expected = new List<SheepCard>() { SheepCard.KING_HEARTS, SheepCard.N8_DIAMONDS, SheepCard.ACE_DIAMONDS };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void GameStateAnalyzer_MyCardsThatCanWin_NoResults()
        {
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(m => m.Cards).Returns(new List<SheepCard>() {
                SheepCard.KING_HEARTS,
                SheepCard.N8_HEARTS,
                SheepCard.N8_DIAMONDS,
                SheepCard.KING_SPADES,
                SheepCard.ACE_DIAMONDS,
                SheepCard.ACE_CLUBS
            });
            var cardsPlayed = new Dictionary<IPlayer, SheepCard>() {
                { new Mock<IPlayer>().Object, SheepCard.N7_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.N10_HEARTS },
                { new Mock<IPlayer>().Object, SheepCard.QUEEN_CLUBS },
                { new Mock<IPlayer>().Object, SheepCard.ACE_SPADES }
            };
            var trickMock = new Mock<ITrick>();
            trickMock.Setup(m => m.CardsPlayed).Returns(cardsPlayed);

            var analyzer = new GameStateAnalyzer();
            var actual = analyzer.MyCardsThatCanWin(playerMock.Object, trickMock.Object);
            var expected = new List<SheepCard>() { };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        #endregion
    }
}
