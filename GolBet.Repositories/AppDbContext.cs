using GolBet.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Bet> Bets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>()
                .Property(t => t.Name)
                .UseCollation("SQL_Latin1_General_CP1_CI_AI");

            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany()
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany()
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppUser>()
                .Property(u => u.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Bet>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Bet>()
                .Property(b => b.OddsAtBet)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Bet>()
                .Property(b => b.Payout)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Bet>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bet>()
                .HasOne(b => b.Match)
                .WithMany()
                .HasForeignKey(b => b.MatchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}