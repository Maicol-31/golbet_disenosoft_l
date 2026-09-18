using System;
using GolBet.Entities.Common;

namespace GolBet.Entities
{
    public class Match : AuditableEntity
    {
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Scheduled";
        public int? HomeGoals { get; set; }
        public int? AwayGoals { get; set; }
        public decimal HomeOdds { get; set; }
        public decimal DrawOdds { get; set; }
        public decimal AwayOdds { get; set; }
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;
    }
}