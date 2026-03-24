using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderManagement.Authentication;

public sealed class JwtFactory : IJwtFactory
{
    private readonly IHashService _hashService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly JwtOptions _options;

    public JwtFactory(IHashService hashService, IDateTimeProvider dateTimeProvider, IOptions<JwtOptions> jwtOptions)
    {
        _hashService = hashService;
        _dateTimeProvider = dateTimeProvider;
        _options = jwtOptions.Value;
    }

    public Result<TokenBundle> CreateTokenBundle(Guid userId, DeviceInfo deviceInfo)
    {
        var token = _hashService.GenerateToken();
        var hash = _hashService.CalculateTokenHash(token);


        var refreshTokenExpiresAt = _dateTimeProvider.GetUtcNow().AddDays(_options.RefreshTokenTimeToLiveInDays);
        var refreshToken = RefreshToken.Create(userId, hash, refreshTokenExpiresAt, deviceInfo);

        if (refreshToken.IsFailure)
        {
            return Result.Failure<TokenBundle>(refreshToken.Error);
        }

        var accessTokenExpiresAt = _dateTimeProvider.GetUtcNow().AddDays(_options.RefreshTokenTimeToLiveInDays);
        var accessToken = CreateAccessToken(userId, refreshToken.Value.Id, accessTokenExpiresAt);

        var pair = new TokenPair(accessToken, token);
        return new TokenBundle(pair, refreshToken.Value);
    }

    private string CreateAccessToken(Guid userId, Guid refreshTokenId, DateTime expiresAt)
    {
        var identity = new ClaimsIdentity([
            new("userId", userId.ToString()),
            new("sessionId", refreshTokenId.ToString())
        ]);

        var bytes = Encoding.UTF8.GetBytes(_options.JwtSecret);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = identity,
            Issuer = _options.JwtIssuer,
            Audience = _options.JwtAudience,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(bytes), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescription);

        return tokenHandler.WriteToken(token);

    }
}
