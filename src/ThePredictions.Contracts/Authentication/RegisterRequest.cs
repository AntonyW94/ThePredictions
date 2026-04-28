namespace ThePredictions.Contracts.Authentication;

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Over18Confirmed { get; set; }
    public bool TermsAccepted { get; set; }
    public bool MarketingOptIn { get; set; }
}
