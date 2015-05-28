using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;
using Sheepshead.Models.Players.Stats;
using System.IO;

namespace Sheepshead.Models
{
    public class LearningHelper
    {
        private string _saveLocation;

        private LearningHelper()
        {
        }

        public LearningHelper(IHand hand, string saveLocation)
        {
            _saveLocation = saveLocation;
            hand.OnHandEnd += WriteHandSummary;
        }

        private static object lockObject = new object();

        private void WriteHandSummary(object sender, EventArgs e)
        {
            var hand = (IHand)sender;
            lock (lockObject)
            {
                using (var sw = File.AppendText(_saveLocation))
                {
                    sw.WriteLine(hand.Summary());
                    sw.Flush();
                }
            }
        }
    }
}