using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace ThePredictions.Web.Client.Services.Theme;

public class ThemeService(ILocalStorageService localStorage, HttpClient httpClient) : IThemeService
{
    private const string StorageKey = "themePreference";

    public string CurrentTheme { get; private set; } = "light";
    public bool IsDarkMode => CurrentTheme == "dark";
    public event Action? OnThemeChanged;

    public async Task InitialiseAsync()
    {
        var cached = await localStorage.GetItemAsync<string>(StorageKey);
        if (!string.IsNullOrEmpty(cached))
        {
            CurrentTheme = cached;
            OnThemeChanged?.Invoke();
        }
    }

    public async Task ToggleThemeAsync()
    {
        CurrentTheme = IsDarkMode ? "light" : "dark";
        await localStorage.SetItemAsync(StorageKey, CurrentTheme);
        OnThemeChanged?.Invoke();

        try
        {
            await httpClient.PutAsJsonAsync("api/account/theme", CurrentTheme);
        }
        catch
        {
            // Fire-and-forget — localStorage is the primary store for instant UX
        }
    }

    public void ToggleThemeTransient()
    {
        CurrentTheme = IsDarkMode ? "light" : "dark";
        OnThemeChanged?.Invoke();
    }

    public async Task ClearPreferenceAsync()
    {
        CurrentTheme = "light";
        await localStorage.RemoveItemAsync(StorageKey);
        OnThemeChanged?.Invoke();
    }

    public async Task SyncFromServerAsync(string serverTheme)
    {
        if (string.IsNullOrEmpty(serverTheme))
            return;

        var cached = await localStorage.GetItemAsync<string>(StorageKey);
        if (cached == serverTheme)
            return;

        CurrentTheme = serverTheme;
        await localStorage.SetItemAsync(StorageKey, CurrentTheme);
        OnThemeChanged?.Invoke();
    }
}
