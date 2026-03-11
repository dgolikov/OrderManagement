using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Tokens;

public interface IJwtFactory
{
    Result<TokenBundle> CreateTokenBundle(Guid userId, DeviceInfo deviceInfo);
}
