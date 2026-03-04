namespace OrderManagement.Domain.Common.Errors;

public sealed record PropertyIsRequiredError(string PropertyName) : Error($"{PropertyName} is required");
