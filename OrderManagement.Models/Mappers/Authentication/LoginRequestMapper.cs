using OrderManagement.Application.Authentication;
using OrderManagement.Models.Requests;

namespace OrderManagement.Models.Mappers.Authentication;

public static class LoginRequestMapper
{
    public static LoginRequest Map(LoginRequestModel model) => new LoginRequest(model.Email, model.Password);
}
