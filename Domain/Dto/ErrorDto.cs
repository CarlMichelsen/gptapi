using Domain.Abstractions;

namespace Domain.Dto;

public class ErrorDto
{
    public ErrorDto(Error error)
    {
        this.Code = error.Code;
        this.Description = error.Description;
    }

    public string Code { get; init; }

    public string? Description { get; init; }

    public DateTime TimeStampUtc { get; } = DateTime.UtcNow;
}
