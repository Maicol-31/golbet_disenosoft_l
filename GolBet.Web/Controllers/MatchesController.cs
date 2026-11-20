using GolBet.Services;
using GolBet.Services.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolBet.Web.Controllers;

public class MatchesController : Controller
{
    private readonly IMatchService _matchService;
    private readonly ITeamService _teamService;
    private static bool _isFirstLoad = true;

    public MatchesController(IMatchService matchService, ITeamService teamService)
    {
        _matchService = matchService;
        _teamService = teamService;
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

    public async Task<IActionResult> Detail(int id)
    {
        var match = await _matchService.GetDetailAsync(id);
        if (match is null) return NotFound();
        return View(match);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadTeamsAsync();
        return View(new MatchFormDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(MatchFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadTeamsAsync();
            return View(dto);
        }

        try
        {
            await _matchService.CreateAsync(dto);
            TempData["Success"] = "Partido creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadTeamsAsync();
            return View(dto);
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _matchService.GetForEditAsync(id);
        if (dto is null) return NotFound();

        await LoadTeamsAsync();
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(MatchFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadTeamsAsync();
            return View(dto);
        }

        try
        {
            await _matchService.UpdateAsync(dto);
            TempData["Success"] = "Partido actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadTeamsAsync();
            return View(dto);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _matchService.DeactivateAsync(id);
        TempData["Success"] = "Partido desactivado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadTeamsAsync()
    {
        var teams = await _teamService.GetAllAsync();
        ViewBag.Teams = new SelectList(teams, "Id", "Name");
    }

    public IActionResult Welcome()
    {
        return View();
    }
}