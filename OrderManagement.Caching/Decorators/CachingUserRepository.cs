using OrderManagement.Domain.Common;
using OrderManagement.Domain.User;

namespace OrderManagement.Caching.Decorators;

public sealed class CachingUserRepository : IUserRepository
{
    private readonly IUserRepository _inner;
    private readonly ICacheService _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(30);

    public CachingUserRepository(IUserRepository inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = CacheKeyBuilder.ForEntity<User>(id);
        var cached = await _cache.GetAsync<User>(key, cancellationToken);

        if (cached is not null)
            return cached;

        var result = await _inner.GetByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
            await _cache.SetAsync(key, result.Value, _cacheDuration, cancellationToken);

        return result;
    }

    public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var key = CacheKeyBuilder.ForEntity<User>($"email:{email.ToLower()}");
        var cached = await _cache.GetAsync<User>(key, cancellationToken);

        if (cached is not null)
            return cached;

        var result = await _inner.GetByEmailAsync(email, cancellationToken);

        if (result.IsSuccess)
            await _cache.SetAsync(key, result.Value, _cacheDuration, cancellationToken);

        return result;
    }

    public async Task<Result<User>> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var result = await _inner.CreateAsync(user, cancellationToken);

        if (result.IsSuccess)
        {
            var key = CacheKeyBuilder.ForEntity<User>(result.Value.Id);
            await _cache.SetAsync(key, result.Value, _cacheDuration, cancellationToken);

            var emailKey = CacheKeyBuilder.ForEntity<User>($"email:{result.Value.Email.ToLower()}");
            await _cache.SetAsync(emailKey, result.Value, _cacheDuration, cancellationToken);
        }

        return result;
    }
}
