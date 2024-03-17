/*
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

*/

export type CarlGptError = {
    code: string;
    description: string|null;
    timeStampUtc: Date;
};