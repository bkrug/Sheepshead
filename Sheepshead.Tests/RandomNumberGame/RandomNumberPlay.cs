using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Sheepshead.Tests.RandomNumberGame
{
    [TestClass]
    public class RandomNumberPlay
    {
        [TestMethod]
        public void PlayRandomNumbers()
        {
            var rnd = new Random();
            var game = new Game(rnd, new EasyToMakePositive(rnd));
            var playObj = new GamePlayer(game);
            playObj.Play();
        }
    }
}
