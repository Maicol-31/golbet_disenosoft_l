namespace GolBet.Services.DTOs;

public class MatchDetailDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string HomeTeamName { get; set; } = string.Empty;
    public string? HomeTeamCrestUrl { get; set; }
    public string AwayTeamName { get; set; } = string.Empty;
    public string? AwayTeamCrestUrl { get; set; }
    public int? HomeGoals { get; set; }
    public int? AwayGoals { get; set; }
    public decimal HomeOdds { get; set; }
    public decimal DrawOdds { get; set; }
    public decimal AwayOdds { get; set; }
    public int TotalBets { get; set; }
}