using OrderManagement.Domain.Common;
using OrderManagement.Domain.Product;

namespace OrderManagement.Caching.Decorators;

public sealed class CachingProductRepository : IProductRepository
{
    private readonly IProductRepository _inner;
    private readonly ICacheService _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public CachingProductRepository(IProductRepository inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Result<Product>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = CacheKeyBuilder.ForEntity<Product>(id);
        var cached = await _cache.GetAsync<Product>(key, cancellationToken);

        if (cached is not null)
            return cached;

        var result = await _inner.GetByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
            await _cache.SetAsync(key, result.Value, _cacheDuration, cancellationToken);

        return result;
    }

    public async Task<IReadOnlyDictionary<Guid, Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var idList = ids.ToList();
        var result = new Dictionary<Guid, Product>();
        var uncachedIds = new List<Guid>();

        foreach (var id in idList)
        {
            var key = CacheKeyBuilder.ForEntity<Product>(id);
            var cached = await _cache.GetAsync<Product>(key, cancellationToken);

            if (cached is not null)
                result[id] = cached;
            else
                uncachedIds.Add(id);
        }

        if (uncachedIds.Count > 0)
        {
            var fromDb = await _inner.GetByIdsAsync(uncachedIds, cancellationToken);

            foreach (var kvp in fromDb)
            {
                result[kvp.Key] = kvp.Value;
                var key = CacheKeyBuilder.ForEntity<Product>(kvp.Key);
                await _cache.SetAsync(key, kvp.Value, _cacheDuration, cancellationToken);
            }
        }

        return result;
    }

    public async Task<Page<Product>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken)
    {
        var key = CacheKeyBuilder.ForPage<Product>(queryParams.PageNumber, queryParams.PageSize, queryParams.SearchTerm);
        var cached = await _cache.GetAsync<Page<Product>>(key, cancellationToken);

        if (cached is not null)
            return cached;

        var result = await _inner.GetPageAsync(queryParams, cancellationToken);

        await _cache.SetAsync(key, result, _cacheDuration, cancellationToken);

        foreach (var item in result.Items)
        {
            var itemKey = CacheKeyBuilder.ForEntity<Product>(item.Id);
            await _cache.SetAsync(itemKey, item, _cacheDuration, cancellationToken);
        }

        return result;
    }

    public async Task<Result<Product>> CreateAsync(Product product, CancellationToken cancellationToken)
    {
        var result = await _inner.CreateAsync(product, cancellationToken);

        if (result.IsSuccess)
        {
            var key = CacheKeyBuilder.ForEntity<Product>(result.Value.Id);
            await _cache.SetAsync(key, result.Value, _cacheDuration, cancellationToken);
        }

        return result;
    }

    public async Task<Result<Product>> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        var result = await _inner.UpdateAsync(product, cancellationToken);

        if (result.IsSuccess)
        {
            var key = CacheKeyBuilder.ForEntity<Product>(product.Id);
            await _cache.RemoveAsync(key, cancellationToken);
        }

        return result;
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inner.DeleteAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            var key = CacheKeyBuilder.ForEntity<Product>(id);
            await _cache.RemoveAsync(key, cancellationToken);
        }

        return result;
    }
}
