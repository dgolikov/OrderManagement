using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Errors;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.Tokens;
using OrderManagement.Domain.User;
using OrderManagement.Domain.User.Errors;

namespace OrderManagement.Application.Authentication;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IHashService _hashService;
    private readonly IJwtFactory _jwtFactory;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthenticationService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IHashService hashService, IJwtFactory jwtFactory, IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _hashService = hashService;
        _jwtFactory = jwtFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<TokenPair>> LoginAsync(LoginRequest request, DeviceInfo info, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure<TokenPair>(new PropertyIsRequiredError(nameof(request.Email)));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure<TokenPair>(new PropertyIsRequiredError(nameof(request.Password)));
        }

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user.IsFailure)
        {
            return Result.Failure<TokenPair>(new EmailOrPasswordMismatchError());
        }

        var hash = _hashService.CalculatePasswordHash(request.Password, user.Value.Salt);
        if (hash != user.Value.PasswordHash)
        {
            return Result.Failure<TokenPair>(new EmailOrPasswordMismatchError());
        }

        var bundle = _jwtFactory.CreateTokenBundle(user.Value.Id, info);

        if (bundle.IsFailure)
        {
            return Result.Failure<TokenPair>(bundle.Error);
        }

        await _refreshTokenRepository.CreateAsync(bundle.Value.RefreshToken, cancellationToken);

        return bundle.Value.Pair;
    }

    public async Task<Result> LogoutAsync(Guid refreshTokenId, DeviceInfo info, CancellationToken cancellationToken)
    {
        var result = await _refreshTokenRepository.GetByIdAsync(refreshTokenId, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        var token = result.Value;
        var dateTime = _dateTimeProvider.GetUtcNow();
        token.RevokeByIp(info.IP, dateTime);

        await _refreshTokenRepository.UpdateAsync(token, cancellationToken);

        return Result.Succsess();
    }

    public async Task<Result<TokenPair>> RefreshTokensAsync(string token, DeviceInfo info, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Failure<TokenPair>(new PropertyIsRequiredError(nameof(token)));
        }

        var hash = _hashService.CalculateTokenHash(token);
        var result = await _refreshTokenRepository.GetByTokenHashAsync(hash, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<TokenPair>(result.Error);
        }

        var oldToken = result.Value;

        var bundle = _jwtFactory.CreateTokenBundle(oldToken.UserId, info);

        if (result.IsFailure)
        {
            return Result.Failure<TokenPair>(result.Error);
        }

        var dateTime = _dateTimeProvider.GetUtcNow();

        oldToken.RevokeByToken(bundle.Value.RefreshToken, dateTime);
        await _refreshTokenRepository.UpdateAsync(oldToken, cancellationToken);

        await _refreshTokenRepository.CreateAsync(bundle.Value.RefreshToken, cancellationToken);

        return bundle.Value.Pair;
    }
}
