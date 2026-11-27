namespace GolBet.Services.DTOs;

public class BetDto
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public string MatchTitle { get; set; } = null!;
    public DateTime MatchDate { get; set; }
    public string Selection { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal OddsAtBet { get; set; }
    public decimal PotentialPayout => Math.Round(Amount * OddsAtBet, 2);
    public decimal? Payout { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}