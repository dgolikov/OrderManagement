using OrderManagement.Domain.Common;
using OrderManagement.Domain.Tokens;

namespace OrderManagement.Application.Authentication;

public interface IAuthenticationService
{
    Task<Result<TokenPair>> LoginAsync(LoginRequest request, DeviceInfo info, CancellationToken cancellationToken);
    Task<Result> LogoutAsync(Guid refreshTokenId, DeviceInfo info, CancellationToken cancellationToken);
    Task<Result<TokenPair>> RefreshTokensAsync(string token, DeviceInfo info, CancellationToken cancellationToken);
}
