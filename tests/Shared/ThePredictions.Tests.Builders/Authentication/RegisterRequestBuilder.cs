using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Tests.Builders.Authentication;

public class RegisterRequestBuilder
{
    private string _firstName = "John";
    private string _lastName = "Smith";
    private string _email = "john.smith@example.com";
    private string _password = "ValidPass1";
    private bool _over18Confirmed = true;
    private bool _termsAccepted = true;
    private bool _marketingOptIn;

    public RegisterRequestBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public RegisterRequestBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public RegisterRequestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public RegisterRequestBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public RegisterRequestBuilder WithOver18Confirmed(bool over18Confirmed)
    {
        _over18Confirmed = over18Confirmed;
        return this;
    }

    public RegisterRequestBuilder WithTermsAccepted(bool termsAccepted)
    {
        _termsAccepted = termsAccepted;
        return this;
    }

    public RegisterRequestBuilder WithMarketingOptIn(bool marketingOptIn)
    {
        _marketingOptIn = marketingOptIn;
        return this;
    }

    public RegisterRequest Build() => new()
    {
        FirstName = _firstName,
        LastName = _lastName,
        Email = _email,
        Password = _password,
        Over18Confirmed = _over18Confirmed,
        TermsAccepted = _termsAccepted,
        MarketingOptIn = _marketingOptIn
    };
}
