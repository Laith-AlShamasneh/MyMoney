using Application.Interfaces.Shared;
using Domain.Shared;
using System.Security.Cryptography;

namespace Application.Implementations.Shared;

public class PasswordHasher : IPasswordHasher
{
    /* * Security Note: 
     * OWASP recommends 600,000 iterations for PBKDF2-HMAC-SHA256 as of 2023.
     * We use 300,000 here as a balance between security and performance for typical servers.
     * Do NOT go below 100,000.
     */
    private const int Iterations = 300_000;
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        Guard.AgainstNullOrWhiteSpace(password, nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            _algorithm,
            HashSize);

        // Combine Salt + Hash into one string for DB storage
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        Guard.AgainstNullOrWhiteSpace(password, nameof(password));
        Guard.AgainstNullOrWhiteSpace(storedHash, nameof(storedHash));

        try
        {
            var hashBytes = Convert.FromBase64String(storedHash);

            // Basic sanity check on length
            if (hashBytes.Length != SaltSize + HashSize)
                return false;

            // Extract the salt from the stored hash
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Re-hash the incoming password with the extracted salt
            var computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                _algorithm,
                HashSize);

            // Extract the original hash part
            var originalHash = new byte[HashSize];
            Array.Copy(hashBytes, SaltSize, originalHash, 0, HashSize);

            // Secure comparison (prevents timing attacks)
            return CryptographicOperations.FixedTimeEquals(computedHash, originalHash);
        }
        catch
        {
            // If base64 parsing fails or other errors, treat as invalid password
            return false;
        }
    }
}