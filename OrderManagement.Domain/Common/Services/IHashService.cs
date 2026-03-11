namespace OrderManagement.Domain.Common.Services;

public interface IHashService
{
    string CalculatePasswordHash(string password, string salt);
    string CalculateTokenHash(string token);
    string GenerateSalt();
    string GenerateToken();
}
