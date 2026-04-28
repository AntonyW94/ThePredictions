using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

namespace ThePredictions.Domain.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PreferredTheme { get; set; } = "light";
    public DateTime? Over18ConfirmedAtUtc { get; set; }
    public DateTime? TermsAcceptedAtUtc { get; set; }
    public DateTime? MarketingOptInAtUtc { get; set; }

    public static ApplicationUser Create(string firstName, string lastName, string email)
    {
        Validate(firstName, lastName);
        Guard.Against.NullOrWhiteSpace(email);

        return new ApplicationUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email
        };
    }

    public void UpdateDetails(string firstName, string lastName, string? phoneNumber)
    {
        Validate(firstName, lastName);

        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    public void RecordRegistrationConsent(bool over18Confirmed, bool termsAccepted, bool marketingOptIn, DateTime nowUtc)
    {
        if (!over18Confirmed)
            throw new ArgumentException("Age confirmation is required to register.", nameof(over18Confirmed));

        if (!termsAccepted)
            throw new ArgumentException("Terms acceptance is required to register.", nameof(termsAccepted));

        Over18ConfirmedAtUtc = nowUtc;
        TermsAcceptedAtUtc = nowUtc;
        MarketingOptInAtUtc = marketingOptIn ? nowUtc : null;
    }

    private static void Validate(string firstName, string lastName)
    {
        Guard.Against.NullOrWhiteSpace(firstName);
        Guard.Against.NullOrWhiteSpace(lastName);
    }
}
