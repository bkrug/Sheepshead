using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players;

namespace Sheepshead.Tests
{
    [TestClass]
    public class AdvancedPlayerTests
    {
        [TestMethod]
        public void AdvancedPlayer_GetMove()
        {
        }


        //Tests:
        //* In five player game, you have many powerful trump. Pick.
        //* In five player game, you have many weak trump, and points you would like to bury. Pick.
        //* In five player game, you have many weak trump, but no points to bury, and are towards the front. Pass.
        //* In five player game, you have many weak trump, but no points to bury, but are towards the back. Pick.
        //* In five player game, you have few trump. Pass.
        //* In three player game, you have many trump and many aces or tens. Pick.
        //* In three player game, you have many trump but fewer aces and tens, and are towards the front. Pass.
        //* In three player game, you have not quite so many trump, aces, and tens, but are towards the back. Pick.
        //* In three player game, you just don't have much. Pass.

        //* You lead the trick as an offensive player, lead with weak trump.
        //* You lead the trick as a defensive player, lead with the suit of called ace.

        //* Not all your opponents have played, your team is winning with the most powerful remaining card. Give away points.
        //* Not all your opponents have played, your team is winning. Give away points.
        //* Not all your opponents have played, your team is loosing. There are points to be had. You have the most powerful remaining card. Play that card.
        //* Not all your opponents have played, your team is loosing. There are points to be had. You have not the most powerful remaining card. Give away (fewest possible) points.
        //* Not all your opponents have played. Your team is loosing. There are not many points. Give away (fewest possible) points.

        //* All your opponents have played, your team is winning the trick. Give away points if you can.
        //* All your opponents have played, your team is not winning. There are points to be won. Play a card that wins the trick.
        //* All your opponents have played, your team is not winning. You can't make them win. Give away (fewest possible) points.
        //* All your opponents have played, your team is not winning. You can't make them win. Give away something worthless.

        //* You think all your opponents have played, your team is winning the trick. Give away points if you can.
        //* You think all your opponents have played, your team is not winning. There are points to be won. Play a card that wins the trick.
        //* You think all your opponents have played, your team is not winning. You can't make them win. Give away (fewest possible) points.
        //* You think all your opponents have played, your team is not winning. You can't make them win. Give away something worthless.

        //* In leasters, you have a lot of high value cards, so give them up in hands that someone else one.
        //* In leasters, you have more limited high value cards, give them up in hands one by someone with many points.
    }
}
