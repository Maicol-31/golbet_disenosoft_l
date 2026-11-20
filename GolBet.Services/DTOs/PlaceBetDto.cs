using System.ComponentModel.DataAnnotations;

namespace GolBet.Services.DTOs;

public class PlaceBetDto
{
    public int MatchId { get; set; }

    [Required(ErrorMessage = "Seleccione un resultado")]
    [RegularExpression("^(Home|Draw|Away)$", ErrorMessage = "Selección inválida")]
    public string Selection { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese el monto")]
    [Range(100, 1_000_000, ErrorMessage = "La apuesta mínima es 100 FutCoins")]
    public decimal Amount { get; set; } = 1000m;
}