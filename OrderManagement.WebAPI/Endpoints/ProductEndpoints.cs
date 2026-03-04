using OrderManagement.Application.Products;
using OrderManagement.Models.Mappers.Common;
using OrderManagement.Models.Mappers.Products;
using OrderManagement.Models.Requests;
using OrderManagement.Models.Responses.Common;
using OrderManagement.WebAPI.Extensions;

namespace OrderManagement.WebAPI.Endpoints;

public static class ProductEndpoints
{
    private const string _baseRoute = "/api/products";

    public static void MapProductEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(_baseRoute, async ([AsParameters]QueryParamModel queryParams, IProductService productService, CancellationToken cancellationToken) =>
        {
            var query = QueryParamMapper.Map(queryParams);
            var result = await productService.GetPageAsync(query, cancellationToken);
            return result.ToApiResponse(ProductMapper.Map);
        }).WithTags("Products");

        builder.MapPost(_baseRoute, async ([AsParameters] CreateProductRequestModel requestModel, IProductService productService, CancellationToken cancellationToken) =>
        {
            var request = CreateProductRequestMapper.Map(requestModel);
            var result = await productService.CreateAsync(request, cancellationToken);
            return result.ToApiResponse(ProductMapper.Map);
        }).WithTags("Products");

        builder.MapPut(_baseRoute + "/{id:guid}", async (Guid id, [AsParameters] CreateProductRequestModel requestModel, IProductService productService, CancellationToken cancellationToken) =>
        {
            var request = CreateProductRequestMapper.Map(requestModel);
            var result = await productService.UpdateAsync(id, request, cancellationToken);
            return result.ToApiResponse(ProductMapper.Map);
        }).WithTags("Products");

        builder.MapDelete(_baseRoute + "/{id:guid}", async (Guid id, IProductService productService, CancellationToken cancellationToken) =>
        {
            var result = await productService.DeleteAsync(id, cancellationToken);
            return result.ToApiResponse();
        }).WithTags("Products");
    }
}