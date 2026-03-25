using OrderManagement.Tests.Integration.Core;
using OrderManagement.Tests.Integration.Extensions;

namespace OrderManagement.Tests.Integration.EndpointTests.ProductTests;

[Collection("Integration")]
public class GetProductsTest : IntegrationTestBase
{
    public GetProductsTest(OrderManagementWebApplicationFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/products");

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ShouldReturnProducts_WhenAuthenticated()
    {
        // Arrange
        var token = await LoginAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/products?pageNumber=1&pageSize=10");
        request.Authorize(token);

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
