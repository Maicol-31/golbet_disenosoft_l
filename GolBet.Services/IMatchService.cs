using GolBet.Services.DTOs;

namespace GolBet.Services;

public interface IMatchService
{
    Task<IEnumerable<MatchDto>> GetBoardAsync(string? status, string? search);
    Task<MatchDto?> GetDetailAsync(int id);
    Task<MatchFormDto?> GetForEditAsync(int id);
    Task CreateAsync(MatchFormDto dto);
    Task UpdateAsync(MatchFormDto dto);
    Task DeactivateAsync(int id);
    Task StartMatchAsync(int id);
    Task FinishMatchAsync(int id, int? homeGoals, int? awayGoals);
}