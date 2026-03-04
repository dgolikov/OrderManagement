using OrderManagement.Domain.Common;
using OrderManagement.Models.Mappers.Common;

namespace OrderManagement.WebAPI.Extensions;

public static class PageExtensions
{
    public static IResult ToApiResponse<TValue, TModel>(this Page<TValue> page, Func<TValue, TModel> convertFunc)
    {
        return Results.Ok(PageMapper.Map(page, convertFunc));
    }
}
