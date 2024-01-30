namespace Domain;

public class DeprecatedResult<TValue, TError>
{
    private readonly TValue? value;
    private readonly TError? error;

    private DeprecatedResult(TValue value)
    {
        this.IsError = false;
        this.value = value;
        this.error = default;
    }

    private DeprecatedResult(TError error)
    {
        this.IsError = true;
        this.value = default;
        this.error = error;
    }

    public bool IsError { get; private set; }
    
    public bool IsSuccess => !this.IsError;

    public static implicit operator DeprecatedResult<TValue, TError>(TValue value) => new(value);

    public static implicit operator DeprecatedResult<TValue, TError>(TError error) => new(error);

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<TError, TResult> failure) =>
        !this.IsError ? success(this.value!) : failure(this.error!);
}
