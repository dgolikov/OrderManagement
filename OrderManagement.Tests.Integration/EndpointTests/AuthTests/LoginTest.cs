using OrderManagement.Tests.Integration.Core;
using System.Text.Json;

namespace OrderManagement.Tests.Integration.EndpointTests.AuthTests;

[Collection("Integration")]
public class LoginTest : IntegrationTestBase
{
    public LoginTest(OrderManagementWebApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task ShouldLoginSuccessfully_WhenValidCredentials()
    {
        // Arrange & Act
        var token = await LoginAsync();

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task ShouldFailLogin_WhenInvalidCredentials()
    {
        // Arrange
        var endpointUrl = "api/auth/login";
        var loginRequest = new { Email = "wrong@email.com", Password = "wrongpassword" };

        var requestBody = JsonSerializer.Serialize(loginRequest, JsonOptions);
        var httpContent = new StringContent(requestBody, null, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
        request.Content = httpContent;

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
