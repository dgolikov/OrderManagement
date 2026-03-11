using MongoDB.Driver;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.Product;

namespace OrderManagement.Persistence;

public sealed class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(IMongoDatabase database, IDateTimeProvider dateTimeProvider) : base(database, dateTimeProvider, "products")
    {
    }
}
