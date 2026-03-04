namespace OrderManagement.Domain.Common.Services;

public interface IDateTimeProvider
{
    DateTime GetUtcNow();
}
