﻿using Domain.Dto.Steam;
using Interface.Handler;

namespace BusinessLogic.Development;

public class DevelopmentIdpHandler : IDevelopmentIdpHandler
{
    public DevelopmentIdpHandler()
    {
        if (DevelopmentUsers is not null)
        {
            return;
        }

        var players = new List<SteamDevelopmentPlayerDto>
        {
            GenerateSteamDevelopmentPlayer("Lars Vegas", TimeSpan.FromDays(1)),
            GenerateSteamDevelopmentPlayer("Bruce Leeglad", TimeSpan.FromDays(5)),
            GenerateSteamDevelopmentPlayer("Nicki Mirage", TimeSpan.FromHours(2)),
        };

        DevelopmentUsers = players;
    }

    public static List<SteamDevelopmentPlayerDto>? DevelopmentUsers { get; set; } = default;

    public static SteamDevelopmentPlayerDto GenerateSteamDevelopmentPlayer(string personaName, TimeSpan timeSinceLastLogout)
    {
        var lastLogOffEpoch = DateTime.Now.Subtract(timeSinceLastLogout) - DateTime.UnixEpoch;

        return new SteamDevelopmentPlayerDto
        {
            DevelopmentAccessToken = $"test-access-token-for:{personaName}",
            SteamId = $"test-steamid-for:{personaName}",
            CommunityVisibilityState = 1,
            ProfileState = 1,
            PersonaName = personaName,
            LastLogoff = (long)lastLogOffEpoch.TotalSeconds,
            ProfileUrl = "fake url",
            Avatar = null,
            AvatarMedium = null,
            AvatarFull = null,
        };
    }

    public Task<DevelopmentIdpResponse> GetDevelopmentUsers()
    {
        var res = new DevelopmentIdpResponse
        {
            SuccessRedirectUrl = "http://localhost:5142/api/v1/oauth/SteamLoginSuccess",
            Players = DevelopmentUsers ?? throw new NullReferenceException("DevelopmentUsers has not yet been assigned"),
        };

        return Task.FromResult(res);
    }
}