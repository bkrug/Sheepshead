using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Sheepshead.Logic.Players;

namespace Sheepshead.Logic.Models
{
    public partial class Game : IGame
    {
        public IReadOnlyList<IHand> IHands => Hands?.OrderBy(h => h.SortOrder).ToList();
        private List<IPlayer> _mockPlayerList = null;
        public virtual List<IPlayer> Players => _mockPlayerList ?? Participants.OrderBy(p => p.SortOrder).Select(p => p.Player).ToList();
        [NotMapped]
        public PartnerMethod PartnerMethodEnum
        {
            get { return PartnerMethod == "A" ? Models.PartnerMethod.CalledAce : Models.PartnerMethod.JackOfDiamonds; }
            private set { PartnerMethod = value == Models.PartnerMethod.CalledAce ? "A" : "J"; }
        }

        public const int CARDS_IN_DECK = 32;
        public virtual int PlayerCount => Players.Count();
        public int TrickCount => PlayerCount == 5 ? 6 : 10;
        public int HumanPlayerCount => Players.Count(p => p is IHumanPlayer);
        public List<IHumanPlayer> UnassignedPlayers => Players.OfType<IHumanPlayer>().Where(p => !p.AssignedToClient).ToList();

        [NotMapped]
        public IRandomWrapper _random { get; private set; } = new RandomWrapper();
        private IGameStateDescriber _gameStateDesciber1;
        private IGameStateDescriber _gameStateDesciber => _gameStateDesciber1 ?? (_gameStateDesciber1 = new GameStateDescriber(this));
        public TurnType TurnType => _gameStateDesciber.GetTurnType();
        public TurnState TurnState => new TurnState
        {
            GameId = Id,
            Hand = _gameStateDesciber.CurrentHand,
            TurnType = TurnType
        };

        public Game(List<Participant> participants, PartnerMethod partnerMethod, bool enableLeasters, IRandomWrapper random = null, IGameStateDescriber gameStateDescriber = null) : this()
        {
            Hands = Hands ?? new List<Hand>();
            LeastersEnabled = enableLeasters;
            Id = Guid.NewGuid();
            Participants = participants;
            PartnerMethodEnum = partnerMethod;
            _random = random ?? _random;
            _gameStateDesciber1 = gameStateDescriber ?? _gameStateDesciber1;
        }

        //TODO: Make this internal except to test project
        /// <summary>
        /// This constructor is for passing in Mocks in unit tests.
        /// </summary>
        public Game(List<IPlayer> mockPlayers, PartnerMethod partnerMethod, bool enableLeasters) : this()
        {
            Hands = Hands ?? new List<Hand>();
            LeastersEnabled = enableLeasters;
            Id = Guid.NewGuid();
            _mockPlayerList = mockPlayers;
            PartnerMethodEnum = partnerMethod;
        }

        public void RearrangePlayers()
        {
            var sortOrder = 0;
            foreach (var player in Players.ToList())
                player.Participant.SortOrder = ++sortOrder;

            for (var i = PlayerCount - 1; i > 0; --i)
            {
                var j = _random.Next(i);
                var participantI = Players[i].Participant;
                var particiapntJ = Players[j].Participant;
                var swap = participantI.SortOrder;
                participantI.SortOrder = particiapntJ.SortOrder;
                particiapntJ.SortOrder = swap;
            }
        }

        public virtual bool LastHandIsComplete()
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
            if (PartnerMethodEnum == Models.PartnerMethod.JackOfDiamonds || !partnerCard.HasValue)
                new PickProcessor().BuryCards(IHands.Last(), player, cards, goItAlone);
            else
                new PickProcessor().BuryCards(IHands.Last(), player, cards, goItAlone, partnerCard.Value);
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

        //TODO: Move this to the Hand class?
        public TrickResults GetTrickWinners()
        {
            var currentDeck = IHands.LastOrDefault(d => d.PickPhaseComplete);
            var tricks = currentDeck?.ITricks ?? new List<ITrick>();
            var winners = tricks.Where(t => t.PlayersWithoutTurn.Count == 0).Select(t => t?.Winner()?.Player?.Name).ToList();
            return new TrickResults()
            {
                Picker = currentDeck?.Picker?.Name,
                Partner = currentDeck?.Partner?.Name,
                PartnerCard = currentDeck?.PartnerCardEnum == null ? null : CardUtil.GetAbbreviation(currentDeck.PartnerCardEnum.Value),
                TrickWinners = winners
            };
        }

        public List<GameCoins> GameCoins()
        {
            var coins = IHands
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
        bool LeastersEnabled { get; }
        List<IPlayer> Players { get; }
        List<IHumanPlayer> UnassignedPlayers { get; }
        ICollection<Hand> Hands { get; }
        IReadOnlyList<IHand> IHands { get; }
        PartnerMethod PartnerMethodEnum { get; }

        int HumanPlayerCount { get; }
        int PlayerCount { get; }
        int TrickCount { get; }
        TurnType TurnType { get; }
        TurnState TurnState { get; }
        void RearrangePlayers();

        bool LastHandIsComplete();
        TrickResults GetTrickWinners();
        List<GameCoins> GameCoins();

        void MaybeGiveComputerPlayersNames();
        IHand ContinueFromHumanPickTurn(IHumanPlayer human, bool willPick);
        IComputerPlayer PlayNonHumanPickTurns(bool returnNullIfHumanNext = false);
        void BuryCards(IHumanPlayer player, List<SheepCard> cards, bool goItAlone, SheepCard? parnterCard);
        void PlayNonHumansInTrick();
        void RecordTurn(IHumanPlayer player, SheepCard card);
    }
}