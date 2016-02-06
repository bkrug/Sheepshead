using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.GuessGeneration
{
    /// <summary>
    /// The GuessGenerator will have several computer players run through a specifieid number of games.
    /// It records the statistical results of their moves as would happen in normal game play.
    /// Finally, it creates a "normalized" version of those results.
    /// If the raw statics say that a move was successfull 123 out of 256 times, the normalized results will likely say they were successful 5 out of 10 times.
    /// This is useful because when starting the web version for the first time,
    /// the Learning Players can begin with some basic data but data from playing against real humans will quickly overwhelm the normalized stats.
    /// </summary>
    public class GuessGenerator
    {
        public void GenerateGuesses(string statPath, string guessPath, int handsToPlay, int newBaseHands)
        {
            statPath = statPath.TrimEnd('\\').TrimEnd('/') + '\\';
            guessPath = guessPath.TrimEnd('\\').TrimEnd('/') + '\\';
            new StatGenerator().GenerateStats(statPath, handsToPlay);
            new StatNormalizer().NormalizeStats(statPath, guessPath, newBaseHands);
        }
    }
}
