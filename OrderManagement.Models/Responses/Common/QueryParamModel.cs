namespace OrderManagement.Models.Responses.Common;

public sealed record QueryParamModel(int PageNumber, int PageSize, string? SearchTerm = null);
