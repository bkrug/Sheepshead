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
        public int PlayerCount => Players.Count();
        public int TrickCount => (int)Math.Floor(32d / PlayerCount);
        public int Blind => CARDS_IN_DECK % Players.Count();
        public int HumanPlayerCount => Players.Count(p => p is IHumanPlayer);
        public List<IPlayer> Players { get; }
        public List<IHumanPlayer> UnassignedPlayers => Players.OfType<IHumanPlayer>().Where(p => !p.AssignedToClient).ToList();
        public List<IDeck> Decks => _gameStateDesciber.Decks;
        public IRandomWrapper _random { get; private set; }
        private IHandFactory _handFactory;
        private IGameStateDescriber _gameStateDesciber;
        public TurnType TurnType => _gameStateDesciber.GetTurnType();
        public TurnState TurnState => new TurnState
        {
            GameId = Id,
            Deck = _gameStateDesciber.CurrentDeck,
            TurnType = TurnType
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
            Players = players;
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
        public void RecordTurn(IHumanPlayer player, SheepCard card)
        {
            var trick = _gameStateDesciber.CurrentTrick;
            if (trick.PlayersWithoutTurn.FirstOrDefault() != player)
                throw new NotPlayersTurnException($"This is not the turn for the player: {player.Name}");
            if (!player.Cards.Contains(card))
                throw new ArgumentException("Player does not have this card", "card");
            if (!trick.IsLegalAddition(card, player))
                throw new ArgumentException("This move is not legal", "card");
            trick.Add(player, card);
        }

        public void MaybeGiveComputerPlayersNames()
        {
            if (UnassignedPlayers.Any())
                return;
            var rnd = new Random();
            var unusedNames = potentialNames.Except(Players.OfType<IHumanPlayer>().Select(h => h.Name)).ToList();
            foreach(var player in Players.OfType<IComputerPlayer>())
            {
                var nameIndex = rnd.Next(unusedNames.Count - 1);
                var nameToUse = unusedNames.ElementAt(nameIndex);
                unusedNames.RemoveAt(nameIndex);
                player.Name = nameToUse;
            }   
        }

        private List<string> potentialNames = new List<string>()
        {
            "Ben", "Sam", "Johann", "Fritz", "Sarah", "Rachel", "Liz", "Vivek", "Jing", "Junior", "Fido", "Peter", "Katrina", "Akinpelu"
        };

        public PlayState PlayState(Guid requestingPlayerId)
        {
            var turnType = TurnType;
            var currentDeck = _gameStateDesciber.CurrentDeck;
            var currentTrick = currentDeck?.Hand != null ? _gameStateDesciber.CurrentTrick : null;
            var currentPlayer =
                turnType == TurnType.PlayTrick 
                ? currentTrick?.PlayersWithoutTurn?.FirstOrDefault()
                : null;
            var tricks = this.Decks.Where(d => d.Hand != null).LastOrDefault()?.Hand?.Tricks ?? new List<ITrick>();
            var humanPlayer = currentPlayer as IHumanPlayer;
            var requestingPlayer = Players.OfType<IHumanPlayer>().SingleOrDefault(p => p.Id == requestingPlayerId);
            return new PlayState
            {
                TurnType = turnType.ToString(),
                HumanTurn = humanPlayer != null,
                RequestingPlayerTurn = humanPlayer?.Id == requestingPlayerId,
                CardsPlayed = tricks.Select(t => t.CardsPlayed
                                                  .Select(cp => new Tuple<string, CardSummary>(cp.Key.Name, CardUtil.GetCardSummary(cp.Value)))
                                                  .ToList()
                                           )?.ToList(),
                PlayerCards = requestingPlayer?.Cards?.Select(c => CardUtil.GetCardSummary(c, currentTrick?.IsLegalAddition(c, requestingPlayer)))?.ToList()
            };
        }

        public PlayState PickState(Guid requestingPlayerId)
        {
            var turnType = TurnType;
            var currentDeck = _gameStateDesciber.CurrentDeck;
            var currentPlayer = 
                turnType == TurnType.Pick 
                ? currentDeck?.PlayersWithoutPickTurn?.FirstOrDefault() 
                : null;
            var humanPlayer = currentPlayer as IHumanPlayer;
            var requestingPlayer = Players.OfType<IHumanPlayer>().SingleOrDefault(p => p.Id == requestingPlayerId);
            return new PlayState
            {
                TurnType = turnType.ToString(),
                RequestingPlayerTurn = humanPlayer?.Id == requestingPlayerId,
                PickChoices =
                    currentDeck?.PlayersRefusingPick.Select(p => new Tuple<string, bool>(p.Name, false))
                    .Union(new List<Tuple<string, bool>> { new Tuple<string, bool>(currentDeck?.Hand?.Picker?.Name, true) })
                    .Where(p => p.Item1 != null)
                    .ToList()
                    ?? new List<Tuple<string, bool>>(),
                PlayerCards = requestingPlayer?.Cards?.Select(c => CardUtil.GetCardSummary(c))?.ToList(),
                HumanTurn = humanPlayer != null,
                Blinds = new List<CardSummary>(),
                CardsPlayed = new List<List<Tuple<string, CardSummary>>>()
            };
        }

        public PlayState BuryState(Guid requestingPlayerId)
        {
            var turnType = TurnType;
            var currentDeck = _gameStateDesciber.CurrentDeck;
            var currentPlayer =
                turnType == TurnType.Bury 
                ? currentDeck?.Hand?.Picker
                : null;
            var humanPlayer = currentPlayer as IHumanPlayer;
            var requestingPlayer = Players.OfType<IHumanPlayer>().SingleOrDefault(p => p.Id == requestingPlayerId);
            return new PlayState
            {
                TurnType = turnType.ToString(),
                RequestingPlayerTurn = humanPlayer?.Id == requestingPlayerId,
                Blinds = turnType == TurnType.Bury ? currentDeck?.Blinds?.Select(b => CardUtil.GetCardSummary(b))?.ToList() : null,
                PlayerCards = requestingPlayer?.Cards?.Select(c => CardUtil.GetCardSummary(c))?.ToList()
            };
        }

        public TrickResults GetTrickWinners()
        {
            var currentDeck = Decks.LastOrDefault(d => d.Hand != null);
            var tricks = currentDeck?.Hand?.Tricks ?? new List<ITrick>();
            var winners = tricks.Where(t => t.PlayersWithoutTurn.Count == 0).Select(t => t?.Winner()?.Player?.Name).ToList();
            return new TrickResults()
            {
                Picker = currentDeck?.Hand?.Picker?.Name,
                Partner = currentDeck?.Hand?.Partner?.Name,
                TrickWinners = winners
            };
        }
    }

    public class HumanMove
    {
        public SheepCard? TrickCard { get; set; }
        public bool? WillPick { get; set; }
        public SheepCard[] BuryCards { get; set; }
    }

    public class TrickResults
    {
        public string Picker { get; set; }
        public string Partner { get; set; }
        public List<string> TrickWinners { get; set; }
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
        int TrickCount { get; }
        List<IPlayer> Players { get; }
        List<IHumanPlayer> UnassignedPlayers { get; }
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
        void MaybeGiveComputerPlayersNames();
        PlayState PlayState(Guid requestingPlayerId);
        PlayState PickState(Guid requestingPlayerId);
        PlayState BuryState(Guid requestingPlayerId);
        TrickResults GetTrickWinners();
    }
}