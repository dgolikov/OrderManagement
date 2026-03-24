using OrderManagement.Domain.Product;
using OrderManagement.Models.Responses;

namespace OrderManagement.Models.Mappers.Products;

public static class ProductMapper
{
    public static ProductResponseModel Map(Product product) => new ProductResponseModel(product.Id, product.Name, product.Price, product.ImageUrl);
}
