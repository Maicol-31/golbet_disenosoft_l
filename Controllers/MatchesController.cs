using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GolBet.Repositories.Data;

namespace GolBet.Web.Controllers
{
    public class MatchesController : Controller
    {
        private readonly AppDbContext _context;

        public MatchesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? status, string? search)
        {
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search;

            var query = _context.Matches
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(m => m.Status.ToString() == status);
            }

            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(m => m.Local.Name.ToLower().Contains(searchLower)
                                      || m.Visitor.Name.ToLower().Contains(searchLower));
            }

            var matches = await query.ToListAsync();
            return View(matches);
        }
    }
}