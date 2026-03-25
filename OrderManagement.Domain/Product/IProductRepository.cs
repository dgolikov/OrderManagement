using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Product;

public interface IProductRepository
{
    Task<Result<Product>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyDictionary<Guid, Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    Task<Page<Product>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken);
    Task<Result<Product>> CreateAsync(Product product, CancellationToken cancellationToken);
    Task<Result<Product>> UpdateAsync(Product product, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
