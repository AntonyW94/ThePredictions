using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Tests.Builders.Authentication;

public class RequestPasswordResetRequestBuilder
{
    private string _email = "user@example.com";

    public RequestPasswordResetRequestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public RequestPasswordResetRequest Build() => new()
    {
        Email = _email
    };
}
