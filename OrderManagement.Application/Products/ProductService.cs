using OrderManagement.Domain.Common;
using OrderManagement.Domain.Product;

namespace OrderManagement.Application.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<Product>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = Product.Create(request.Name, request.Price, request.SKU);

        if (result.IsFailure)
        {
            return result;
        }

        return await _productRepository.CreateAsync(result.Value, cancellationToken);
    }

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return _productRepository.DeleteAsync(id, cancellationToken);
    }

    public Task<Page<Product>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken)
    {
        return _productRepository.GetPageAsync(queryParams, cancellationToken);
    }

    public async Task<Result<Product>> UpdateAsync(Guid id, CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        var product = result.Value;

        var updateNameResult = product.SetName(request.Name);
        if (updateNameResult.IsFailure)
        {
            return Result.Failure<Product>(updateNameResult.Error);
        }

        var updatePriceResult = product.SetPrice(request.Price);
        if (updatePriceResult.IsFailure)
        {
            return Result.Failure<Product>(updatePriceResult.Error);
        }

        var updateSkuResult = product.SetSKU(request.SKU);
        if (updateSkuResult.IsFailure)
        {
            return Result.Failure<Product>(updateSkuResult.Error);
        }

        await _productRepository.UpdateAsync(product, cancellationToken);
        return product;
    }
}
