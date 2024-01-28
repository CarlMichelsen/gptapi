namespace Domain.Dto.Session;

public class UserDto
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string AvatarUrl { get; init; }
}
