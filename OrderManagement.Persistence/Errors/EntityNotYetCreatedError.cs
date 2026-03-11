using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Persistence.Errors;

public sealed record EntityNotYetCreatedError(Guid Id, string Type) : Error($"Entity {Type} with {Id} not yet created");
