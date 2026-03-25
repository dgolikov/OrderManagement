namespace OrderManagement.Models.Responses;

public sealed record OrderResponseModel(
    Guid Id,
    long OrderNumber,
    Guid UserId,
    string Status,
    decimal Total,
    IReadOnlyCollection<OrderLineItemResponseModel> LineItems);
