using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blackjack.Shared;

namespace Blackjack.Client.Services
{
    public class MockGameHubService : IGameHubService  // ← ДОБАВЬ ЭТУ ЧАСТЬ
    {
        // Публичные свойства для привязки
        public List<Card> PlayerHand { get; set; } = new List<Card>();
        public List<Card> DealerHand { get; set; } = new List<Card>();
        public GameResult GameState { get; set; } = GameResult.InProgress;
        public bool IsMyTurn { get; set; } = true;
        public int DeckCount { get; set; } = 52;
        public string PlayerName { get; set; } = "Саня (тест)";

        // Случайные карты для теста
        private Random _random = new Random();

        public MockGameHubService()
        {
            // Начальные карты для демо
            PlayerHand = new List<Card>
            {
                new Card { Suit = Suit.Hearts, Rank = Rank.Ten, IsHidden = false },
                new Card { Suit = Suit.Diamonds, Rank = Rank.Seven, IsHidden = false }
            };

            DealerHand = new List<Card>
            {
                new Card { Suit = Suit.Clubs, Rank = Rank.King, IsHidden = false },
                new Card { Suit = Suit.Spades, Rank = Rank.Ace, IsHidden = true }
            };
        }

        // Методы-заглушки
        public async Task HitAsync()
        {
            // Добавляем случайную карту
            var newCard = GenerateRandomCard();
            PlayerHand.Add(newCard);

            // Имитируем задержку сети
            await Task.Delay(300);

            // Проверяем на перебор
            if (CalculateHandValue(PlayerHand) > 21)
            {
                GameState = GameResult.PlayerBust;
                IsMyTurn = false;
                // Открываем карты дилера
                foreach (var card in DealerHand) card.IsHidden = false;
            }

            Console.WriteLine($"Player hit. Cards: {PlayerHand.Count}, Value: {CalculateHandValue(PlayerHand)}");
        }

        public async Task StandAsync()
        {
            IsMyTurn = false;

            // Открываем скрытую карту дилера
            foreach (var card in DealerHand) card.IsHidden = false;

            // Дилер берет карты (упрощенная логика)
            while (CalculateHandValue(DealerHand) < 17)
            {
                await Task.Delay(500);
                DealerHand.Add(GenerateRandomCard());
            }

            // Определяем победителя
            int playerScore = CalculateHandValue(PlayerHand);
            int dealerScore = CalculateHandValue(DealerHand);

            if (playerScore > 21) GameState = GameResult.PlayerBust;
            else if (dealerScore > 21) GameState = GameResult.PlayerWin;
            else if (dealerScore > playerScore) GameState = GameResult.DealerWin;
            else if (playerScore > dealerScore) GameState = GameResult.PlayerWin;
            else GameState = GameResult.Push;

            Console.WriteLine($"Game over. Player: {playerScore}, Dealer: {dealerScore}, Result: {GameState}");
        }

        public async Task DoubleAsync()
        {
            // Упрощенное удвоение
            await HitAsync();
            if (GameState == GameResult.InProgress)
            {
                await StandAsync();
            }
        }

        public async Task StartNewGameAsync()
        {
            // Сбрасываем игру
            PlayerHand = new List<Card>
            {
                GenerateRandomCard(),
                GenerateRandomCard()
            };

            DealerHand = new List<Card>
            {
                GenerateRandomCard(),
                GenerateRandomCard()
            };
            DealerHand[1].IsHidden = true;

            GameState = GameResult.InProgress;
            IsMyTurn = true;
            DeckCount = 52;

            await Task.Delay(200);
            Console.WriteLine("New game started");
        }

        private Card GenerateRandomCard()
        {
            var suits = Enum.GetValues(typeof(Suit));
            var ranks = Enum.GetValues(typeof(Rank));

            return new Card
            {
                Suit = (Suit)suits.GetValue(_random.Next(suits.Length)),
                Rank = (Rank)ranks.GetValue(_random.Next(ranks.Length)),
                IsHidden = false
            };
        }

        private int CalculateHandValue(List<Card> hand)
        {
            int value = 0;
            int aces = 0;

            foreach (var card in hand)
            {
                if (card.IsHidden) continue;

                if (card.Rank == Rank.Ace) aces++;
                value += card.Value;
            }

            // Корректировка тузов
            while (value > 21 && aces > 0)
            {
                value -= 10;
                aces--;
            }

            return value;
        }
    }
}