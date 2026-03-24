using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Order;

public interface IOrderRepository
{
    Task<Result<Order>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Page<Order>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken);
    Task<Result<Order>> CreateAsync(Order order, CancellationToken cancellationToken);
    Task<Result<Order>> UpdateAsync(Order order, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
