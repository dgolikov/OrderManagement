using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Errors;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.User;
using OrderManagement.Domain.User.Errors;

namespace OrderManagement.Application.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IHashService _hashService;

    public UserService(IUserRepository userRepository, IHashService hashService)
    {
        _userRepository = userRepository;
        _hashService = hashService;
    }

    public async Task<Result<User>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure<User>(new PropertyIsRequiredError(nameof(request.Email)));
        }

        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser.IsSuccess)
        {
            return Result.Failure<User>(new UserAlreadyExistError(request.Email));
        }

        var salt = _hashService.GenerateSalt();
        var hash = _hashService.CalculatePasswordHash(request.Password, salt);

        var user = User.Create(request.FirstName, request.LastName, request.Email, hash, salt);

        if (user.IsFailure)
        {
            return user;
        }

        return await _userRepository.CreateAsync(user.Value, cancellationToken);
    }
}
