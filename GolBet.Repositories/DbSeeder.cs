using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GolBet.Entities;
using GolBet.Repositories.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppRole = Microsoft.AspNetCore.Identity.IdentityRole;

namespace GolBet.Repositories
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            await context.Database.MigrateAsync();

            foreach (var roleName in new[] { "Admin", "Apostador" })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole(roleName));
                }
            }

            if (await userManager.FindByEmailAsync("admin@golbet.co") is null)
            {
                var admin = new AppUser
                {
                    UserName = "admin@golbet.co",
                    Email = "admin@golbet.co",
                    EmailConfirmed = true,
                    FullName = "Administrador GolBet",
                    Balance = 0m
                };
                await userManager.CreateAsync(admin, "Admin123*");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (await userManager.FindByEmailAsync("demo@golbet.co") is null)
            {
                var demo = new AppUser
                {
                    UserName = "demo@golbet.co",
                    Email = "demo@golbet.co",
                    EmailConfirmed = true,
                    FullName = "Apostador Demo",
                    Balance = 100_000m
                };
                await userManager.CreateAsync(demo, "Demo123*");
                await userManager.AddToRoleAsync(demo, "Apostador");
            }

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
                    HomeTeamId = teams.ElementAt(0).Id,
                    AwayTeamId = teams.ElementAt(1).Id,
                    Date = DateTime.UtcNow.AddDays(2),
                    Status = "Scheduled",
                    HomeOdds = 2.10m, DrawOdds = 3.20m, AwayOdds = 3.50m
                },
                new Match
                {
                    HomeTeamId = teams.ElementAt(2).Id,
                    AwayTeamId = teams.ElementAt(3).Id,
                    Date = DateTime.UtcNow.AddDays(3),
                    Status = "Scheduled",
                    HomeOdds = 1.95m, DrawOdds = 3.40m, AwayOdds = 3.80m
                },
                new Match
                {
                    HomeTeamId = teams.ElementAt(4).Id,
                    AwayTeamId = teams.ElementAt(5).Id,
                    Date = DateTime.UtcNow.AddDays(-1),
                    Status = "Finished",
                    HomeGoals = 2, AwayGoals = 1,
                    HomeOdds = 2.00m, DrawOdds = 3.10m, AwayOdds = 3.60m
                },
                new Match
                {
                    HomeTeamId = teams.ElementAt(6).Id,
                    AwayTeamId = teams.ElementAt(0).Id,
                    Date = DateTime.UtcNow.AddDays(4),
                    Status = "Scheduled",
                    HomeOdds = 2.20m, DrawOdds = 3.15m, AwayOdds = 2.90m
                },
                new Match
                {
                    HomeTeamId = teams.ElementAt(1).Id,
                    AwayTeamId = teams.ElementAt(7).Id,
                    Date = DateTime.UtcNow.AddDays(5),
                    Status = "Scheduled",
                    HomeOdds = 1.85m, DrawOdds = 3.30m, AwayOdds = 4.10m
                },
                new Match
                {
                    HomeTeamId = teams.ElementAt(3).Id,
                    AwayTeamId = teams.ElementAt(6).Id,
                    Date = DateTime.UtcNow.AddHours(2),
                    Status = "InPlay",
                    HomeGoals = 0, AwayGoals = 0,
                    HomeOdds = 2.40m, DrawOdds = 3.00m, AwayOdds = 2.85m
                },
                new Match
                {
                    HomeTeamId = teams.ElementAt(7).Id,
                    AwayTeamId = teams.ElementAt(2).Id,
                    Date = DateTime.UtcNow.AddDays(6),
                    Status = "Scheduled",
                    HomeOdds = 2.30m, DrawOdds = 3.20m, AwayOdds = 2.70m
                },
                new Match
                {
                    HomeTeamId = teams.ElementAt(5).Id,
                    AwayTeamId = teams.ElementAt(4).Id,
                    Date = DateTime.UtcNow.AddDays(7),
                    Status = "Scheduled",
                    HomeOdds = 2.50m, DrawOdds = 3.10m, AwayOdds = 2.65m
                }
            };
            context.Matches.AddRange(matches);
            await context.SaveChangesAsync();
        }
    }
}