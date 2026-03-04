namespace OrderManagement.Domain.Common.Errors;

public sealed record EntityNotFoundError(string Type) : Error($"Entity {Type} not found");
