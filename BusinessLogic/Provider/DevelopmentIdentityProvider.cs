using Domain;
using Domain.Dto.Steam;
using Interface.Provider;
using Microsoft.AspNetCore.Routing;

namespace BusinessLogic.Provider;

public class DevelopmentIdentityProvider : IDevelopmentIdentityProvider
{
    private readonly IEndpointUrlProvider endpointUrlProvider;

    public DevelopmentIdentityProvider(IEndpointUrlProvider endpointUrlProvider)
    {
        this.endpointUrlProvider = endpointUrlProvider;
    }

    public static List<SteamDevelopmentPlayerDto> DevelopmentUsers 
    {
        get 
        {
            return new List<SteamDevelopmentPlayerDto>
            {
                GenerateSteamDevelopmentPlayer("Lars Vegas", TimeSpan.FromDays(1), 1239018904512123123),
                GenerateSteamDevelopmentPlayer("Bruce Leeglad", TimeSpan.FromDays(5), 382372919023123123),
                GenerateSteamDevelopmentPlayer("Nicki Mirage", TimeSpan.FromHours(2), 2238901240891248902),
            };
        }
    }

    public static SteamDevelopmentPlayerDto GenerateSteamDevelopmentPlayer(string personaName, TimeSpan timeSinceLastLogout, long testSteamId)
    {
        var lastLogOffEpoch = DateTime.UtcNow.Subtract(timeSinceLastLogout) - DateTime.UnixEpoch;

        return new SteamDevelopmentPlayerDto
        {
            DevelopmentAccessToken = $"test-access-token-for:{personaName}",
            SteamId = testSteamId.ToString(),
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
        var uri = this.endpointUrlProvider.GetEndpointUrlFromEndpointName(GptApiConstants.DevelopmentLoginSuccessEndPointName);

        var res = new DevelopmentIdpResponse
        {
            SuccessRedirectUrl = uri,
            Players = DevelopmentUsers,
        };

        return Task.FromResult(res);
    }
}