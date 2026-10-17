using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GolBet.Entities;
using GolBet.Repositories.Contracts;
using GolBet.Services.DTOs;

namespace GolBet.Services
{
    public class MatchService : IMatchService
    {
        private readonly IGenericRepository<Match> _genericRepository;

        public MatchService(IGenericRepository<Match> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<IEnumerable<MatchDto>> GetBoardAsync(string? status, string? search)
        {
            var matches = await _genericRepository.GetAllWithTeamsAsync(status, search);

            return matches.Select(m => new MatchDto
            {
                Id = m.Id,
                Date = m.Date,
                Status = m.Status.ToString(),
                HomeTeamName = m.HomeTeam != null ? m.HomeTeam.Name : string.Empty,
                AwayTeamName = m.AwayTeam != null ? m.AwayTeam.Name : string.Empty,
                HomeGoals = m.HomeGoals,
                AwayGoals = m.AwayGoals,
                HomeOdds = m.HomeOdds,
                DrawOdds = m.DrawOdds,
                AwayOdds = m.AwayOdds
            });
        }
    }
}

