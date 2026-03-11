using System.Reflection;

namespace OrderManagement.WebAPI.Endpoints.RequestData;

public sealed record DeviceInfoRequestData(string? IP, string? UserAgent)
{
    public static ValueTask<DeviceInfoRequestData> BindAsync(HttpContext context, ParameterInfo info)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();
        string? userAgent = null;
        if (context.Request.Headers.TryGetValue("User-Agent", out var agentValue))
        {
            userAgent = agentValue.FirstOrDefault();
        }
        return ValueTask.FromResult(new DeviceInfoRequestData(ip, userAgent));
    }
}
