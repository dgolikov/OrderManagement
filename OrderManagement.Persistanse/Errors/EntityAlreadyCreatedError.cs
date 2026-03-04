using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Persistanse.Errors;

public sealed record EntityAlreadyCreatedError(Guid Id, string Type) : Error($"Entity {Type} with {Id} already created");

