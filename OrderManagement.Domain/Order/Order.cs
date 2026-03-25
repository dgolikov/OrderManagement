using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Abstractions;
using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Domain.Order;

public sealed class Order : PersistableEntity
{
    private List<OrderLineItem> _lineItems = [];

    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public List<OrderLineItem> LineItems
    {
        get => _lineItems?.ToList() ?? [];
        private set => _lineItems = value;
    }
    public decimal Total => LineItems.Sum(item => item.Quantity * item.Price);
    public long OrderNumber { get; private set; }

    private Order(Guid userId, OrderStatus status)
    {
        UserId = userId;
        Status = status;
    }

    public static Result<Order> Create(Guid userId, IEnumerable<OrderLineItem> lineItems)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure<Order>(new PropertyIsRequiredError(nameof(userId)));
        }

        var itemsList = lineItems.ToList();

        if (itemsList.Count == 0)
        {
            return Result.Failure<Order>(new PropertyIsRequiredError(nameof(lineItems)));
        }

        var order = new Order(userId, OrderStatus.Created);
        order._lineItems.AddRange(itemsList);

        return order;
    }

    public Result UpdateLineItems(IEnumerable<OrderLineItem> lineItems)
    {
        var itemsList = lineItems.ToList();

        if (itemsList.Count == 0)
        {
            return Result.Failure(new PropertyIsRequiredError(nameof(lineItems)));
        }

        _lineItems.Clear();
        _lineItems.AddRange(itemsList);

        return Result.Succsess();
    }

    public void SetStatus(OrderStatus status)
    {
        Status = status;
    }

    public void SetOrderNumber(long orderNumber)
    {
        OrderNumber = orderNumber;
    }
}
