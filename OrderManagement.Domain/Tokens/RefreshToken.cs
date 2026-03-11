using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Abstractions;
using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Domain.Tokens;

public sealed class RefreshToken : PersistableEntity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsExpired(DateTime date) => date > ExpiresAt;
    public DeviceInfo DeviceInfo { get; private set; }
    public bool IsActive(DateTime date) => RevokedAt is null && !IsExpired(date);

    public DateTime? RevokedAt { get; private set;  }
    public string? RevokedByIp { get; private set;  }
    public string? ReplacedByToken { get; private set;  }

    private RefreshToken(Guid userId, string tokenHash, DateTime expiresAt, DeviceInfo deviceInfo)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        DeviceInfo = deviceInfo;
    }

    public static Result<RefreshToken> Create(Guid userId, string tokenHash, DateTime expiresAt, DeviceInfo deviceInfo)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure<RefreshToken>(new PropertyIsRequiredError(nameof(userId)));
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            return Result.Failure<RefreshToken>(new PropertyIsRequiredError(nameof(tokenHash)));
        }

        return new RefreshToken(userId, tokenHash, expiresAt, deviceInfo);
    }

    public void RevokeByToken(RefreshToken token, DateTime date)
    {
        RevokedAt = date;
        RevokedByIp = token.DeviceInfo.IP;
        ReplacedByToken = token.TokenHash;
    }

    public void RevokeByIp(string ip, DateTime date)
    {
        RevokedAt = date;
        RevokedByIp = ip;
    }
}
