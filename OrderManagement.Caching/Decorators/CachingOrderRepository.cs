using OrderManagement.Domain.Common;
using OrderManagement.Domain.Order;

namespace OrderManagement.Caching.Decorators;

public sealed class CachingOrderRepository : IOrderRepository
{
    private readonly IOrderRepository _inner;
    private readonly ICacheService _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(2);

    public CachingOrderRepository(IOrderRepository inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Result<Order>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = CacheKeyBuilder.ForEntity<Order>(id);
        var cached = await _cache.GetAsync<Order>(key, cancellationToken);

        if (cached is not null)
            return cached;

        var result = await _inner.GetByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
            await _cache.SetAsync(key, result.Value, _cacheDuration, cancellationToken);

        return result;
    }

    public async Task<Page<Order>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken)
    {
        var key = CacheKeyBuilder.ForPage<Order>(queryParams.PageNumber, queryParams.PageSize, queryParams.SearchTerm);
        var cached = await _cache.GetAsync<Page<Order>>(key, cancellationToken);

        if (cached is not null)
            return cached;

        var result = await _inner.GetPageAsync(queryParams, cancellationToken);

        await _cache.SetAsync(key, result, _cacheDuration, cancellationToken);

        foreach (var item in result.Items)
        {
            var itemKey = CacheKeyBuilder.ForEntity<Order>(item.Id);
            await _cache.SetAsync(itemKey, item, _cacheDuration, cancellationToken);
        }

        return result;
    }

    public async Task<Result<Order>> CreateAsync(Order order, CancellationToken cancellationToken)
    {
        var result = await _inner.CreateAsync(order, cancellationToken);

        if (result.IsSuccess)
        {
            var key = CacheKeyBuilder.ForEntity<Order>(result.Value.Id);
            await _cache.SetAsync(key, result.Value, _cacheDuration, cancellationToken);
        }

        return result;
    }

    public async Task<Result<Order>> UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        var result = await _inner.UpdateAsync(order, cancellationToken);

        if (result.IsSuccess)
        {
            var key = CacheKeyBuilder.ForEntity<Order>(order.Id);
            await _cache.RemoveAsync(key, cancellationToken);
        }

        return result;
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inner.DeleteAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            var key = CacheKeyBuilder.ForEntity<Order>(id);
            await _cache.RemoveAsync(key, cancellationToken);
        }

        return result;
    }
}
