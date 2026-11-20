using GolBet.Services.DTOs;

namespace GolBet.Services;

public interface IMatchService
{
    Task<IEnumerable<MatchDto>> GetBoardAsync(string? status, string? search);
    Task<MatchDetailDto?> GetDetailAsync(int id);
    Task<MatchFormDto?> GetForEditAsync(int id);
    Task CreateAsync(MatchFormDto dto);
    Task UpdateAsync(MatchFormDto dto);
    Task DeactivateAsync(int id);
}
