namespace OrderManagement.Domain.Common.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetUtcNow() => DateTime.UtcNow;
}
