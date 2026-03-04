using OrderManagement.Domain.Common.Errors;

namespace OrderManagement.Domain.Common;

public class Result
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; init; }

    protected Result(bool isSuccsess, Error? error)
    {
        if (isSuccsess && error is not null || !isSuccsess && error is null)
        {
            throw new ArgumentException("Invalid result state");
        }

        IsSuccess = isSuccsess;
        Error = error;
    }

    public static Result Succsess() => new(true, null);
    public static Result Failure(Error? error) => new(false, error);

    public static Result<TValue> Succsess<TValue>(TValue value) => new(value, true, null);
    public static Result<TValue> Failure<TValue>(Error? error) => new(default!, false, error);

}

public class Result<TValue> : Result
{
    private readonly TValue _value;

    protected internal Result(TValue value, bool isSuccsess, Error? error) : base(isSuccsess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Attempt to retrieve value from failured operation");

    public static implicit operator Result<TValue>(TValue value) => Succsess(value);
}
