using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sheepshead.Models.Wrappers;
using Sheepshead.Models.Players.Stats;

namespace Utilities.GuessGeneration
{
    public class StatNormalizer
    {
        public void NormalizeStats(string sourcePath, string destPath, int newBaseHands)
        {
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);
            NormalizePicks(sourcePath, destPath, newBaseHands);
            NormalizeBuries(sourcePath, destPath, newBaseHands);
            NormalizeMoves(sourcePath, destPath, newBaseHands);
            NormalizeLeasters(sourcePath, destPath, newBaseHands);
        }

        private void NormalizePicks(string sourcePath, string destPath, int numberOfHands)
        {
            var streamWrapper = new StreamReaderWrapper(sourcePath + StatGenerator.PICK_SAVE_LOCATION);
            var statReader = StatReader<PickStatUniqueKey, PickStat>.FromFile(streamWrapper);
            StatReader<PickStatUniqueKey, PickStat>.StatMath statMath = (int baseNumber, PickStat stat) =>
            {
                if (stat.AvgPassedPoints.HasValue)
                {
                    stat.TotalPassedPoints = (int)Math.Round(baseNumber * (stat.AvgPassedPoints ?? 0));
                    stat.HandsPassed = baseNumber;
                }
                if (stat.AvgPickPoints.HasValue)
                {
                    stat.TotalPickPoints = (int)Math.Round(baseNumber * (stat.AvgPickPoints ?? 0));
                    stat.HandsPicked = baseNumber;
                }
            };
            statReader.NormalizeStats(numberOfHands, statMath);
            statReader.SaveToFile(new StreamWriterWrapper(destPath + StatGenerator.PICK_SAVE_LOCATION));
        }

        private void NormalizeBuries(string sourcePath, string destPath, int numberOfHands)
        {
            var streamWrapper = new StreamReaderWrapper(sourcePath + StatGenerator.BURY_SAVE_LOCATION);
            var statReader = StatReader<BuryStatUniqueKey, BuryStat>.FromFile(streamWrapper);
            StatReader<BuryStatUniqueKey, BuryStat>.StatMath statMath = (int baseNumber, BuryStat stat) =>
            {
                if (stat.AvgPickPoints.HasValue)
                {
                    stat.TotalPoints = (int)Math.Round(baseNumber * (stat.AvgPickPoints ?? 0));
                    stat.TotalPoints = baseNumber;
                }
            };
            statReader.NormalizeStats(numberOfHands, statMath);
            statReader.SaveToFile(new StreamWriterWrapper(destPath + StatGenerator.BURY_SAVE_LOCATION));
        }

        private void NormalizeMoves(string sourcePath, string destPath, int numberOfHands)
        {
            var streamWrapper = new StreamReaderWrapper(sourcePath + StatGenerator.MOVE_SAVE_LOCATION);
            var statReader = StatReader<MoveStatUniqueKey, MoveStat>.FromFile(streamWrapper);
            StatReader<MoveStatUniqueKey, MoveStat>.StatMath statMath = (int baseNumber, MoveStat stat) =>
            {
                if (stat.HandPortionWon.HasValue)
                {
                    stat.HandsWon = (int)Math.Round(baseNumber * (stat.HandPortionWon ?? 0));
                    stat.HandsTried = baseNumber;
                }
                if (stat.TrickPortionWon.HasValue)
                {
                    stat.TricksWon = (int)Math.Round(baseNumber * (stat.TrickPortionWon ?? 0));
                    stat.TricksTried = baseNumber;
                }
            };
            statReader.NormalizeStats(numberOfHands, statMath);
            statReader.SaveToFile(new StreamWriterWrapper(destPath + StatGenerator.MOVE_SAVE_LOCATION));
        }

        private void NormalizeLeasters(string sourcePath, string destPath, int numberOfHands)
        {
            var streamWrapper = new StreamReaderWrapper(sourcePath + StatGenerator.LEASTER_SAVE_LOCATION);
            var statReader = StatReader<LeasterStatUniqueKey, LeasterStat>.FromFile(streamWrapper);
            StatReader<LeasterStatUniqueKey, LeasterStat>.StatMath statMath = (int baseNumber, LeasterStat stat) =>
            {
                if (stat.HandPortionWon.HasValue)
                {
                    stat.HandsWon = (int)Math.Round(baseNumber * (stat.HandPortionWon ?? 0));
                    stat.HandsTried = baseNumber;
                }
            };
            statReader.NormalizeStats(numberOfHands, statMath);
            statReader.SaveToFile(new StreamWriterWrapper(destPath + StatGenerator.LEASTER_SAVE_LOCATION));
        }
    }
}
