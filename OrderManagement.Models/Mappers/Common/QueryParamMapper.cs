using OrderManagement.Domain.Common;
using OrderManagement.Models.Responses.Common;

namespace OrderManagement.Models.Mappers.Common;

public static class QueryParamMapper
{
    public static QueryParams Map(QueryParamModel queryParams) => new(queryParams.PageNumber, queryParams.PageSize);
}
