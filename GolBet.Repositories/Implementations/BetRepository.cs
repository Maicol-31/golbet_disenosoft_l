using GolBet.Entities;
using GolBet.Repositories.Contracts;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Implementations;

public class BetRepository : GenericRepository<Bet>, IBetRepository
{
    public BetRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsAsync(int matchId, string userId, string selection)
        => await _context.Bets.AnyAsync(b =>
            b.MatchId == matchId && b.UserId == userId && b.Selection == selection && b.IsActive);

    public async Task<IEnumerable<Bet>> GetByUserAsync(string userId)
        => await _context.Bets
            .AsNoTracking()
            .Include(b => b.Match).ThenInclude(m => m.HomeTeam)
            .Include(b => b.Match).ThenInclude(m => m.AwayTeam)
            .Where(b => b.UserId == userId && b.IsActive)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public async Task<int> CountByMatchAsync(int matchId)
        => await _context.Bets.CountAsync(b => b.MatchId == matchId && b.IsActive);

    public async Task PlaceBetAtomicAsync(string userId, Bet bet)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        if (user.Balance < bet.Amount)
            throw new InvalidOperationException($"Saldo insuficiente: tienes {user.Balance:N0} FutCoins.");

        user.Balance -= bet.Amount;
        _context.Bets.Add(bet);

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task<(int Resolved, int Won, decimal Paid)> SettleMatchAtomicAsync(int matchId, int homeGoals, int awayGoals)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId && m.IsActive)
            ?? throw new KeyNotFoundException("Partido no encontrado.");

        if (match.Status == "Finished")
            throw new InvalidOperationException("Este partido ya fue cerrado y sus apuestas resueltas.");

        if (match.Status != "InPlay")
            throw new InvalidOperationException("El partido debe estar En juego para cerrarse. Inícialo primero.");

        match.HomeGoals = homeGoals;
        match.AwayGoals = awayGoals;
        match.Status = "Finished";

        string outcome = homeGoals > awayGoals ? "Home"
                       : homeGoals < awayGoals ? "Away"
                       : "Draw";

        var pendingBets = await _context.Bets
            .Include(b => b.User)
            .Where(b => b.MatchId == matchId && b.Status == "Pending" && b.IsActive)
            .ToListAsync();

        int wonBets = 0;
        decimal totalPaid = 0m;

        foreach (var bet in pendingBets)
        {
            if (bet.Selection == outcome)
            {
                decimal payout = Math.Round(bet.Amount * bet.OddsAtBet, 2);
                bet.Status = "Won";
                bet.Payout = payout;
                bet.User.Balance += payout;
                wonBets++;
                totalPaid += payout;
            }
            else
            {
                bet.Status = "Lost";
                bet.Payout = 0m;
            }
        }

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return (pendingBets.Count, wonBets, totalPaid);
    }
}