using MongoDB.Driver;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.Order;

namespace OrderManagement.Persistence;

public sealed class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(IMongoDatabase database, IDateTimeProvider dateTimeProvider) : base(database, dateTimeProvider, "orders")
    {
    }
}
