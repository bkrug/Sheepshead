using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;
using Sheepshead.Models.Wrappers;


namespace Sheepshead.Models
{
    public class Game : LongId, IGame
    {
        public const int CARDS_IN_DECK = 32;
        public string Name { get; set; }
        public int PlayerCount => _players.Count();
        public int Blind => CARDS_IN_DECK % _players.Count();
        public int HumanPlayerCount => _players.Count(p => p is IHumanPlayer);
        protected List<IPlayer> _players;
        public List<IPlayer> Players => _players.ToList();
        public List<IDeck> Decks { get; } = new List<IDeck>();
        public IRandomWrapper _random { get; private set; }
        public IPlayer CurrentTurn { get { throw new NotImplementedException(); } }
        private IHandFactory _handFactory;
        public TurnType TurnType
        {
            get
            {
                var deck = Decks.LastOrDefault();
                if (!Decks.Any() || LastDeckIsComplete())
                    return TurnType.BeginDeck;
                else if (deck.Hand == null)
                    return TurnType.Pick;
                else if (!deck.Buried.Any() && !deck.Hand.Leasters)
                    return TurnType.Bury;
                else
                    return TurnType.PlayTrick;
            }
        } 

        public Game(long id, List<IPlayer> players, IRandomWrapper random, IHandFactory handFactory)
        {
            _players = players;
            _id = id;
            _random = random;
            _handFactory = handFactory;
        }

        public void RearrangePlayers()
        {
            for (var i = PlayerCount - 1; i > 0; --i)
            {
                var j = _random.Next(i);
                var swap = Players[i];
                Players[i] = Players[j];
                Players[j] = swap;
            }
        }

        public bool LastDeckIsComplete()
        {
            var lastDeck = Decks.LastOrDefault();
            return lastDeck == null || lastDeck.Hand != null && lastDeck.Hand.IsComplete();
        }

        public IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick)
        {
            if (TurnType != TurnType.Pick)
                throw new WrongGamePhaseExcpetion("Game must be in the Pick phase.");
            var deck = Decks.Last();
            return new PickProcessorOuter2().ContinueFromHumanPickTurn(human, willPick, deck, _handFactory, new PickProcessorOuter());
        }

        public IComputerPlayer PlayUpToHumanPickTurn()
        {
            if (Decks.Last().PlayersWithoutPickTurn.FirstOrDefault() is IComputerPlayer)
                return PlayNonHumanPickTurns();
            return null;
        }

        public IComputerPlayer PlayNonHumanPickTurns()
        {
            if (TurnType != TurnType.Pick)
                throw new WrongGamePhaseExcpetion("Game must be in the Pick phase.");
            return new PickProcessorOuter().PlayNonHumanPickTurns(Decks.Last(), _handFactory);
        }

        public void BuryCards(IHumanPlayer player, List<SheepCard> cards)
        {
            if (TurnType != TurnType.Bury)
                throw new WrongGamePhaseExcpetion("Game must be in the Bury phase.");
            new PickProcessorOuter().BuryCards(Decks.Last(), player, cards);
        }

        public void PlayNonHumansInTrick()
        {
            var trick = Decks.LastOrDefault()?.Hand.Tricks.LastOrDefault();
            if (trick == null || trick.IsComplete() && !Decks.LastOrDefault().Hand.IsComplete())
                trick = new Trick(Decks.LastOrDefault().Hand);
            foreach (var player in trick.PlayersWithoutTurn)
            {
                var computerPlayer = player as IComputerPlayer;
                if (computerPlayer == null)
                    return;
                var card = computerPlayer.GetMove(trick);
                trick.Add(computerPlayer, card);
            }
        }

        //TEST: That the move is recorded.
        //TEST: Throw an error if it is not this player's turn yet.
        //TEST: Throw an error if this is not the play trick phase.
        //TEST: Throw an error if the player doesn't have this card.
        public void RecordTurn(IHumanPlayer player, SheepCard card)
        {
            ITrick trick = Decks.Last().Hand.Tricks.Last();
            trick.Add(player, card);
        }
    }

    public class TooManyPlayersException : ApplicationException
    {
        public TooManyPlayersException(string message) : base(message) { }
    }

    public class TooManyHumanPlayersException : TooManyPlayersException
    {
        public TooManyHumanPlayersException(string message) : base(message) { }
    }

    public class ObjectInListException : ApplicationException
    {
        public ObjectInListException(string message) : base(message) { }
    }

    public class WrongGamePhaseExcpetion : ApplicationException
    {
        public WrongGamePhaseExcpetion(string message) : base(message) { }
    }

    public class NotPlayersTurnException : ApplicationException
    {
        public NotPlayersTurnException(string message) : base(message) { }
    }

    public interface IGame : ILongId
    {
        string Name { get; set; }
        int HumanPlayerCount { get; }
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        List<IDeck> Decks { get; }
        IPlayer CurrentTurn { get; }
        TurnType TurnType { get; }
        void RearrangePlayers();
        bool LastDeckIsComplete();
        IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick);
        IComputerPlayer PlayNonHumanPickTurns();
        IComputerPlayer PlayUpToHumanPickTurn();
        void BuryCards(IHumanPlayer player, List<SheepCard> cards);
        void PlayNonHumansInTrick();
        void RecordTurn(IHumanPlayer player, SheepCard card);
    }
}