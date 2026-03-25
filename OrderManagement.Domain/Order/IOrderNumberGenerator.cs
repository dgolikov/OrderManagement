namespace OrderManagement.Domain.Order;

public interface IOrderNumberGenerator
{
    Task<long> GenerateNextOrderNumberAsync(CancellationToken cancellationToken);
}
