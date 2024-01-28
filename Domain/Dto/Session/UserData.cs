﻿namespace Domain.Dto.Session;

public class UserData
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Email { get; init; }

    public required string AvatarUrl { get; init; }
}
