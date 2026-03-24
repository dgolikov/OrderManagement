namespace OrderManagement.Domain.Order;

public sealed record OrderLineItem(Guid ProductId, int Quantity, decimal Price);
