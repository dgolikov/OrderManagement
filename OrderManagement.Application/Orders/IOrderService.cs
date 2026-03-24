using OrderManagement.Domain.Common;
using OrderManagement.Domain.Order;

namespace OrderManagement.Application.Orders;

public interface IOrderService
{
    Task<Result<Order>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Page<Order>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken);
    Task<Result<Order>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken);
    Task<Result<Order>> UpdateAsync(Guid id, CreateOrderRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
