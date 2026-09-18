using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GolBet.Entities;
using GolBet.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            if (await context.Teams.AnyAsync()) return;

            var teams = new List<Team>
            {
                new Team { Name = "Atlético Nacional",       City = "Medellín" },
                new Team { Name = "Independiente Medellín",  City = "Medellín" },
                new Team { Name = "Millonarios",             City = "Bogotá" },
                new Team { Name = "Independiente Santa Fe",  City = "Bogotá" },
                new Team { Name = "América de Cali",         City = "Cali" },
                new Team { Name = "Deportivo Cali",          City = "Cali" },
                new Team { Name = "Junior de Barranquilla",  City = "Barranquilla" },
                new Team { Name = "Once Caldas",             City = "Manizales" }
            };

            context.Teams.AddRange(teams);
            await context.SaveChangesAsync();

            var matches = new List<Match>
            {
                new Match
                {
                    HomeTeamId = teams[0].Id, // Nacional
                    AwayTeamId = teams[1].Id, // Medellín
                    Date = DateTime.UtcNow.AddDays(2),
                    Status = "Scheduled",
                    HomeOdds = 2.10m, DrawOdds = 3.20m, AwayOdds = 3.50m
                },
                new Match
                {
                    HomeTeamId = teams[2].Id, // Millonarios
                    AwayTeamId = teams[3].Id, // Santa Fe
                    Date = DateTime.UtcNow.AddDays(3),
                    Status = "Scheduled",
                    HomeOdds = 1.95m, DrawOdds = 3.40m, AwayOdds = 3.80m
                },
                new Match
                {
                    HomeTeamId = teams[4].Id, // América
                    AwayTeamId = teams[5].Id, // Cali
                    Date = DateTime.UtcNow.AddDays(-1),
                    Status = "Finished",
                    HomeGoals = 2, AwayGoals = 1,
                    HomeOdds = 2.00m, DrawOdds = 3.10m, AwayOdds = 3.60m
                }
            };

            context.Matches.AddRange(matches);
            await context.SaveChangesAsync();
        }
    }
}
