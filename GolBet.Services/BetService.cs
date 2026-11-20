using GolBet.Entities;
using GolBet.Repositories.Contracts;
using GolBet.Services.DTOs;

namespace GolBet.Services;

public class BetService : IBetService
{
    private readonly IBetRepository _betRepository;
    private readonly IGenericRepository<Match> _matchRepository;

    public BetService(
        IBetRepository betRepository,
        IGenericRepository<Match> matchRepository)
    {
        _betRepository = betRepository;
        _matchRepository = matchRepository;
    }

    public async Task<BetDto> PlaceBetAsync(string userId, PlaceBetDto dto)
    {
        if (dto.Amount <= 0)
            throw new InvalidOperationException("El monto debe ser mayor que cero.");

        var match = await _matchRepository.GetByIdAsync(dto.MatchId)
            ?? throw new KeyNotFoundException("Partido no encontrado.");

        if (match.Status != "Scheduled" || match.Date <= DateTime.UtcNow)
            throw new InvalidOperationException("Este partido ya no acepta apuestas.");

        if (await _betRepository.ExistsAsync(dto.MatchId, userId, dto.Selection))
            throw new InvalidOperationException("Ya tienes una apuesta con esta selección en este partido.");

        var odds = dto.Selection switch
        {
            "Home" => match.HomeOdds,
            "Draw" => match.DrawOdds,
            "Away" => match.AwayOdds,
            _ => throw new InvalidOperationException("Selección inválida.")
        };

        var bet = new Bet
        {
            UserId = userId,
            MatchId = match.Id,
            Selection = dto.Selection,
            Amount = dto.Amount,
            OddsAtBet = odds,
            Status = "Pending",
            IsActive = true
        };

        await _betRepository.PlaceBetAtomicAsync(userId, bet);

        return new BetDto
        {
            Id = bet.Id,
            MatchId = bet.MatchId,
            Selection = bet.Selection,
            Amount = bet.Amount,
            OddsAtBet = bet.OddsAtBet,
            Status = bet.Status,
            CreatedAt = bet.CreatedAt
        };
    }

    public async Task<IEnumerable<BetDto>> GetUserBetsAsync(string userId)
    {
        var bets = await _betRepository.GetByUserAsync(userId);
        return bets.Select(b => new BetDto
        {
            Id = b.Id,
            MatchId = b.MatchId,
            MatchTitle = b.Match != null && b.Match.HomeTeam != null && b.Match.AwayTeam != null
                ? $"{b.Match.HomeTeam.Name} vs {b.Match.AwayTeam.Name}"
                : $"Partido #{b.MatchId}",
            MatchDate = b.Match?.Date ?? DateTime.UtcNow,
            Selection = b.Selection,
            Amount = b.Amount,
            OddsAtBet = b.OddsAtBet,
            Payout = b.Payout,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        });
    }

    public Task<int> CountMatchBetsAsync(int matchId)
        => _betRepository.CountByMatchAsync(matchId);
}