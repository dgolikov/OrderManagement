using OrderManagement.Tests.Integration.Core;

namespace OrderManagement.Tests.Integration;

[CollectionDefinition("Integration")]
public class SharedTestCollection : ICollectionFixture<OrderManagementWebApplicationFactory>
{
}
