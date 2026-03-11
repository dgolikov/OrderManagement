using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Domain.User.Errors;

public sealed record EmailOrPasswordMismatchError() : Error($"Email or password mismatch");
