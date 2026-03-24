namespace OrderManagement.Models.Responses;

public sealed record ProductResponseModel(Guid Id, string Name, decimal Price, string? ImageUrl);
