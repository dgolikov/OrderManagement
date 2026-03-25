namespace OrderManagement.Tests.Integration.Extensions;

internal static class HttpRequestMessageExtensions
{
    internal static void Authorize(this HttpRequestMessage message, string token)
    {
        message.Headers.Add("Authorization", $"Bearer {token}");
    }
}
