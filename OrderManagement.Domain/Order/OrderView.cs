using ProductEntity = OrderManagement.Domain.Product.Product;

namespace OrderManagement.Domain.Order;

public sealed class OrderView
{
    public Guid Id { get; }
    public long OrderNumber { get; }
    public Guid UserId { get; }
    public string Status { get; }
    public decimal Total { get; }
    public IReadOnlyCollection<OrderLineItemView> LineItems { get; }

    private OrderView(
        Guid id,
        long orderNumber,
        Guid userId,
        string status,
        decimal total,
        IReadOnlyCollection<OrderLineItemView> lineItems)
    {
        Id = id;
        OrderNumber = orderNumber;
        UserId = userId;
        Status = status;
        Total = total;
        LineItems = lineItems;
    }

    public static OrderView Create(Order order, IReadOnlyDictionary<Guid, ProductEntity> products)
    {
        var lineItemViews = order.LineItems
            .Select(li => CreateLineItemView(li, products))
            .ToList();

        return new OrderView(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.Status.ToString(),
            order.Total,
            lineItemViews);
    }

    private static OrderLineItemView CreateLineItemView(
        OrderLineItem lineItem,
        IReadOnlyDictionary<Guid, ProductEntity> products)
    {
        var product = products.GetValueOrDefault(lineItem.ProductId);

        return new OrderLineItemView(
            lineItem.ProductId,
            lineItem.Quantity,
            lineItem.Price,
            product?.Name ?? "Product no longer available",
            product?.ImageUrl);
    }

    public static IReadOnlyCollection<OrderView> CreateCollection(
        IEnumerable<Order> orders,
        IReadOnlyDictionary<Guid, ProductEntity> products)
        => orders.Select(o => Create(o, products)).ToList();
}
