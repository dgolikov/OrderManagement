using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.User;

public interface IUserRepository
{
    Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result<User>> CreateAsync(User user, CancellationToken cancellationToken);
}
