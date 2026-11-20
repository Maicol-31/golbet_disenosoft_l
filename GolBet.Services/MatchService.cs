using GolBet.Entities;
using GolBet.Repositories.Contracts;
using GolBet.Services.DTOs;
using GolBet.Services.Helpers;

namespace GolBet.Services;

public class MatchService : IMatchService
{
    private readonly IGenericRepository<Match> _genericRepository;
    private readonly IBetRepository _betRepository;

    public MatchService(
        IGenericRepository<Match> genericRepository,
        IBetRepository betRepository)
    {
        _genericRepository = genericRepository;
        _betRepository = betRepository;
    }

    public async Task<IEnumerable<MatchDto>> GetBoardAsync(string? status, string? search)
    {
        var matches = await _genericRepository.GetAllWithTeamsAsync(status, search);
        return matches
            .Where(m => m.IsActive)
            .Select(m => new MatchDto
            {
                Id = m.Id,
                Date = m.Date,
                Status = m.Status,
                HomeTeamName = m.HomeTeam != null ? m.HomeTeam.Name : string.Empty,
                AwayTeamName = m.AwayTeam != null ? m.AwayTeam.Name : string.Empty,
                HomeGoals = m.HomeGoals,
                AwayGoals = m.AwayGoals,
                HomeOdds = m.HomeOdds,
                DrawOdds = m.DrawOdds,
                AwayOdds = m.AwayOdds
            });
    }

    public async Task<MatchDto?> GetDetailAsync(int id)
    {
        var matches = await _genericRepository.GetAllWithTeamsAsync();
        var m = matches.FirstOrDefault(x => x.Id == id && x.IsActive);
        if (m is null) return null;

        var totalBets = await _betRepository.CountByMatchAsync(id);

        return new MatchDto
        {
            Id = m.Id,
            Date = m.Date,
            Status = m.Status,
            HomeTeamName = m.HomeTeam?.Name ?? string.Empty,
            HomeTeamCrestUrl = m.HomeTeam?.CrestUrl,
            AwayTeamName = m.AwayTeam?.Name ?? string.Empty,
            AwayTeamCrestUrl = m.AwayTeam?.CrestUrl,
            HomeGoals = m.HomeGoals,
            AwayGoals = m.AwayGoals,
            HomeOdds = m.HomeOdds,
            DrawOdds = m.DrawOdds,
            AwayOdds = m.AwayOdds,
            TotalBets = totalBets
        };
    }

    public async Task<MatchFormDto?> GetForEditAsync(int id)
    {
        var match = await _genericRepository.GetByIdAsync(id);
        if (match is null || !match.IsActive) return null;

        return new MatchFormDto
        {
            Id = match.Id,
            HomeTeamId = match.HomeTeamId,
            AwayTeamId = match.AwayTeamId,
            Date = match.Date.ToColombiaTime(),
            HomeOdds = match.HomeOdds,
            DrawOdds = match.DrawOdds,
            AwayOdds = match.AwayOdds
        };
    }

    public async Task CreateAsync(MatchFormDto dto)
    {
        ValidateBusinessRules(dto);

        var match = new Match
        {
            HomeTeamId = dto.HomeTeamId,
            AwayTeamId = dto.AwayTeamId,
            Date = dto.Date.ToUtcFromColombia(),
            HomeOdds = dto.HomeOdds,
            DrawOdds = dto.DrawOdds,
            AwayOdds = dto.AwayOdds,
            Status = "Scheduled",
            IsActive = true
        };

        await _genericRepository.AddAsync(match);
        await _genericRepository.SaveAsync();
    }

    public async Task UpdateAsync(MatchFormDto dto)
    {
        ValidateBusinessRules(dto);

        var match = await _genericRepository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Match {dto.Id} not found.");

        match.HomeTeamId = dto.HomeTeamId;
        match.AwayTeamId = dto.AwayTeamId;
        match.Date = dto.Date.ToUtcFromColombia();
        match.HomeOdds = dto.HomeOdds;
        match.DrawOdds = dto.DrawOdds;
        match.AwayOdds = dto.AwayOdds;

        _genericRepository.Update(match);
        await _genericRepository.SaveAsync();
    }

    public async Task DeactivateAsync(int id)
    {
        var match = await _genericRepository.GetByIdAsync(id);
        if (match is null) return;

        match.IsActive = false;
        _genericRepository.Update(match);
        await _genericRepository.SaveAsync();
    }

    public async Task StartMatchAsync(int id)
    {
        var match = await _genericRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Partido no encontrado.");

        if (match.Status != "Scheduled")
            throw new InvalidOperationException("Solo se pueden iniciar partidos programados.");

        match.Status = "InPlay";
        match.HomeGoals = 0;
        match.AwayGoals = 0;

        _genericRepository.Update(match);
        await _genericRepository.SaveAsync();
    }

    public async Task<ResolutionSummaryDto> FinishMatchAsync(int id, int? homeGoals, int? awayGoals)
    {
        int finalHome = homeGoals ?? Random.Shared.Next(0, 5);
        int finalAway = awayGoals ?? Random.Shared.Next(0, 5);

        if (finalHome < 0 || finalAway < 0)
            throw new InvalidOperationException("Los goles no pueden ser negativos.");

        var (resolved, won, paid) = await _betRepository.SettleMatchAtomicAsync(id, finalHome, finalAway);

        return new ResolutionSummaryDto
        {
            HomeGoals = finalHome,
            AwayGoals = finalAway,
            ResolvedBets = resolved,
            WonBets = won,
            TotalPaid = paid
        };
    }

    private static void ValidateBusinessRules(MatchFormDto dto)
    {
        if (dto.HomeTeamId == dto.AwayTeamId)
            throw new InvalidOperationException(
                "El equipo local y el visitante no pueden ser el mismo.");

        if (dto.Date.ToUtcFromColombia() <= DateTime.UtcNow)
            throw new InvalidOperationException(
                "La fecha del partido debe ser futura.");
    }
}