using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Domain.Product.Errors;

public sealed record InvalidPriceValueError() : Error($"Product price must be between 1 and 999999");
