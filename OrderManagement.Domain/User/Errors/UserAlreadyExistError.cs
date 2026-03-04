using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Domain.User.Errors;

public sealed record UserAlreadyExistError(string Email) : Error($"User with email {Email} already exists in the system");
