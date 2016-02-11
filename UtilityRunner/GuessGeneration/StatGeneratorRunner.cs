using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.GuessGeneration;

namespace UtilityRunner.GuessGeneration
{
    [TestClass]
    public class GussGenerationRunner
    {
        //[TestMethod]
        public void Generate()
        {
            string statPath = @"C:\generator\foundStats\";
            string guessPath = @"C:\generator\NormalizedStats\";
            int handsToPlay = 20 * 1000;
            int newBaseHands = 10;
            new GuessGenerator().GenerateGuesses(statPath, guessPath, handsToPlay, newBaseHands);
        }
    }
}
