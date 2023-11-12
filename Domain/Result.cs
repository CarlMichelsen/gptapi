namespace Domain;

public class Result<TValue, TError>
{
    private readonly TValue? value;
    private readonly TError? error;

    private Result(TValue value)
    {
        this.value = value;
        this.error = default;
    }

    private Result(TError error)
    {
        this.value = default;
        this.error = error;
    }

    public bool IsError { get; }

    public bool IsSuccess => !this.IsError;

    public static implicit operator Result<TValue, TError>(TValue value) => new(value);

    public static implicit operator Result<TValue, TError>(TError error) => new(error);

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<TError, TResult> failure) =>
        !this.IsError ? success(this.value!) : failure(this.error!);
}
