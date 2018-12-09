using System;
using System.Collections.Generic;
using System.Linq;
using Sheepshead.Model.Players;
using Sheepshead.Model.Wrappers;

namespace Sheepshead.Model
{
    public class Game : IGame
    {
        public Guid Id { get; }
        public const int CARDS_IN_DECK = 32;
        public int PlayerCount => Players.Count();
        public int TrickCount => (int)Math.Floor(32d / PlayerCount);
        public int HumanPlayerCount => Players.Count(p => p is IHumanPlayer);
        public bool LeastersEnabled { get; }
        public List<IPlayer> Players { get; }
        public List<IHumanPlayer> UnassignedPlayers => Players.OfType<IHumanPlayer>().Where(p => !p.AssignedToClient).ToList();
        public List<IHand> Hands => _gameStateDesciber.Hands;
        public IRandomWrapper _random { get; private set; }
        private IGameStateDescriber _gameStateDesciber;
        public TurnType TurnType => _gameStateDesciber.GetTurnType();
        public PartnerMethod PartnerMethod { get; }
        public TurnState TurnState => new TurnState
        {
            GameId = Id,
            Hand = _gameStateDesciber.CurrentHand,
            TurnType = TurnType
        };

        public Game(List<IPlayer> players, PartnerMethod partnerMethod, bool enableLeasters) : this(players, partnerMethod, null, null)
        {
            LeastersEnabled = enableLeasters;
            Id = Guid.NewGuid();
        }

        //TODO: Make this internal except to test project
        /// <summary>
        /// This constructor is for passing in Mocks in unit tests.
        /// </summary>
        public Game(List<IPlayer> players, PartnerMethod partnerMethod, IRandomWrapper random, IGameStateDescriber gameStateDescriber)
        {
            Players = players;
            PartnerMethod = partnerMethod;
            _random = random ?? new RandomWrapper();
            _gameStateDesciber = gameStateDescriber ?? new GameStateDescriber();
            Id = Guid.NewGuid();
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

        public bool LastHandIsComplete()
        {
            return _gameStateDesciber.LastHandIsComplete();
        }

        public IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick)
        {
            if (TurnType != TurnType.Pick)
                throw new WrongGamePhaseExcpetion("Game must be in the Pick phase.");
            var hand = _gameStateDesciber.CurrentHand;
            return new PickProcessor().ContinueFromHumanPickTurn(human, willPick, hand, new PickProcessor());
        }

        public IComputerPlayer PlayNonHumanPickTurns(bool returnNullIfHumanNext = false)
        {
            if (returnNullIfHumanNext && _gameStateDesciber.CurrentHand.PlayersWithoutPickTurn.FirstOrDefault() is IHumanPlayer)
                return null;
            if (TurnType != TurnType.Pick)
                throw new WrongGamePhaseExcpetion("Game must be in the Pick phase.");
            return new PickProcessor().PlayNonHumanPickTurns(_gameStateDesciber.CurrentHand);
        }

        public void BuryCards(IHumanPlayer player, List<SheepCard> cards, bool goItAlone, SheepCard? partnerCard)
        {
            if (TurnType != TurnType.Bury)
                throw new WrongGamePhaseExcpetion("Game must be in the Bury phase.");
            if (PartnerMethod == PartnerMethod.JackOfDiamonds || !partnerCard.HasValue)
                new PickProcessor().BuryCards(Hands.Last(), player, cards, goItAlone);
            else
                new PickProcessor().BuryCards(Hands.Last(), player, cards, goItAlone, partnerCard.Value);
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
            var currentDeck = _gameStateDesciber.CurrentHand;
            var currentTrick = currentDeck?.PickPhaseComplete == true ? _gameStateDesciber.CurrentTrick : null;
            var currentPlayer =
                turnType == TurnType.PlayTrick 
                ? currentTrick?.PlayersWithoutTurn?.FirstOrDefault()
                : null;
            var tricks = Hands.LastOrDefault(d => d.PickPhaseComplete)?.Tricks ?? new List<ITrick>();
            var humanPlayer = currentPlayer as IHumanPlayer;
            var requestingPlayer = Players.OfType<IHumanPlayer>().SingleOrDefault(p => p.Id == requestingPlayerId);
            return new PlayState
            {
                TurnType = turnType.ToString(),
                HumanTurn = humanPlayer != null,
                CurrentTurn = currentPlayer?.Name,
                RequestingPlayerTurn = humanPlayer?.Id == requestingPlayerId,
                CardsPlayed = tricks.Select(t => t.CardsPlayed
                                                  .Select(cp => new Tuple<string, CardSummary>(cp.Key.Name, CardUtil.GetCardSummary(cp.Value)))
                                                  .ToList()
                                           )?.ToList(),
                PlayerCards = requestingPlayer?.Cards?.Select(c => CardUtil.GetCardSummary(c, currentTrick?.IsLegalAddition(c, requestingPlayer)))?.ToList()
            };
        }

        public PickState PickState(Guid requestingPlayerId)
        {
            var turnType = TurnType;
            var currentDeck = Hands.LastOrDefault();
            var currentPlayer = 
                turnType == TurnType.Pick 
                ? currentDeck?.PlayersWithoutPickTurn?.FirstOrDefault() 
                : null;
            var humanPlayer = currentPlayer as IHumanPlayer;
            var requestingPlayer = Players.OfType<IHumanPlayer>().SingleOrDefault(p => p.Id == requestingPlayerId);
            return new PickState
            {
                TurnType = turnType.ToString(),
                RequestingPlayerTurn = humanPlayer?.Id == requestingPlayerId,
                PickChoices =
                    currentDeck?.PlayersRefusingPick.Select(p => new Tuple<string, bool>(p.Name, false))
                    .Union(new List<Tuple<string, bool>> { new Tuple<string, bool>(currentDeck?.Picker?.Name, true) })
                    .Where(p => p.Item1 != null)
                    .ToList()
                    ?? new List<Tuple<string, bool>>(),
                PlayerCards = requestingPlayer?.Cards?.Select(c => CardUtil.GetCardSummary(c))?.ToList(),
                HumanTurn = humanPlayer != null,
                CurrentTurn = currentPlayer?.Name,
                MustRedeal = currentDeck?.MustRedeal ?? false
            };
        }

        public BuryState BuryState(Guid requestingPlayerId)
        {
            var turnType = TurnType;
            var currentDeck = _gameStateDesciber.CurrentHand;
            var currentPlayer =
                turnType == TurnType.Bury 
                ? currentDeck?.Picker
                : null;
            var humanPlayer = currentPlayer as IHumanPlayer;
            var requestingPlayer = Players.OfType<IHumanPlayer>().SingleOrDefault(p => p.Id == requestingPlayerId);
            return new BuryState
            {
                TurnType = turnType.ToString(),
                RequestingPlayerTurn = humanPlayer?.Id == requestingPlayerId,
                Blinds = turnType == TurnType.Bury ? currentDeck?.Blinds?.Select(b => CardUtil.GetCardSummary(b))?.ToList() : null,
                PlayerCards = requestingPlayer?.Cards?.Select(c => CardUtil.GetCardSummary(c))?.ToList(),
                PartnerMethod = PartnerMethod.ToString(),
                LegalCalledAces = humanPlayer?.LegalCalledAces(currentDeck).Select(c => CardUtil.GetCardSummary(c)).ToList()
            };
        }

        public TrickResults GetTrickWinners()
        {
            var currentDeck = Hands.LastOrDefault(d => d.PickPhaseComplete);
            var tricks = currentDeck?.Tricks ?? new List<ITrick>();
            var winners = tricks.Where(t => t.PlayersWithoutTurn.Count == 0).Select(t => t?.Winner()?.Player?.Name).ToList();
            return new TrickResults()
            {
                Picker = currentDeck?.Picker?.Name,
                Partner = currentDeck?.Partner?.Name,
                PartnerCard = currentDeck?.PartnerCard == null ? null : CardUtil.ToAbbr(currentDeck.PartnerCard.Value),
                TrickWinners = winners
            };
        }

        public List<GameCoins> GameCoins()
        {
            var coins = Hands
                .Select(d => d.Scores()?.Coins)
                .Where(c => c != null)
                .ToList();
            return Players
                .Select(p => new GameCoins() {
                    Name = p.Name,
                    Coins = coins.Sum(c => c[p])
                })
                .ToList();
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
        public string PartnerCard { get; set; }
        public List<string> TrickWinners { get; set; }
    }

    public class GameCoins
    {
        public string Name { get; set; }
        public int Coins { get; set; }
    }

    public enum PartnerMethod
    {
        JackOfDiamonds, CalledAce
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
        bool LeastersEnabled { get; }
        List<IPlayer> Players { get; }
        List<IHumanPlayer> UnassignedPlayers { get; }
        List<IHand> Hands { get; }
        TurnType TurnType { get; }
        TurnState TurnState { get; }
        PartnerMethod PartnerMethod { get; }
        void RearrangePlayers();
        bool LastHandIsComplete();
        IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick);
        IComputerPlayer PlayNonHumanPickTurns(bool returnNullIfHumanNext = false);
        void BuryCards(IHumanPlayer player, List<SheepCard> cards, bool goItAlone, SheepCard? parnterCard);
        void PlayNonHumansInTrick();
        void RecordTurn(IHumanPlayer player, SheepCard card);
        void MaybeGiveComputerPlayersNames();
        List<GameCoins> GameCoins();
        PlayState PlayState(Guid requestingPlayerId);
        PickState PickState(Guid requestingPlayerId);
        BuryState BuryState(Guid requestingPlayerId);
        TrickResults GetTrickWinners();
    }
}