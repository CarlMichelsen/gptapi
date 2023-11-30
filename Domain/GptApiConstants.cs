namespace Domain;

public static class GptApiConstants
{
    public const string ChatHubEndpoint = "/chathub";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Code Smell", "S1075:HardcodedAbsolutePathsOrURIs", Justification = "Used for development IDP. Not in production.")]
    public const string DeveloperFrontendUrl = "http://localhost:3000";

    public const string FrontendEndpointName = "FrontendEndpoint";

    public const string SteamLoginSuccessEndPointName = "SteamLoginSuccess";
    
    public const string DeveloperIdpName = "IdentityProvider";
}