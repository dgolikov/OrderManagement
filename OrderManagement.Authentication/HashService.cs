using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using OrderManagement.Domain.Common.Services;
using System.Security.Cryptography;
using System.Text;

namespace OrderManagement.Authentication;

public class HashService : IHashService
{
    public string CalculatePasswordHash(string password, string salt)
    {
        var bytes = Convert.FromBase64String(salt);
        var hash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(password, bytes, KeyDerivationPrf.HMACSHA256, 10000, 64)
        );

        return hash;
    }

    public string CalculateTokenHash(string token)
    {
        var value = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(value);
    }

    public string GenerateSalt() => GenerateRandomString(32);

    public string GenerateToken() => GenerateRandomString(64);

    private static string GenerateRandomString(int size)
    {
        var value = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(value);
    }
}
