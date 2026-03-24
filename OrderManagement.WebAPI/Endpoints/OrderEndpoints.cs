using OrderManagement.Application.Orders;
using OrderManagement.Models.Mappers.Common;
using OrderManagement.Models.Mappers.Orders;
using OrderManagement.Models.Requests;
using OrderManagement.Models.Responses;
using OrderManagement.Models.Responses.Common;
using OrderManagement.WebAPI.Extensions;

namespace OrderManagement.WebAPI.Endpoints;

public static class OrderEndpoints
{
    private const string _baseRoute = "/api/orders";

    public static void MapOrderEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(_baseRoute, async ([AsParameters] QueryParamModel queryParams, IOrderService orderService, CancellationToken cancellationToken) =>
        {
            var query = QueryParamMapper.Map(queryParams);
            var result = await orderService.GetPageAsync(query, cancellationToken);
            return result.ToApiResponse(OrderMapper.Map);
        })
        .WithTags("Orders")
        .RequireAuthorization();

        builder.MapGet(_baseRoute + "/{id:guid}", async (Guid id, IOrderService orderService, CancellationToken cancellationToken) =>
        {
            var result = await orderService.GetByIdAsync(id, cancellationToken);
            return result.ToApiResponse(OrderMapper.Map);
        })
        .WithTags("Orders")
        .RequireAuthorization();

        builder.MapPost(_baseRoute, async ([AsParameters] CreateOrderRequestModel requestModel, IOrderService orderService, CancellationToken cancellationToken) =>
        {
            var request = CreateOrderRequestMapper.Map(requestModel);
            var result = await orderService.CreateAsync(request, cancellationToken);
            return result.ToApiResponse(OrderMapper.Map);
        })
        .WithTags("Orders")
        .RequireAuthorization();

        builder.MapPut(_baseRoute + "/{id:guid}", async (Guid id, [AsParameters] CreateOrderRequestModel requestModel, IOrderService orderService, CancellationToken cancellationToken) =>
        {
            var request = CreateOrderRequestMapper.Map(requestModel);
            var result = await orderService.UpdateAsync(id, request, cancellationToken);
            return result.ToApiResponse(OrderMapper.Map);
        })
        .WithTags("Orders")
        .RequireAuthorization();

        builder.MapDelete(_baseRoute + "/{id:guid}", async (Guid id, IOrderService orderService, CancellationToken cancellationToken) =>
        {
            var result = await orderService.DeleteAsync(id, cancellationToken);
            return result.ToApiResponse();
        })
        .WithTags("Orders")
        .RequireAuthorization();
    }
}
