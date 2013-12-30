using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;

namespace Sheepshead.Tests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void Player_HasSameNameAsUser()
        {
            var expectedName = "NameOfSomeoneImportant";
            var user = new Mock<IUser>();
            user.Setup(m => m.Name).Returns(expectedName);
            var player = new HumanPlayer(user.Object);
            Assert.AreEqual(expectedName, player.Name, "Name of player object and user object match.");
        }
    }
}
