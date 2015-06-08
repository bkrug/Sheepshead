using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sheepshead.Models;
using Sheepshead.Models.Players.Stats;

namespace Sheepshead.Tests.NonUnitTests
{
    [TestClass]
    public class LearningStats
    {
        private Dictionary<string, Tuple<int, int>> _ranges = new Dictionary<string, Tuple<int, int>>()
        {
            { "PointsInTrick", new Tuple<int, int>(0, 44) },
            { "HighestRankInTrick", new Tuple<int, int>(1, 33) },
            { "MorePowerfulUnknownCards", new Tuple<int, int>(0, 31) },
            { "RemainingUnknownPoints", new Tuple<int, int>(0, 120) },
            { "MorePowerfulHeld", new Tuple<int, int>(0, 5) },
            { "PointsHeld", new Tuple<int, int>(0, 54) },
            { "CardsHeldWithPoints", new Tuple<int, int>(0, 5) },
            { "MoveIndex", new Tuple<int, int>(1, 5) },
            { "TrickIndex", new Tuple<int, int>(1, 6) }
        };
    }
}
