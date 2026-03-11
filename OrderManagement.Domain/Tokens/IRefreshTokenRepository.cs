using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Tokens;

public interface IRefreshTokenRepository
{
    Task<Result<RefreshToken>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<RefreshToken>> GetByTokenHashAsync(string hash, CancellationToken cancellationToken);
    Task<Result<RefreshToken>> CreateAsync(RefreshToken token, CancellationToken cancellationToken);
    Task<Result<RefreshToken>> UpdateAsync(RefreshToken token, CancellationToken cancellationToken);
}
