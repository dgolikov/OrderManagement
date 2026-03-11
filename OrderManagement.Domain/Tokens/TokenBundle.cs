namespace OrderManagement.Domain.Tokens;

public sealed record TokenBundle(TokenPair Pair, RefreshToken RefreshToken);
