using System.ComponentModel.DataAnnotations;
using GolBet.Entities.Common;

namespace GolBet.Entities;

public class Bet : AuditableEntity
{
    public string UserId { get; set; } = null!;
    public AppUser User { get; set; } = null!;

    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    [MaxLength(10)]
    public string Selection { get; set; } = null!;

    public decimal Amount { get; set; }
    public decimal OddsAtBet { get; set; }
    public decimal? Payout { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}