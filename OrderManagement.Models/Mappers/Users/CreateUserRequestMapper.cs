using OrderManagement.Application.Users;
using OrderManagement.Models.Requests;

namespace OrderManagement.Models.Mappers.Users;

public static class CreateUserRequestMapper
{
    public static CreateUserRequest Map(CreateUserRequestModel model) => new(model.FirstName, model.LastName, model.Email, model.Password);
}
