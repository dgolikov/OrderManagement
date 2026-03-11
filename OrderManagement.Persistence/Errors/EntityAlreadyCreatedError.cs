using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Persistence.Errors;

public sealed record EntityAlreadyCreatedError(Guid Id, string Type) : Error($"Entity {Type} with {Id} already created");

