namespace Interface.Provider;

public interface IEndpointUrlProvider
{
    string GetEndpointUrlFromEndpointName(string endpointName);

    string GenerateQueryParamsToAppend(Dictionary<string, string> keyValuePairs);
}
