using Application.DTOs.Shared;
using Application.Interfaces.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Implementations.Shared;

public class JwtProvider(IOptions<JwtSettings> jwtOptions) : IJwtProvider
{
    private readonly JwtSettings _settings = jwtOptions.Value;

    public string GenerateToken(UserClaimsModel user)
    {
        // 1. Define Claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new("PersonId", user.PersonId.ToString()),
            new("LangId", ((int)user.Language).ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add Roles (comma separated)
        if (user.RoleIds is not null && user.RoleIds.Any())
        {
            claims.Add(new Claim("RoleIds", string.Join(",", user.RoleIds)));
        }

        // 2. Prepare Key & Creds
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. Create Token
        var token = new JwtSecurityToken(
            issuer: _settings.IssuerKey,
            audience: _settings.AudienceKey,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = _settings.AudienceKey,
            ValidateIssuer = true,
            ValidIssuer = _settings.IssuerKey,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm");
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}