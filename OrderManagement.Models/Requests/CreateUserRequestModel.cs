namespace OrderManagement.Models.Requests;

public sealed record CreateUserRequestModel(string FirstName, string LastName, string Email, string Password);
