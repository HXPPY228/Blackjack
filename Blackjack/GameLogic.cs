namespace Blackjack.Server.Logic
{
    using Blackjack.Shared;

    public class BlackjackGameLogic
    {
        public List<Card> Deck { get; private set; }
        public List<Card> PlayerHand { get; private set; }
        public List<Card> DealerHand { get; private set; }
        public GameResult CurrentState { get; private set; }

        public BlackjackGameLogic()
        {
            StartNewGame();
        }

        // 1. Генерация и перемешивание колоды
        public void StartNewGame()
        {
            Deck = new List<Card>();
            PlayerHand = new List<Card>();
            DealerHand = new List<Card>();
            CurrentState = GameResult.InProgress;

            // Создаем 52 карты
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    Deck.Add(new Card { Suit = suit, Rank = rank });
                }
            }

            ShuffleDeck();
            DealInitialCards();
        }

        // Алгоритм тасования (Фишера-Йейтса)
        private void ShuffleDeck()
        {
            Random rng = new Random();
            int n = Deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = Deck[k];
                Deck[k] = Deck[n];
                Deck[n] = value;
            }
        }

        // Раздача начальных карт
        private void DealInitialCards()
        {
            // Игрок получает 2 открытые
            PlayerHand.Add(DrawCard());
            PlayerHand.Add(DrawCard());

            // Дилер получает 1 открытую и 1 скрытую
            DealerHand.Add(DrawCard());
            var hiddenCard = DrawCard();
            hiddenCard.IsHidden = true;
            DealerHand.Add(hiddenCard);

            CheckBlackjack();
        }

        private Card DrawCard()
        {
            if (Deck.Count == 0) return null; // Или пересоздать колоду
            var card = Deck[0];
            Deck.RemoveAt(0);
            return card;
        }

        // 2. Главная математика: Подсчет очков с учетом Туза (1 или 11)
        public int CalculateScore(List<Card> hand)
        {
            int score = 0;
            int aceCount = 0;

            foreach (var card in hand)
            {
                if (card.IsHidden) continue; // Скрытая карта не считается пока
                score += card.Value;
                if (card.Rank == Rank.Ace) aceCount++;
            }

            // Если перебор, превращаем Тузы из 11 в 1
            while (score > 21 && aceCount > 0)
            {
                score -= 10;
                aceCount--;
            }

            return score;
        }

        // Мгновенная проверка на Блэкджек (21 с двух карт)
        private void CheckBlackjack()
        {
            int pScore = CalculateScore(PlayerHand);
            int dScore = CalculateScore(DealerHand);

            if (pScore == 21)
            {
                // Открываем карту дилера
                DealerHand.ForEach(c => c.IsHidden = false);
                dScore = CalculateScore(DealerHand);

                if (dScore == 21) CurrentState = GameResult.Push; // Ничья
                else CurrentState = GameResult.PlayerWin;
            }
        }

        // 3. Действия игрока

        // Взять карту (Hit)
        public void Hit()
        {
            if (CurrentState != GameResult.InProgress) return;

            PlayerHand.Add(DrawCard());

            if (CalculateScore(PlayerHand) > 21)
            {
                CurrentState = GameResult.PlayerBust; // Проиграл перебором
                DealerHand.ForEach(c => c.IsHidden = false); // Показываем карты дилера
            }
        }

        // Остановиться (Stand) - ход переходит к дилеру
        public void Stand()
        {
            if (CurrentState != GameResult.InProgress) return;

            // Открываем скрытую карту дилера
            DealerHand.ForEach(c => c.IsHidden = false);

            // Логика дилера: брать, пока меньше 17
            while (CalculateScore(DealerHand) < 17)
            {
                DealerHand.Add(DrawCard());
            }

            DetermineWinner();
        }

        private void DetermineWinner()
        {
            int pScore = CalculateScore(PlayerHand);
            int dScore = CalculateScore(DealerHand);

            if (dScore > 21) CurrentState = GameResult.PlayerWin; // Дилер перебрал
            else if (dScore > pScore) CurrentState = GameResult.DealerWin;
            else if (pScore > dScore) CurrentState = GameResult.PlayerWin;
            else CurrentState = GameResult.Push;
        }
    }
}