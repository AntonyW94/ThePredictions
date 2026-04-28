namespace ThePredictions.Web.Client.Services.Consent;

public interface IConsentBannerService
{
    bool HasResponded { get; }
    CookieConsentDecision Decision { get; }
    bool AnalyticsAllowed { get; }
    bool MarketingAllowed { get; }
    DateTime? RespondedAtUtc { get; }
    event Action? OnStateChange;
    Task InitialiseAsync();
    Task AcceptAllAsync();
    Task RejectNonEssentialAsync();
    Task SaveCustomPreferencesAsync(bool analyticsAllowed, bool marketingAllowed);
    Task ReopenAsync();
}
