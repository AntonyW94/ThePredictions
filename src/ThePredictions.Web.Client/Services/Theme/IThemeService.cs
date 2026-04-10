namespace ThePredictions.Web.Client.Services.Theme;

public interface IThemeService
{
    string CurrentTheme { get; }
    bool IsDarkMode { get; }
    event Action? OnThemeChanged;
    Task InitialiseAsync();
    Task ToggleThemeAsync();
}
