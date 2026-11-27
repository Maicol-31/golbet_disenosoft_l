using System.Globalization;
using GolBet.Entities;
using GolBet.Repositories;
using GolBet.Repositories.Contracts;
using GolBet.Repositories.Implementations;
using GolBet.Repositories.Data;
using GolBet.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AppRole = Microsoft.AspNetCore.Identity.IdentityRole;

var culture = new CultureInfo("es-CO");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IBetRepository, BetRepository>();
builder.Services.AddScoped<IBetService, BetService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await DbSeeder.SeedAsync(context, userManager, roleManager);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/General");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSerilogRequestLogging();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();