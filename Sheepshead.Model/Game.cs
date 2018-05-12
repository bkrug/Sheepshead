using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Models.Players;
using Sheepshead.Models.Wrappers;
using Sheepshead.Model;

namespace Sheepshead.Models
{
    public class Game : IGame
    {
        public Guid Id { get; } = Guid.NewGuid();
        public const int CARDS_IN_DECK = 32;
        public int PlayerCount => _players.Count();
        public int Blind => CARDS_IN_DECK % _players.Count();
        public int HumanPlayerCount => _players.Count(p => p is IHumanPlayer);
        protected List<IPlayer> _players;
        public List<IPlayer> Players => _players.ToList();
        public List<IDeck> Decks => _gameStateDesciber.Decks;
        public IRandomWrapper _random { get; private set; }
        private IHandFactory _handFactory;
        private IGameStateDescriber _gameStateDesciber;
        public TurnType TurnType => _gameStateDesciber.GetTurnType();
        public TurnState TurnState => new TurnState
        {
            HumanPlayer = (IHumanPlayer)Players.First(p => p is IHumanPlayer),
            Deck = _gameStateDesciber.CurrentDeck,
            TurnType = TurnType,
            GameId = Id
        };

        public Game(List<IPlayer> players) : this(players, null, null, null)
        {

        }

        //TODO: Make this internal except to test project
        /// <summary>
        /// This constructor is for passing in Mocks in unit tests.
        /// </summary>
        public Game(List<IPlayer> players, IRandomWrapper random, IHandFactory handFactory, IGameStateDescriber gameStateDescriber)
        {
            _players = players;
            _random = random ?? new RandomWrapper();
            _handFactory = handFactory ?? new HandFactory();
            _gameStateDesciber = gameStateDescriber ?? new GameStateDescriber();
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
            return _gameStateDesciber.LastDeckIsComplete();
        }

        public IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick)
        {
            if (TurnType != TurnType.Pick)
                throw new WrongGamePhaseExcpetion("Game must be in the Pick phase.");
            var deck = _gameStateDesciber.CurrentDeck;
            return new PickProcessor().ContinueFromHumanPickTurn(human, willPick, deck, _handFactory, new PickProcessor());
        }

        public IComputerPlayer PlayNonHumanPickTurns(bool returnNullIfHumanNext = false)
        {
            if (returnNullIfHumanNext && _gameStateDesciber.CurrentDeck.PlayersWithoutPickTurn.FirstOrDefault() is IHumanPlayer)
                return null;
            if (TurnType != TurnType.Pick)
                throw new WrongGamePhaseExcpetion("Game must be in the Pick phase.");
            return new PickProcessor().PlayNonHumanPickTurns(_gameStateDesciber.CurrentDeck, _handFactory);
        }

        public void BuryCards(IHumanPlayer player, List<SheepCard> cards)
        {
            if (TurnType != TurnType.Bury)
                throw new WrongGamePhaseExcpetion("Game must be in the Bury phase.");
            new PickProcessor().BuryCards(Decks.Last(), player, cards);
        }

        public void PlayNonHumansInTrick()
        {
            var trick = _gameStateDesciber.CurrentTrick;
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

    public class HumanMove
    {
        public SheepCard? TrickCard { get; set; }
        public bool? WillPick { get; set; }
        public SheepCard[] BuryCards { get; set; }
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

    public interface IGame
    {
        Guid Id { get; }
        int HumanPlayerCount { get; }
        int PlayerCount { get; }
        List<IPlayer> Players { get; }
        List<IDeck> Decks { get; }
        TurnType TurnType { get; }
        TurnState TurnState { get; }
        void RearrangePlayers();
        bool LastDeckIsComplete();
        IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick);
        IComputerPlayer PlayNonHumanPickTurns(bool returnNullIfHumanNext = false);
        void BuryCards(IHumanPlayer player, List<SheepCard> cards);
        void PlayNonHumansInTrick();
        void RecordTurn(IHumanPlayer player, SheepCard card);
    }
}