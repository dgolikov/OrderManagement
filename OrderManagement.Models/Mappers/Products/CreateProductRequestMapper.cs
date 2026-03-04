using OrderManagement.Application.Products;
using OrderManagement.Models.Requests;

namespace OrderManagement.Models.Mappers.Products;

public static class CreateProductRequestMapper
{
    public static CreateProductRequest Map(CreateProductRequestModel model) => new(model.Name, model.Price, model.SKU);
}
