using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Tests.Builders.Authentication;

public class ResetPasswordRequestBuilder
{
    private string _token = "valid-reset-token";
    private string _newPassword = "NewValidPass1";
    private string _confirmPassword = "NewValidPass1";

    public ResetPasswordRequestBuilder WithToken(string token)
    {
        _token = token;
        return this;
    }

    public ResetPasswordRequestBuilder WithNewPassword(string newPassword)
    {
        _newPassword = newPassword;
        return this;
    }

    public ResetPasswordRequestBuilder WithConfirmPassword(string confirmPassword)
    {
        _confirmPassword = confirmPassword;
        return this;
    }

    public ResetPasswordRequest Build() => new()
    {
        Token = _token,
        NewPassword = _newPassword,
        ConfirmPassword = _confirmPassword
    };
}
