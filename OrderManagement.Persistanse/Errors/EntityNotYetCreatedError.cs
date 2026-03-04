using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Persistanse.Errors;

public sealed record EntityNotYetCreatedError(Guid Id, string Type) : Error($"Entity {Type} with {Id} not yet created");
