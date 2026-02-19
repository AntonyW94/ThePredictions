using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ThePredictions.Application.Data;
using ThePredictions.Application.Features.Admin.Rounds.Strategies;
using ThePredictions.Application.Formatters;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Application.Services.Boosts;
using ThePredictions.Domain.Common;
using ThePredictions.Domain.Models;
using ThePredictions.Domain.Services;
using ThePredictions.Infrastructure.Data;
using ThePredictions.Infrastructure.Formatters;
using ThePredictions.Infrastructure.Identity;
using ThePredictions.Infrastructure.Repositories;
using ThePredictions.Infrastructure.Repositories.Boosts;
using ThePredictions.Infrastructure.Services;
using System.Net;

namespace ThePredictions.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IApplicationReadDbConnection, DapperReadDbConnection>();

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password policy
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 4;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddUserStore<DapperUserStore>()
            .AddRoleStore<DapperRoleStore>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                else
                    context.Response.Redirect(context.RedirectUri);

                return Task.CompletedTask;
            };
        });

        services.AddScoped<ILeagueRepository, LeagueRepository>();
        services.AddScoped<ILeagueMemberRepository, LeagueMemberRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IRoundRepository, RoundRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IUserPredictionRepository, UserPredictionRepository>();
        services.AddScoped<IWinningsRepository, WinningsRepository>();
        services.AddScoped<IBoostReadRepository, BoostReadRepository>();
        services.AddScoped<IBoostWriteRepository, BoostWriteRepository>();
        services.AddScoped<ILeagueStatsRepository, LeagueStatsRepository>();
        services.AddScoped<IPrizeStrategy, RoundPrizeStrategy>();
        services.AddScoped<IPrizeStrategy, MonthlyPrizeStrategy>();
        services.AddScoped<IPrizeStrategy, OverallPrizeStrategy>();
        services.AddScoped<IPrizeStrategy, MostExactScoresPrizeStrategy>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<PredictionDomainService>();
        services.AddSingleton<IEmailDateFormatter, UkEmailDateFormatter>();

        services.AddScoped<IAuthenticationTokenService, AuthenticationTokenService>();
        services.AddScoped<IEmailService, BrevoEmailService>();
        services.AddScoped<IReminderService, ReminderService>();
        services.AddScoped<IBoostService, BoostService>();
        services.AddScoped<IUserManager, UserManagerService>();
        services.AddHttpClient<IFootballDataService, FootballDataService>();
        services.AddScoped<ILeagueStatsService, LeagueStatsService>();
        services.AddScoped<ILeagueMembershipService, LeagueMembershipService>();
    }
}