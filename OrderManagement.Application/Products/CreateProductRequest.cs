namespace OrderManagement.Application.Products;

public sealed record CreateProductRequest(string Name, decimal Price, string SKU, string? ImageUrl = null);
