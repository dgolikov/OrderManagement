using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Order;

public interface IOrderRepository
{
    Task<Page<Order>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken);
    Task<Result<Order>> CreateAsync(Order product, CancellationToken cancellationToken);
    Task<Result<Order>> UpdateAsync(Order product, CancellationToken cancellationToken);
}
