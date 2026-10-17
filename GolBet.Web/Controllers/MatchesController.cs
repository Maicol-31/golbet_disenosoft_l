using GolBet.Services;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers
{
    public class MatchesController : Controller
    {
        private readonly IMatchService _matchService;
        private static bool _isFirstLoad = true;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        public async Task<IActionResult> Index(string? status, string? search)
        {
            if (_isFirstLoad && string.IsNullOrEmpty(status) && string.IsNullOrEmpty(search))
            {
                _isFirstLoad = false;
                return RedirectToAction("Index", "Home");
            }

            string? filterStatus = (string.IsNullOrEmpty(status) || status == "Todos") ? null : status;

            var matches = await _matchService.GetBoardAsync(filterStatus, search);

            ViewBag.CurrentStatus = string.IsNullOrEmpty(status) ? "Todos" : status;
            ViewBag.CurrentSearch = search;

            return View(matches);
        }

        public IActionResult Welcome()
        {
            return View();
        }
    }
}