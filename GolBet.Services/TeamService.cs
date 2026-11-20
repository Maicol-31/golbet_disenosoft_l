using GolBet.Entities;
using GolBet.Repositories.Contracts;
using GolBet.Services.DTOs;

namespace GolBet.Services;

public class TeamService : ITeamService
{
    private readonly IGenericRepository<Team> _teamRepository;

    public TeamService(IGenericRepository<Team> teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<IEnumerable<TeamDto>> GetAllAsync()
    {
        var teams = await _teamRepository.GetAllAsync();
        return teams
            .Where(t => t.IsActive)
            .Select(t => new TeamDto
            {
                Id = t.Id,
                Name = t.Name,
                City = t.City,
                CrestUrl = t.CrestUrl,
                IsActive = t.IsActive
            });
    }

    public async Task<TeamFormDto?> GetForEditAsync(int id)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team is null || !team.IsActive) return null;

        return new TeamFormDto
        {
            Id = team.Id,
            Name = team.Name,
            City = team.City,
            CrestUrl = team.CrestUrl
        };
    }

    public async Task CreateAsync(TeamFormDto dto)
    {
        var team = new Team
        {
            Name = dto.Name,
            City = dto.City,
            CrestUrl = dto.CrestUrl,
            IsActive = true
        };
        await _teamRepository.AddAsync(team);
        await _teamRepository.SaveAsync();
    }

    public async Task UpdateAsync(TeamFormDto dto)
    {
        var team = await _teamRepository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Team {dto.Id} not found.");

        team.Name = dto.Name;
        team.City = dto.City;
        team.CrestUrl = dto.CrestUrl;

        _teamRepository.Update(team);
        await _teamRepository.SaveAsync();
    }

    public async Task DeactivateAsync(int id)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team is null) return;

        team.IsActive = false;
        _teamRepository.Update(team);
        await _teamRepository.SaveAsync();
    }
}