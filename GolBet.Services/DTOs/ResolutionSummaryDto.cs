namespace GolBet.Services.DTOs;

public class ResolutionSummaryDto
{
    public int HomeGoals { get; set; }
    public int AwayGoals { get; set; }
    public int ResolvedBets { get; set; }
    public int WonBets { get; set; }
    public decimal TotalPaid { get; set; }
}