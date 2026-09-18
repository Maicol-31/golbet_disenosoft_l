using GolBet.Entities.Common;

namespace GolBet.Entities
{
    public class Team : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? CrestUrl { get; set; }
    }
}