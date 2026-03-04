namespace OrderManagement.Models.Responses;

public sealed record UserResponseModel(Guid Id, string FirstName, string LastName, string Email);
