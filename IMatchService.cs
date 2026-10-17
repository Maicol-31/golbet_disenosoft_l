using System.Collections.Generic;
using System.Threading.Tasks;
using GolBet.Services.DTOs;

namespace GolBet.Services
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchDto>> GetBoardAsync(string? status, string? search);
    }
}

