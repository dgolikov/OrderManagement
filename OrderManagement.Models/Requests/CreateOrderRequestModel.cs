namespace OrderManagement.Models.Requests;

public sealed record CreateOrderRequestModel(Guid UserId, IEnumerable<CreateOrderLineItemModel> LineItems);

public sealed record CreateOrderLineItemModel(Guid ProductId, int Quantity);
