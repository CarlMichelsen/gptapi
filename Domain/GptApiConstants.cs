﻿namespace Domain;

public static class GptApiConstants
{
    public const string AccessTokenAuthentication = "AccessTokenAuthentication";
    
    public const string ChatHubEndpoint = "/chathub";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Code Smell", "S1075:HardcodedAbsolutePathsOrURIs", Justification = "Used for development IDP. Not in production.")]
    public const string DeveloperFrontendUrl = "http://localhost:3000";

    public const string DeveloperIdpName = "IdentityProvider";
}