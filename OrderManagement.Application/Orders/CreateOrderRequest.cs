namespace OrderManagement.Application.Orders;

public sealed record CreateOrderRequest(Guid UserId, IEnumerable<CreateOrderLineItemRequest> LineItems);

public sealed record CreateOrderLineItemRequest(Guid ProductId, int Quantity, decimal Price);
