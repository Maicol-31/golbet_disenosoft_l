using GolBet.Entities;

namespace GolBet.Repositories.Contracts;

public interface IBetRepository : IGenericRepository<Bet>
{
    Task<IEnumerable<Bet>> GetByUserAsync(string userId);
    Task<int> CountByMatchAsync(int matchId);
    Task PlaceBetAtomicAsync(string userId, Bet bet);
}