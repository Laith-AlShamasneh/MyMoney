using Domain.Shared;
using System.Security.Claims;

namespace Application.Interfaces.Shared;

public interface IJwtProvider
{
    /// <summary>
    /// Generates a JWT token for the user.
    /// </summary>
    string GenerateToken(UserClaimsModel userClaims);

    /// <summary>
    /// Generates a secure random refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a token and returns the Principal (ignoring expiration date).
    /// Used for Refresh Token flow.
    /// </summary>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

// Simple DTO to pass data cleanly
public record UserClaimsModel(
    long UserId,
    long PersonId,
    string Email,
    Languages Language,
    IEnumerable<int> RoleIds
);