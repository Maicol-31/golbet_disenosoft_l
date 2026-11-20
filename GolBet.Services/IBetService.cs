using GolBet.Services.DTOs;

namespace GolBet.Services;

public interface IBetService
{
    Task<BetDto> PlaceBetAsync(string userId, PlaceBetDto dto);
    Task<IEnumerable<BetDto>> GetUserBetsAsync(string userId);
    Task<int> CountMatchBetsAsync(int matchId);
}