using Sheepshead.Logic.Models;
using Sheepshead.Logic.Players;

using Sheepshead.Logic; namespace Sheepshead.Logic
{
    public interface IStartingPlayerCalculator
    {
        IPlayer GetStartingPlayer(IHand hand, ITrick currentTrick);
    }

    public class StartingPlayerCalculator : IStartingPlayerCalculator
    {
        public IPlayer GetStartingPlayer(IHand hand, ITrick currentTrick)
        {
            var index = hand.ITricks.IndexOf(currentTrick);
            return (index == 0)
                ? hand.StartingPlayer
                : hand.ITricks[index - 1].Winner().Player;
        }
    }
}