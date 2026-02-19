using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ThePredictions.Web.Client.Authentication;
using ThePredictions.Web.Client.Services.Boosts;
using ThePredictions.Web.Client.Services.Browser;
using ThePredictions.Web.Client.Services.Dashboard;
using ThePredictions.Web.Client.Services.Leagues;
using ThePredictions.Web.Client.ViewModels.Admin.Rounds;

namespace ThePredictions.Web.Client;

public static class DependencyInjection
{
    public static void AddClientServices(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddBlazoredLocalStorage();
        services.AddTransient<CookieHandler>();
      
        services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ILeagueService, LeagueService>();
        services.AddScoped<IDashboardStateService, DashboardStateService>();
        services.AddScoped<IBrowserService, BrowserService>(); 
        services.AddScoped<LeagueDashboardStateService>(); 
        services.AddScoped<BoostClientService>();
        services.AddScoped<EnterResultsViewModel>();
    }
}