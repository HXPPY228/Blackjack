using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blackjack.Client.Services;
using Blackjack.Shared;

namespace Blackjack.Server.Services
{
    public class ServerMockGameHubService : IGameHubService
    {
        public List<Card> PlayerHand { get; set; } = new();
        public List<Card> DealerHand { get; set; } = new();
        public GameResult GameState { get; set; } = GameResult.InProgress;
        public bool IsMyTurn { get; set; } = true;
        public int DeckCount { get; set; } = 52;
        public string PlayerName { get; set; } = "Саня (серверный тест)";

        private Random _random = new();

        public ServerMockGameHubService()
        {
            InitializeTestGame();
        }

        private void InitializeTestGame()
        {
            PlayerHand = new List<Card>
            {
                new() { Suit = Suit.Hearts, Rank = Rank.Ten },
                new() { Suit = Suit.Diamonds, Rank = Rank.Four }
            };

            DealerHand = new List<Card>
            {
                new() { Suit = Suit.Clubs, Rank = Rank.King },
                new() { Suit = Suit.Spades, Rank = Rank.Ace, IsHidden = true }
            };
        }

        public async Task HitAsync()
        {
            // Добавляем случайную карту игроку
            var suits = Enum.GetValues<Suit>();
            var ranks = Enum.GetValues<Rank>();

            var newCard = new Card
            {
                Suit = suits.GetValue(_random.Next(suits.Length)) as Suit? ?? Suit.Hearts,
                Rank = ranks.GetValue(_random.Next(ranks.Length)) as Rank? ?? Rank.Two
            };

            PlayerHand.Add(newCard);

            await Task.Delay(100);
        }

        public async Task StandAsync()
        {
            // Открываем карту дилера
            if (DealerHand.Count > 1)
                DealerHand[1].IsHidden = false;

            await Task.Delay(100);
        }

        public async Task DoubleAsync()
        {
            await HitAsync();
            await Task.Delay(100);
        }

        public async Task StartNewGameAsync()
        {
            PlayerHand.Clear();
            DealerHand.Clear();

            InitializeTestGame();

            await Task.Delay(100);
        }
    }
}