namespace OrderManagement.Domain.Tokens;

public sealed record DeviceInfo
{
    public string IP { get; init; }
    public string UserAgent { get; init; }

    public DeviceInfo(string? ip, string? userAgent)
    {
        IP = string.IsNullOrWhiteSpace(ip) ? "Unknown" : ip;
        UserAgent = string.IsNullOrWhiteSpace(ip) ? "Unknown" : ip;
    }
}
