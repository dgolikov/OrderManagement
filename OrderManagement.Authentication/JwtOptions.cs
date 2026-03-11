namespace OrderManagement.Authentication;

public class JwtOptions
{
    public required string JwtSecret { get; init; }
    public required string JwtIssuer { get; init; }
    public required string JwtAudience { get; init; }
    public required int AccessTokenTimeToLiveInMinutes { get; init; }
    public required int RefreshTokenTimeToLiveInDays { get; init; }
}