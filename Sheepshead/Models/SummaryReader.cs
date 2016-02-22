using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sheepshead.Models.Players;

namespace Sheepshead.Models
{
    public class SummaryReader
    {
        private class FakeGame : IGame
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public int HumanPlayerCount { get { throw new NotImplementedException(); } }
            public int PlayerCount { get; set; }
            public List<IPlayer> Players { get; set; }
            public List<IDeck> Decks { get; set; }
            public void PlayNonHumans(ITrick trick) { throw new NotImplementedException(); }
            public IPlayer PlayNonHumanPickTurns(IDeck deck) { throw new NotImplementedException(); }
            public void RearrangePlayers() { throw new NotImplementedException(); }
            public bool LastDeckIsComplete() { throw new NotImplementedException(); }
            public TurnType TurnType { get { throw new NotImplementedException(); } }
            public IPlayer CurrentTurn { get { throw new NotImplementedException(); } }
            public IHand ContinueFromHumanPickTurn(IHumanPlayer player, bool willPick) { throw new NotImplementedException(); }
            public IHand AcceptComputerPicker(IComputerPlayer player) { throw new NotImplementedException(); }
            public void BuryCards(IHumanPlayer player, List<ICard> cards) { throw new NotImplementedException(); }
            public void RecordTurn(IHumanPlayer player, ICard card) { throw new NotImplementedException(); }
        }

        private class FakeDeck : IDeck
        {
            public FakeDeck(IGame game) { Game = game; }
            public List<ICard> Blinds { get; set; }
            public List<ICard> Buried { get; set; }
            public IGame Game { get; private set; }
            public IHand Hand { get; set; }
            public List<IPlayer> PlayersRefusingPick { get { throw new NotImplementedException(); } }
            public void PlayerWontPick(IPlayer player) { throw new NotImplementedException(); }
            public IPlayer StartingPlayer { get; set; }
            public int PlayerCount { get { return Game.PlayerCount; } }
            public List<IPlayer> Players { get { return Game.Players; } }
            public List<IPlayer> PlayersWithoutPickTurn { get { return Game.Players; } }
        }

        public static IHand FromSummary(string summary)
        {
            var pieces = summary.Split(',');
            var blindSummary = pieces[0];
            var buriedSummary = pieces[1];
            var trickSummaries = pieces.Skip(2).ToList();
            var playerCount = trickSummaries[0].Length / 2;

            var playerList = GetPlayerList(playerCount);
            GivePlayersCards(trickSummaries, playerList);
            var buriedList = String.IsNullOrEmpty(buriedSummary)
                ? null
                : new List<ICard>() { GetCard(buriedSummary.Substring(1, 2)), GetCard(buriedSummary.Substring(3, 2)) };

            var game = GetGame(playerCount, playerList);
            var deck = GetDeck(blindSummary, playerList, game, buriedList);
            var hand = GetHand(buriedSummary, playerList, buriedList, deck);
            BuildTrickList(trickSummaries, playerList, hand);

            return hand;
        }

        private static List<IPlayer> GetPlayerList(int playerCount)
        {
            var playerList = new List<IPlayer>();
            for (var i = 0; i < playerCount; ++i)
                playerList.Add(new NewbiePlayer()); //It doesn't matter which player subclass we use.
            return playerList;
        }

        private static ICard GetCard(string abbr)
        {
            var typeLetter = abbr.Substring(0, 1);
            var suiteLetter = abbr.Substring(1);
            var type = CardRepository.ReverseCardTypeLetter[typeLetter];
            var suite = CardRepository.ReverseSuiteLetter[suiteLetter];
            return CardRepository.Instance[suite, type];
        }

        private static FakeGame GetGame(int playerCount, List<IPlayer> playerList)
        {
            var game = new FakeGame()
            {
                PlayerCount = playerCount,
                Players = playerList,
                Decks = new List<IDeck>()
            };
            return game;
        }

        private static FakeDeck GetDeck(string blindSummary, List<IPlayer> playerList, FakeGame game, List<ICard> buriedList)
        {
            var deck = new FakeDeck(game)
            {
                Blinds = new List<ICard>() {
                    GetCard(blindSummary.Substring(0, 2)), GetCard(blindSummary.Substring(2, 2))
                },
                Buried = buriedList,
                StartingPlayer = playerList.First()
            };
            return deck;
        }

        private static Hand GetHand(string buriedSummary, List<IPlayer> playerList, List<ICard> buriedList, FakeDeck deck)
        {
            var pickerNo = buriedSummary.Length > 0 ? (int?)int.Parse(buriedSummary.Substring(0, 1)) - 1 : null;
            var picker = pickerNo == null ? (IPlayer)null : playerList[pickerNo.Value];
            var hand = new Hand(deck, picker, buriedList);
            return hand;
        }

        private static void GivePlayersCards(List<string> trickSummaries, List<IPlayer> playerList)
        {
            foreach (var trickSummary in trickSummaries)
            {
                for (var p = 0; p < playerList.Count; ++p)
                {
                    var card = GetCard(trickSummary.Substring(p * 2, 2));
                    if (!playerList[p].Cards.Contains(card))
                        playerList[p].Cards.Add(card);
                }
            }
        }

        private static void BuildTrickList(List<string> trickSummaries, List<IPlayer> playerList, Hand hand)
        {
            foreach (var trickSummary in trickSummaries)
            {
                var trick = new Trick(hand);
                for (var p = 0; p < hand.PlayerCount; ++p)
                {
                    var card = GetCard(trickSummary.Substring(p * 2, 2));
                    trick.Add(playerList[p], card);
                }
            }
        }
    }
}