using OrderManagement.Application.Orders;
using OrderManagement.Models.Requests;

namespace OrderManagement.Models.Mappers.Orders;

public static class CreateOrderRequestMapper
{
    public static CreateOrderRequest Map(CreateOrderRequestModel model) => new(
        model.UserId,
        model.LineItems.Select(li => new CreateOrderLineItemRequest(li.ProductId, li.Quantity, li.Price)));
}
