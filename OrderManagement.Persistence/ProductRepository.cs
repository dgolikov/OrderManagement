using MongoDB.Bson;
using MongoDB.Driver;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.Product;

namespace OrderManagement.Persistence;

public sealed class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(IMongoDatabase database, IDateTimeProvider dateTimeProvider) : base(database, dateTimeProvider, "products")
    {
    }

    protected override FilterDefinition<Product> BuildPageFilter(QueryParams queryParams)
    {
        if (string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            return Builders<Product>.Filter.Empty;
        }

        var regex = new BsonRegularExpression(queryParams.SearchTerm, "i");
        return Builders<Product>.Filter.Regex(p => p.Name, regex);
    }

    public async Task<IReadOnlyDictionary<Guid, Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var idList = ids.ToList();
        var products = await Collection.Find(p => idList.Contains(p.Id)).ToListAsync(cancellationToken);
        return products.ToDictionary(p => p.Id);
    }
}
