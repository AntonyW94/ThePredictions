using Microsoft.AspNetCore.Mvc;

namespace ThePredictions.API.Controllers;

[ApiController]
public abstract class AuthControllerBase(IConfiguration configuration) : ApiControllerBase
{
    protected void SetTokenCookie(string token)
    {
        var expiryDays = double.Parse(configuration["JwtSettings:RefreshTokenExpiryDays"]!);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(expiryDays),
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Domain = ".thepredictions.co.uk"
        };

        try
        {
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to set 'refreshToken' cookie.", ex);
        }
    }
}