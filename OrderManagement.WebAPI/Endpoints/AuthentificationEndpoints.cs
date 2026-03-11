using OrderManagement.Application.Authentication;
using OrderManagement.Domain.Tokens;
using OrderManagement.Models.Mappers.Authentication;
using OrderManagement.Models.Requests;
using OrderManagement.WebAPI.Endpoints.RequestData;
using OrderManagement.WebAPI.Extensions;

namespace OrderManagement.WebAPI.Endpoints;

public static class AuthentificationEndpoints
{
    private const string _baseRoute = "/api/auth";

    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(_baseRoute + "/login", async (LoginRequestModel model, DeviceInfoRequestData deviceInfoRequestData, IAuthenticationService authenticationService, CancellationToken cancellationToken) =>
        {
            var request = LoginRequestMapper.Map(model);
            var info = new DeviceInfo(deviceInfoRequestData.IP, deviceInfoRequestData.UserAgent);
            var result = await authenticationService.LoginAsync(request, info, cancellationToken);
            return result.ToApiResponse(TokenPairMapper.Map);
        })
        .WithTags("Authentication")
        .AllowAnonymous();

        builder.MapPost(_baseRoute + "/logout", async (AuthenticationRequestData data, DeviceInfoRequestData deviceInfoRequestData, IAuthenticationService authenticationService, CancellationToken cancellationToken) =>
        {
            var info = new DeviceInfo(deviceInfoRequestData.IP, deviceInfoRequestData.UserAgent);
            var result = await authenticationService.LogoutAsync(data.RefreshTokenId, info, cancellationToken);
            return result.ToApiResponse();
        })
        .WithTags("Authentication")
        .RequireAuthorization();

        builder.MapPost(_baseRoute + "/refresh", async (RefreshTokenRequestModel model, DeviceInfoRequestData deviceInfoRequestData, IAuthenticationService authenticationService, CancellationToken cancellationToken) =>
        {
            var info = new DeviceInfo(deviceInfoRequestData.IP, deviceInfoRequestData.UserAgent);
            var result = await authenticationService.RefreshTokensAsync(model.Token, info, cancellationToken);
            return result.ToApiResponse(TokenPairMapper.Map);
        })
        .WithTags("Authentication")
        .RequireAuthorization();
    }
}
