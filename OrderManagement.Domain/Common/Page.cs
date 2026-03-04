namespace OrderManagement.Domain.Common;

public sealed record Page<TEntity>(int PageNumber, int PageSize, long Total, IReadOnlyCollection<TEntity> Items);
