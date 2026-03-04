using OrderManagement.Domain.Common;
using OrderManagement.Domain.Product;

namespace OrderManagement.Application.Products;

public interface IProductService
{
    Task<Page<Product>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken);
    Task<Result<Product>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<Result<Product>> UpdateAsync(Guid id, CreateProductRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
