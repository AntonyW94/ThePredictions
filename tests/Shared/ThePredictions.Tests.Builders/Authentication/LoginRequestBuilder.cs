using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Tests.Builders.Authentication;

public class LoginRequestBuilder
{
    private string _email = "user@example.com";
    private string _password = "ValidPass1";

    public LoginRequestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public LoginRequestBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public LoginRequest Build() => new()
    {
        Email = _email,
        Password = _password
    };
}
