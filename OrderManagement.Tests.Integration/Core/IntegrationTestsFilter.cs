using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace OrderManagement.Tests.Integration.Core;

internal class IntegrationTestsFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseMiddleware<FakeRemoteIpAddressMiddleware>();
            app.UseMiddleware<FakeUserAgentMiddleware>();
            next(app);
        };
    }
}
