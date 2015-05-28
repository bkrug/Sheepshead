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

        //[TestMethod]
        public void CompareStats()
        {
            var movesAndStats = SummaryLoader.ReadMoves(@"c:\temp\HandSummaries.txt", 0, 2500);
            using (var sw = new StreamWriter(@"C:\Temp\propCompare.csv"))
            {
                sw.WriteLine("Property Name,Value,Trick,Move,Attempts,Hands Won %,Tricks Won %");
                foreach (var experimentProp in typeof(MoveStatUniqueKey2).GetFields().Where(p => !(new List<string>() {"MoveIndex", "TrickIndex"}).Contains(p.Name)))
                for(var t = 0; t < 6; ++t)
                for(var m = 0; m < 5; ++m)
                foreach (var testValue in GetTestValues(experimentProp))
                {
                    var stats = movesAndStats.Where(kvp => kvp.Key.TrickIndex == t
                                                        && kvp.Key.MoveIndex == m
                                                        && object.Equals(experimentProp.GetValue(kvp.Key), testValue))
                                             .Select(kvp => kvp.Value)
                                             .ToList();
                    var handsTried = stats.Sum(s => s.HandsTried);
                    if (handsTried == 0)
                        continue;
                    var avgHand = handsTried == 0 ? 0 : (double)stats.Sum(s => s.HandsWon) / handsTried;
                    var avgTrick = handsTried == 0 ? 0 : (double)stats.Sum(s => s.TricksWon) / stats.Sum(s => s.TricksTried);
                    var testValueAsString = testValue == null ? "" : testValue.ToString();
                    sw.WriteLine(experimentProp.Name + "," + testValueAsString + "," + t + "," + m + "," + handsTried + "," + avgHand.ToString("P3") + "," + avgTrick.ToString("P3"));
                }
            }
        }

        private List<object> GetTestValues(FieldInfo property)
        {
            if (property.FieldType == typeof(bool))
                return new List<object>() { true, false };
            if (property.FieldType == typeof(bool?))
                return new List<object>() { true, false, null };
            if (!_ranges.ContainsKey(property.Name))
                return new List<object>();
            var range = _ranges[property.Name];
            if (range.Item2 < range.Item1)
                return new List<object>();
            var list = new List<object>();
            for (var i = range.Item1; i <= range.Item2; ++i)
                list.Add(i);
            return list;
        }

        //private MoveStatUniqueKey GetKey(int roomNo, MoveStatCentroid centroid)
        //{
        //    var key = new MoveStatUniqueKey()
        //    {
        //        CardsHeldWithPoints = centroid.CardsHeldWithPoints,
        //        HighestRankInTrick = centroid.HighestRankInTrick,
        //        MorePowerfulHeld = centroid.MorePowerfulHeld,
        //        MorePowerfulUnknownCards = centroid.MorePowerfulUnknownCards,
        //        MoveIndex = centroid.MoveIndex,
        //        PointsHeld = centroid.PointsHeld,
        //        PointsInTrick = centroid.PointsInTrick,
        //        RemainingUnknownPoints = centroid.RemainingUnknownPoints,
        //        TrickIndex = centroid.TrickIndex
        //    };
        //    key.OffenseSide = roomNo % 2 == 1;
        //    roomNo /= 2;
        //    key.PickerDone = roomNo % 2 == 1;
        //    roomNo /= 2;
        //    key.WinningSide = roomNo % 2 == 1;
        //    roomNo /= 2;
        //    key.ThisCardMorePowerful = roomNo % 2 == 1;
        //    roomNo /= 2;
        //    switch(roomNo){
        //        case 0:
        //            key.PartnerDone = false;
        //            break;
        //        case 1:
        //            key.PartnerDone = true;
        //            break;
        //        default:
        //            key.PartnerDone = null;
        //            break;
        //    }
        //    return key;
        //}

        //get
        //{
        //    return (OffenseSide ? 1 : 0)
        //        + (PickerDone ? 1 : 0) * 2
        //        + (WinningSide ? 1 : 0) * 2 * 2
        //        + (ThisCardMorePowerful ? 1 : 0) * 2 * 2 * 2
        //        + PartnerDoneNo * 2 * 2 * 2 * 2;
        //}
    }
}
