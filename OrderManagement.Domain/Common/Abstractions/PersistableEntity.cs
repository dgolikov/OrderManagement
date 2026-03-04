namespace OrderManagement.Domain.Common.Abstractions;

public abstract class PersistableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime? CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    public void PerformCreationAudit(DateTime dateTime)
    {
        CreatedOn = dateTime;
        ModifiedOn = dateTime;
    }

    public void PerformModificationAudit(DateTime dateTime)
    {
        ModifiedOn = dateTime;
    }
}
