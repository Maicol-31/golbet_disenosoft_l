using GolBet.Entities;

namespace GolBet.Repositories.Contracts;

public interface IBetRepository : IGenericRepository<Bet>
{
    Task<bool> ExistsAsync(int matchId, string userId, string selection);
    Task<IEnumerable<Bet>> GetByUserAsync(string userId);
    Task<int> CountByMatchAsync(int matchId);
    Task PlaceBetAtomicAsync(string userId, Bet bet);
    Task<(int Resolved, int Won, decimal Paid)> SettleMatchAtomicAsync(int matchId, int homeGoals, int awayGoals);
}