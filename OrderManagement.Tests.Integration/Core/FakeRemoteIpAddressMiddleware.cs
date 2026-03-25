using Microsoft.AspNetCore.Http;
using System.Net;

namespace OrderManagement.Tests.Integration.Core;

internal class FakeRemoteIpAddressMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IPAddress _fakeIpAddress = IPAddress.Parse("123.123.1.1");

    public FakeRemoteIpAddressMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Connection.RemoteIpAddress = _fakeIpAddress;
        await _next(httpContext);
    }
}
