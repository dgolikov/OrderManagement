using MongoDB.Driver;
using OrderManagement.Domain.Order;

namespace OrderManagement.Persistence;

public class OrderNumberGenerator : IOrderNumberGenerator
{
    private readonly IMongoDatabase _database;
    private const string CountersCollectionName = "counters";
    private const string OrderNumberCounterId = "orderNumber";
    private const long StartingOrderNumber = 10000;

    public OrderNumberGenerator(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<long> GenerateNextOrderNumberAsync(CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<Counter>(CountersCollectionName);

        var filter = Builders<Counter>.Filter.Eq(c => c.Id, OrderNumberCounterId);
        var update = Builders<Counter>.Update.Inc(c => c.Sequence, 1);

        var options = new FindOneAndUpdateOptions<Counter>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var existingCounter = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (existingCounter is null)
        {
            await collection.InsertOneAsync(
                new Counter { Id = OrderNumberCounterId, Sequence = StartingOrderNumber - 1 },
                cancellationToken: cancellationToken);
        }

        var result = await collection.FindOneAndUpdateAsync(filter, update, options, cancellationToken);
        return result.Sequence;
    }
}
