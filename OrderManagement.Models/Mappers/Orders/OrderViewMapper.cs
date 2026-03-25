using OrderManagement.Domain.Order;
using OrderManagement.Models.Responses;

namespace OrderManagement.Models.Mappers.Orders;

public static class OrderViewMapper
{
    public static OrderResponseModel Map(OrderView orderView) => new(
        orderView.Id,
        orderView.OrderNumber,
        orderView.UserId,
        orderView.Status,
        orderView.Total,
        orderView.LineItems.Select(MapLineItem).ToList());

    public static IReadOnlyCollection<OrderResponseModel> MapCollection(
        IEnumerable<OrderView> orderViews)
        => orderViews.Select(Map).ToList();

    private static OrderLineItemResponseModel MapLineItem(OrderLineItemView lineItem) => new(
        lineItem.ProductId,
        lineItem.Quantity,
        lineItem.Price,
        lineItem.ProductName,
        lineItem.ProductImageUrl);
}
