using OrderManagement.Application.Users;
using OrderManagement.Models.Mappers.Users;
using OrderManagement.Models.Requests;
using OrderManagement.WebAPI.Extensions;

namespace OrderManagement.WebAPI.Endpoints;

public static class UserEndpoints
{
    private const string _baseRoute = "/api/users";

    public static void MapUserEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(_baseRoute, async ([AsParameters] CreateUserRequestModel requestModel, IUserService userService, CancellationToken cancellationToken) =>
        {
            var request = CreateUserRequestMapper.Map(requestModel);
            var result = await userService.CreateAsync(request, cancellationToken);
            return result.ToApiResponse(UserMapper.Map);
        })
        .WithTags("Users")
        .AllowAnonymous();
    }
}
