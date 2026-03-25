using MongoDB.Driver;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Abstractions;
using OrderManagement.Domain.Common.Errors;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Persistence.Errors;

namespace OrderManagement.Persistence;

public abstract class BaseRepository<TEntity> where TEntity : PersistableEntity
{
    private readonly IMongoDatabase _database;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly string _collectionName;

    protected IMongoCollection<TEntity> Collection => _database.GetCollection<TEntity>(_collectionName);

    protected virtual FilterDefinition<TEntity> BuildPageFilter(QueryParams queryParams)
        => Builders<TEntity>.Filter.Empty;

    protected BaseRepository(IMongoDatabase database, IDateTimeProvider dateTimeProvider, string collectionName)
    {
        _database = database;
        _dateTimeProvider = dateTimeProvider;
        _collectionName = collectionName;
    }

    public async Task<Result<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var value = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        if (value is null)
        {
            return Result.Failure<TEntity>(new EntityNotFoundError(typeof(TEntity).Name));
        }
        return value;
    }

    public async Task<Page<TEntity>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken)
    {
        var filter = BuildPageFilter(queryParams);

        var pageTask = Collection
            .Find(filter)
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Limit(queryParams.PageSize)
            .ToListAsync(cancellationToken);

        var countTask = Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        await Task.WhenAll(pageTask, countTask);

        var items = await pageTask;
        var count = await countTask;

        return new Page<TEntity>(queryParams.PageNumber, queryParams.PageSize, count, items);
    }

    public async Task<Result<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity.CreatedOn is not null)
        {
            return Result.Failure<TEntity>(new EntityAlreadyCreatedError(entity.Id, entity.GetType().Name));
        }

        entity.PerformCreationAudit(_dateTimeProvider.GetUtcNow());

        var options = new InsertOneOptions();
        await Collection.InsertOneAsync(entity, options, cancellationToken);
        return entity;
    }

    public async Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity.CreatedOn is null)
        {
            return Result.Failure<TEntity>(new EntityNotYetCreatedError(entity.Id, entity.GetType().Name));
        }

        entity.PerformModificationAudit(_dateTimeProvider.GetUtcNow());

        await Collection.FindOneAndReplaceAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await Collection.DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken);
        if (result.DeletedCount == 0)
        {
            return Result.Failure(new DeletionError(id));
        }

        return Result.Succsess();
    }
}
