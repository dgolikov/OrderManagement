using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Abstractions;

namespace OrderManagement.Domain.Order;

public sealed class Order : PersistableEntity
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set;  }


    private Order(Guid productId, Guid userId, OrderStatus status)
    {
        ProductId = productId;
        UserId = userId;
        Status = status;
    }

    public Result<Order> Create(Guid productId, Guid userId, OrderStatus status)
    {
        return new Order(productId, userId, status);
    }
}
