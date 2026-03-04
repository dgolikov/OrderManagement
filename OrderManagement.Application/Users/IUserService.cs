using OrderManagement.Domain.Common;
using OrderManagement.Domain.User;

namespace OrderManagement.Application.Users;

public interface IUserService
{
    Task<Result<User>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);
}
