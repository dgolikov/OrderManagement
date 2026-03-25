using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.User;
using OrderManagement.Models.Requests;
using OrderManagement.Tests.Integration.Core;

namespace OrderManagement.Tests.Integration.EndpointTests;

public class IntegrationTestBase : IAsyncLifetime
{
    private readonly OrderManagementWebApplicationFactory _appFactory;
    protected readonly HttpClient HttpClient;

    public string TestUserEmail { get; init; } = "testemail@domain.com";
    public string TestUserPassword { get; init; } = "12345678";
    public Guid TestUserId { get; private set; }

    protected readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public IntegrationTestBase(OrderManagementWebApplicationFactory appFactory)
    {
        _appFactory = appFactory;
        HttpClient = _appFactory.HttpClient;
    }

    protected async Task ConfigureDatabase(Func<OrderManagementWebApplicationFactory, Task> config) =>
        await config.Invoke(_appFactory);

    public Task DisposeAsync() => _appFactory.ResetDatabaseAsync();

    public async Task InitializeAsync()
    {
        using var scope = _appFactory.Services.CreateScope();
        var hashService = scope.ServiceProvider.GetRequiredService<IHashService>();

        var salt = hashService.GenerateSalt();
        var passwordHash = hashService.CalculatePasswordHash(TestUserPassword, salt);

        var result = User.Create("Test", "User", TestUserEmail, passwordHash, salt);

        var testUser = result.Value;
        TestUserId = testUser.Id;

        await _appFactory.FillCollection("users", [testUser]);
    }

    public async Task<string> LoginAsync(string? email = null, string? password = null)
    {
        var endpointUrl = "api/auth/login";
        var loginRequest = new LoginRequestModel(email ?? TestUserEmail, password ?? TestUserPassword);

        var requestBody = JsonSerializer.Serialize(loginRequest, JsonOptions);
        var httpContent = new StringContent(requestBody, null, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
        request.Content = httpContent;

        var response = await HttpClient.SendAsync(request);

        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        var accessToken = doc.RootElement
            .GetProperty("accessToken")
            .GetString();

        return accessToken ?? throw new Exception("Unable to login");
    }
}
