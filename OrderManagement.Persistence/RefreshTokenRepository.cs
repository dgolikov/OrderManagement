using MongoDB.Driver;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Errors;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.Tokens;

namespace OrderManagement.Persistence;

public sealed class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IMongoDatabase database, IDateTimeProvider dateTimeProvider) : base(database, dateTimeProvider, "refresh-tokens")
    {
    }

    public async Task<Result<RefreshToken>> GetByTokenHashAsync(string hash, CancellationToken cancellationToken)
    {
        var token = await Collection.Find(x => x.TokenHash == hash).FirstOrDefaultAsync(cancellationToken);
        if (token is null)
        {
            return Result.Failure<RefreshToken>(new EntityNotFoundError(typeof(RefreshToken).Name));
        }

        return token;
    }
}
