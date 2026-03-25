namespace OrderManagement.Caching;

public static class CacheKeyBuilder
{
    public static string ForEntity<TEntity>(Guid id) => $"{typeof(TEntity).Name.ToLower()}:{id}";
    public static string ForEntity<TEntity>(string suffix) => $"{typeof(TEntity).Name.ToLower()}:{suffix}";
    public static string ForPage<TEntity>(int pageNumber, int pageSize, string? searchTerm = null)
        => string.IsNullOrWhiteSpace(searchTerm)
            ? $"{typeof(TEntity).Name.ToLower()}:page:{pageNumber}:{pageSize}"
            : $"{typeof(TEntity).Name.ToLower()}:page:{pageNumber}:{pageSize}:{searchTerm.ToLower()}";
}
