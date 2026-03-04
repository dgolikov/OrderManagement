using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using OrderManagement.Domain.Common.Services;
using System.Security.Cryptography;

namespace OrderManagement.Authentication;

public class HashService : IHashService
{
    public string CalculateHash(string password, string salt)
    {
        var bytes = Convert.FromBase64String(salt);
        var hash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(password, bytes, KeyDerivationPrf.HMACSHA256, 10000, 64)
        );

        return hash;
    }

    public string CreateSalt()
    {
        var value = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(value);
    }
}
