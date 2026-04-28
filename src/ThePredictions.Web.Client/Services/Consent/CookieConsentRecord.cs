namespace ThePredictions.Web.Client.Services.Consent;

public class CookieConsentRecord
{
    public int Version { get; set; }
    public CookieConsentDecision Decision { get; set; }
    public DateTime TimestampUtc { get; set; }
    public bool AnalyticsAllowed { get; set; }
    public bool MarketingAllowed { get; set; }
}
