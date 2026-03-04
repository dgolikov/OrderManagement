namespace OrderManagement.Models.Responses.Common;

public sealed record PageModel<TEntity>(int PageNumber, int PageSize, long Total, IReadOnlyCollection<TEntity> Items);
