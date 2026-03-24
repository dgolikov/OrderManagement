namespace OrderManagement.Models.Requests;

public sealed record CreateProductRequestModel(string Name, decimal Price, string SKU, string? ImageUrl = null);