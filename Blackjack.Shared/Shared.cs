using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Shared
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }

    public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public class Card
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public bool IsHidden { get; set; }

        public string ImageName => $"{Rank}_{Suit}.png";

        public int Value
        {
            get
            {
                if ((int)Rank <= 10) return (int)Rank;
                if (Rank == Rank.Ace) return 11;
                return 10; // J, Q, K
            }
        }
    }
    public enum GameResult { InProgress, PlayerWin, DealerWin, Push, PlayerBust }
}