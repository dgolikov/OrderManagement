using OrderManagement.Domain.Common;
using OrderManagement.Domain.Order;

namespace OrderManagement.Application.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<Order>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _orderRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Page<Order>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken)
    {
        return _orderRepository.GetPageAsync(queryParams, cancellationToken);
    }

    public async Task<Result<Order>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var lineItems = request.LineItems.Select(li => new OrderLineItem(li.ProductId, li.Quantity, li.Price));

        var result = Order.Create(request.UserId, lineItems);

        if (result.IsFailure)
        {
            return result;
        }

        return await _orderRepository.CreateAsync(result.Value, cancellationToken);
    }

    public async Task<Result<Order>> UpdateAsync(Guid id, CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        var order = result.Value;

        var lineItems = request.LineItems.Select(li => new OrderLineItem(li.ProductId, li.Quantity, li.Price));
        var updateResult = order.UpdateLineItems(lineItems);

        if (updateResult.IsFailure)
        {
            return Result.Failure<Order>(updateResult.Error);
        }

        order.SetStatus(OrderStatus.Approved);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        return order;
    }

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return _orderRepository.DeleteAsync(id, cancellationToken);
    }
}
