using System.Security.Claims;
using GolBet.Services;
using GolBet.Services.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers;

[Authorize(Roles = "Apostador")]
public class BetsController : Controller
{
    private readonly IBetService _betService;

    public BetsController(IBetService betService)
        => _betService = betService;

    public async Task<IActionResult> MyBets()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var bets = await _betService.GetUserBetsAsync(userId);
        return View(bets);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Place(PlaceBetDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Datos de apuesta inválidos. Verifica la selección y el monto (mínimo 100 FutCoins).";
            return RedirectToAction("Detail", "Matches", new { id = dto.MatchId });
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var bet = await _betService.PlaceBetAsync(userId, dto);
            TempData["Success"] = $"¡Apuesta registrada por {bet.Amount:N0} FutCoins (cuota {bet.OddsAtBet:0.00})!";
            return RedirectToAction(nameof(MyBets));
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Detail", "Matches", new { id = dto.MatchId });
        }
    }
}