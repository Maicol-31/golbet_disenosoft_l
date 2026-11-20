using GolBet.Entities;
using GolBet.Repositories.Contracts;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Implementations;

public class BetRepository : GenericRepository<Bet>, IBetRepository
{
    public BetRepository(AppDbContext context) : base(context) { }

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
            throw new InvalidOperationException("Saldo insuficiente para realizar esta apuesta.");

        user.Balance -= bet.Amount;
        _context.Bets.Add(bet);

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }
}