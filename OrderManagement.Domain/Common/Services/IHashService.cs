namespace OrderManagement.Domain.Common.Services;

public interface IHashService
{
    string CalculateHash(string password, string salt);
    string CreateSalt();
}
