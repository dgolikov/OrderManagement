using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Abstractions;
using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Domain.User;

public sealed class User : PersistableEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }

    private User(string firstName, string lastName, string email, string passwordHash, string salt)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Salt = salt;
    }

    public static Result<User> Create(string firstName, string lastName, string email, string passwordHash, string salt)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<User>(new PropertyIsRequiredError(nameof(email)));
        }

        return new User(firstName, lastName, email, passwordHash, salt);
    }
}
