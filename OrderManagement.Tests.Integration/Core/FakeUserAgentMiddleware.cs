using Microsoft.AspNetCore.Http;

namespace OrderManagement.Tests.Integration.Core;

internal class FakeUserAgentMiddleware
{
    private readonly RequestDelegate _next;
    private const string FakeUserAgent = "TestUserAgent/1.0";

    public FakeUserAgentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Request.Headers.UserAgent = FakeUserAgent;
        await _next(httpContext);
    }
}
