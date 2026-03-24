namespace OrderManagement.Models.Responses;

public sealed record OrderResponseModel(
    Guid Id,
    Guid UserId,
    string Status,
    decimal Total,
    IReadOnlyCollection<OrderLineItemResponseModel> LineItems);
