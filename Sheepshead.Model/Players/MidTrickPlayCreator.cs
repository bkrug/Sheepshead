namespace Sheepshead.Models.Players
{
    public interface IMidTrickPlayCreator
    {
        SheepCard GiveAwayPoints(IPlayer thisPlayer, ITrick trick);
        SheepCard PlayToWin(IPlayer thisPlayer, ITrick trick);
        SheepCard GiveAwayLeastPowerLeastPoints(IPlayer thisPlayer, ITrick trick);
    }

    public class MidTrickPlayCreator : IMidTrickPlayCreator
    {
        public SheepCard GiveAwayLeastPowerLeastPoints(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public SheepCard GiveAwayPoints(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }

        public SheepCard PlayToWin(IPlayer thisPlayer, ITrick trick)
        {
            throw new System.NotImplementedException();
        }
    }
}