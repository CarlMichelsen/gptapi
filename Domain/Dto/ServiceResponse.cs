namespace Domain.Dto;

public class ServiceResponse<T>
    where T : new()
{
    public ServiceResponse(T data)
    {
        this.Data = data;
    }

    public ServiceResponse(params string[] error)
    {
        this.Errors = new List<string>(error);
    }

    public bool Ok => this.Data is not null && this.Errors?.Count == 0;

    public T? Data { get; init; }

    public List<string>? Errors { get; init; }
}
