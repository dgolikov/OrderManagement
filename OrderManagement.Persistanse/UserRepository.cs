using MongoDB.Driver;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Errors;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.User;

namespace OrderManagement.Persistanse;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IMongoDatabase database, IDateTimeProvider dateTimeProvider) : base(database, dateTimeProvider, "users")
    {
    }

    public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await Collection.Find(x => x.Email == email).FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            return Result.Failure<User>(new EntityNotFoundError(typeof(User).Name));
        }

        return user;
    }
}
