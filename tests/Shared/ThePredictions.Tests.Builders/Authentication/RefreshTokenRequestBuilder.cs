using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Tests.Builders.Authentication;

public class RefreshTokenRequestBuilder
{
    private string? _token = "valid-refresh-token";

    public RefreshTokenRequestBuilder WithToken(string? token)
    {
        _token = token;
        return this;
    }

    public RefreshTokenRequest Build() => new()
    {
        Token = _token
    };
}
