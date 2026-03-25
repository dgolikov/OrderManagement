namespace OrderManagement.Models.Responses;

public sealed record OrderLineItemResponseModel(
    Guid ProductId,
    int Quantity,
    decimal Price,
    string ProductName,
    string? ImageUrl);
