using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;

namespace Sheepshead.Tests
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        public void Card_ToAbbr()
        {
            var card1 = new Card(StandardSuite.DIAMONDS, CardType.JACK, 0, 0);
            Assert.AreEqual("JD", card1.ToAbbr());
            var card2 = new Card(StandardSuite.HEARTS, CardType.N7, 0, 0);
            Assert.AreEqual("7H", card2.ToAbbr());
        }
    }
}
