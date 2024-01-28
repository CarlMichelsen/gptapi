namespace Domain;

public static class GptApiConstants
{
    public const string FrontendEndpointName = "FrontendEndpoint";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Code Smell", "S1075:HardcodedAbsolutePathsOrURIs", Justification = "Used for development. Not in production.")]
    public const string DeveloperFrontendUrl = "http://localhost:3001";

    public const string GithubHttpClient = "githubHttpClient";

    public const string GithubAPIHttpClient = "githubAPIHttpClient";

    public const string RequireSessionAuthorize = "RequireSession";
}