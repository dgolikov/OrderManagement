using System.Reflection;

namespace OrderManagement.WebAPI.Endpoints.RequestData;

public sealed record AuthenticationRequestData(Guid UserId, Guid RefreshTokenId)
{
    public static ValueTask<AuthenticationRequestData> BindAsync(HttpContext context, ParameterInfo info)
    {
        var identity = context.User.Identities.First();

        if (!Guid.TryParse(identity.Claims.First(c => c.Type == "userId").Value, out var userId))
        {
            throw new Exception("Invalid authentication token");
        }

        if (!Guid.TryParse(identity.Claims.First(c => c.Type == "sessionId").Value, out var refreshTokenId))
        {
            throw new Exception("Invalid authentication token");
        }

        return ValueTask.FromResult(new AuthenticationRequestData(userId, refreshTokenId));
    }
}
