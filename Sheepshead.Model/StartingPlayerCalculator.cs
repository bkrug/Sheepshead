using Sheepshead.Models.Players;


namespace Sheepshead.Models
{
    public interface IStartingPlayerCalculator
    {
        IPlayer GetStartingPlayer(IHand hand, ITrick currentTrick);
    }

    public class StartingPlayerCalculator : IStartingPlayerCalculator
    {
        public IPlayer GetStartingPlayer(IHand hand, ITrick currentTrick)
        {
            var index = hand.Tricks.IndexOf(currentTrick);
            return (index == 0)
                ? hand.StartingPlayer
                : hand.Tricks[index - 1].Winner().Player;
        }
    }
}