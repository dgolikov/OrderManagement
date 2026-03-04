using OrderManagement.Domain.Common;

namespace OrderManagement.WebAPI.Extensions;

public static class ResultExtenstions
{
    public static IResult ToApiResponse(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok();
        }
        else
        {
            return Results.BadRequest(result.Error?.Message);
        }
    }

    public static IResult ToApiResponse<TValue, TModel>(this Result<TValue> result, Func<TValue, TModel> convertFunc)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(convertFunc(result.Value));
        }
        else
        {
            return Results.BadRequest(result.Error?.Message);
        }
    }
}
