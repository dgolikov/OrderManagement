namespace OrderManagement.Domain.Order;

public sealed record OrderLineItemView(
    Guid ProductId,
    int Quantity,
    decimal Price,
    string ProductName,
    string? ProductImageUrl)
{
    public decimal LineTotal => Quantity * Price;
}
