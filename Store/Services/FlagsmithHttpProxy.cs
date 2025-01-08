using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Web;

public class FlagsmithHttpProxy
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<FlagsmithHttpProxy> _logger;

    public FlagsmithHttpProxy(ILogger<FlagsmithHttpProxy> logger, string apiKey, string apiUrl)
    {
        _logger = logger;
        _apiKey = apiKey;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(apiUrl)
        };
    }

    public async Task<JArray> GetFeatureFlagsAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "flags/");
            request.Headers.Add("x-environment-key", _apiKey);

            _logger.LogInformation("Sending request to Flagsmith API at {Url}", _httpClient.BaseAddress);

            var response = await _httpClient.SendAsync(request);
            var encodedContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Received response: {StatusCode}, Content: {Content}", response.StatusCode, encodedContent);

            response.EnsureSuccessStatusCode();

            // Decode the HTTP encoded content
            var decodedContent = HttpUtility.HtmlDecode(encodedContent);

            // Log the raw JSON content
            _logger.LogInformation("Decoded Content: {DecodedContent}", decodedContent);

            // Parse the JSON array
            return JArray.Parse(decodedContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching feature flags");
            throw;
        }
    }

    public async Task<bool> IsFeatureEnabledAsync(string featureKey)
    {
        var flags = await GetFeatureFlagsAsync();
        foreach (var flag in flags)
        {
            if (flag["feature"]?["name"]?.ToString() == featureKey)
            {
                return flag["enabled"]?.Value<bool>() ?? false;
            }
        }
        return false;
    }
}
