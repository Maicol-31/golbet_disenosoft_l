using GolBet.Entities;
using GolBet.Repositories.Contracts;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public void Update(T entity) => _context.Set<T>().Update(entity);

        public void Delete(T entity) => _context.Set<T>().Remove(entity);

        public async Task<bool> SaveAsync() => await _context.SaveChangesAsync() > 0;

        public async Task<IEnumerable<T>> GetAllWithTeamsAsync(string? status = null, string? search = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (typeof(T) == typeof(Match))
            {
                var matchQuery = query as IQueryable<Match>;
                if (matchQuery != null)
                {
                    matchQuery = matchQuery
                        .Include(m => m.HomeTeam)
                        .Include(m => m.AwayTeam);

                    if (!string.IsNullOrEmpty(status))
                    {
                        string statusDb = status switch
                        {
                            "Programados" => "Scheduled",
                            "En juego" => "InPlay",
                            "Finalizados" => "Finished",
                            _ => status
                        };
                        matchQuery = matchQuery.Where(m => m.Status.ToString() == statusDb);
                    }

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        string term = search.Trim();
                        matchQuery = matchQuery.Where(m => m.HomeTeam.Name.Contains(term)
                                                      || m.AwayTeam.Name.Contains(term));
                    }

                    var result = await matchQuery.OrderBy(m => m.Date).ToListAsync();
                    return result as IEnumerable<T> ?? Enumerable.Empty<T>();
                }
            }

            return await query.ToListAsync();
        }
    }
}