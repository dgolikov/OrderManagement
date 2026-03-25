using OrderManagement.Domain.Order;
using OrderManagement.Models.Responses;

namespace OrderManagement.Models.Mappers.Orders;

public static class OrderMapper
{
    public static OrderResponseModel Map(Order order) => new(
        order.Id,
        order.OrderNumber,
        order.UserId,
        order.Status.ToString(),
        order.Total,
        order.LineItems.Select(li => new OrderLineItemResponseModel(
            li.ProductId,
            li.Quantity,
            li.Price,
            string.Empty,
            null)).ToList());
}
