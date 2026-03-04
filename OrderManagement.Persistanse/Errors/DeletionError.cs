using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Persistanse.Errors;

public sealed record DeletionError(Guid Id) : Error($"Unable to delete entity with id {Id}");
