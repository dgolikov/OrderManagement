using OrderManagement.Domain.Tokens;
using OrderManagement.Models.Responses;

namespace OrderManagement.Models.Mappers.Authentication;

public static class TokenPairMapper
{
    public static TokenPairResponseModel Map(TokenPair pair) => new(pair.AccessToken, pair.RefreshToken);
}
