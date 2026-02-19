using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Domain.Common;
using ThePredictions.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ThePredictions.Infrastructure.Services;

public class AuthenticationTokenService : IAuthenticationTokenService
{
    private readonly IUserManager _userManager;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthenticationTokenService(IUserManager userManager, IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository, IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc)> GenerateTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("FirstName", user.FirstName),
            new("LastName", user.LastName),
            new("FullName", $"{user.FirstName} {user.LastName}")
        }.Union(userRoles.Select(role => new Claim("role", role)));

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var expiryMinutes = double.Parse(jwtSettings["ExpiryMinutes"]!);
        var expiresAt = _dateTimeProvider.UtcNow.AddMinutes(expiryMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = _dateTimeProvider.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpiryDays"]!)),
            Created = _dateTimeProvider.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(refreshToken, cancellationToken);

        return (accessToken, refreshToken.Token, expiresAt);
    }
}
