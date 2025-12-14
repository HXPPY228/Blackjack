using Blackjack.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blackjack.Client.Services
{
    public interface IGameHubService
    {
        List<Card> PlayerHand { get; set; }
        List<Card> DealerHand { get; set; }
        GameResult GameState { get; set; }
        bool IsMyTurn { get; set; }
        int DeckCount { get; set; }
        string PlayerName { get; set; }

        Task HitAsync();
        Task StandAsync();
        Task DoubleAsync();
        Task StartNewGameAsync();
    }
}