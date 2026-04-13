using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using ThePredictions.Application.Configuration;
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
using ThePredictions.Infrastructure.Data.Resilience;
using ThePredictions.Infrastructure.Formatters;
using ThePredictions.Infrastructure.HealthChecks;
using ThePredictions.Infrastructure.Identity;
using ThePredictions.Infrastructure.Repositories;
using ThePredictions.Infrastructure.Repositories.Boosts;
using ThePredictions.Infrastructure.Services;
using System.Net;

namespace ThePredictions.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SqlRetryPolicyOptions>(
            configuration.GetSection(SqlRetryPolicyOptions.SectionName));
        services.AddSingleton<ISqlRetryPolicy, SqlRetryPolicy>();

        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IApplicationReadDbConnection, DapperReadDbConnection>();

        var connectionString = configuration.GetConnectionString("DataConnection")
                               ?? throw new InvalidOperationException("Connection string 'DataConnection' not found.");

        services.AddHealthChecks()
            .AddSqlServer(connectionString, name: "database", tags: ["ready"])
            .AddCheck<FootballApiHealthCheck>("football-api", tags: ["ready"]);

        services.AddHttpClient<FootballApiHealthCheck>();

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
        services.AddScoped<ITournamentRoundMappingRepository, TournamentRoundMappingRepository>();
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
        services.AddHttpClient<IFootballDataService, FootballDataService>((serviceProvider, client) =>
        {
            var timeoutSettings = serviceProvider.GetRequiredService<IOptions<TimeoutSettings>>().Value;
            client.Timeout = TimeSpan.FromSeconds(timeoutSettings.FootballApiTimeoutSeconds);
        })
            .AddResilienceHandler("FootballApi", ConfigureFootballApiResilience);

        services.AddScoped<ILeagueStatsService, LeagueStatsService>();
        services.AddScoped<ILeagueMembershipService, LeagueMembershipService>();
    }

    private static void ConfigureFootballApiResilience(
        ResiliencePipelineBuilder<HttpResponseMessage> builder,
        ResilienceHandlerContext context)
    {
        var settings = context.ServiceProvider
            .GetRequiredService<IOptions<FootballApiResilienceSettings>>().Value;

        builder
            .AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = settings.MaxRetryAttempts,
                Delay = TimeSpan.FromSeconds(settings.MedianFirstRetryDelaySeconds),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(r =>
                        r.StatusCode == HttpStatusCode.TooManyRequests
                        || r.StatusCode == HttpStatusCode.RequestTimeout
                        || (int)r.StatusCode >= 500)
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>(),
                OnRetry = args =>
                {
                    var logger = context.ServiceProvider
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("ThePredictions.Infrastructure.Resilience.FootballApi");

                    logger.LogWarning(
                        args.Outcome.Exception,
                        "Football API retry attempt {AttemptNumber} of {MaxRetryAttempts} after {RetryDelayMs}ms. Status code: {StatusCode}",
                        args.AttemptNumber + 1,
                        settings.MaxRetryAttempts,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Result?.StatusCode);

                    return ValueTask.CompletedTask;
                }
            })
            .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = settings.CircuitBreakerFailureThreshold,
                BreakDuration = TimeSpan.FromSeconds(settings.CircuitBreakerBreakDurationSeconds),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(r =>
                        r.StatusCode == HttpStatusCode.TooManyRequests
                        || r.StatusCode == HttpStatusCode.RequestTimeout
                        || (int)r.StatusCode >= 500)
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>(),
                OnOpened = args =>
                {
                    var logger = context.ServiceProvider
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("ThePredictions.Infrastructure.Resilience.FootballApi");

                    logger.LogError(
                        args.Outcome.Exception,
                        "Football API circuit breaker opened for {BreakDurationSeconds}s due to repeated failures. Status code: {StatusCode}",
                        settings.CircuitBreakerBreakDurationSeconds,
                        args.Outcome.Result?.StatusCode);

                    return ValueTask.CompletedTask;
                },
                OnClosed = _ =>
                {
                    var logger = context.ServiceProvider
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("ThePredictions.Infrastructure.Resilience.FootballApi");

                    logger.LogInformation("Football API circuit breaker closed. Service recovered");

                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = _ =>
                {
                    var logger = context.ServiceProvider
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("ThePredictions.Infrastructure.Resilience.FootballApi");

                    logger.LogInformation("Football API circuit breaker half-opened. Testing service availability");

                    return ValueTask.CompletedTask;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(settings.RequestTimeoutSeconds));
    }
}
