using OrderManagement.Domain.User;
using OrderManagement.Models.Responses;

namespace OrderManagement.Models.Mappers.Users;

public static class UserMapper
{
    public static UserResponseModel Map(User user) => new(user.Id, user.FirstName, user.LastName, user.Email);
}
