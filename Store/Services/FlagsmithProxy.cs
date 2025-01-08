using Flagsmith;
using FlagsmithEngine.Models;
using Microsoft.Extensions.Configuration;

namespace Store.Services
{
    public interface IFlagsmithProxy
    {
        Task<bool> IsFeatureEnabled(string featureName);
    }

    public class FlagsmithProxy : IFlagsmithProxy
    {
        private readonly FlagsmithClient _client;
        private readonly ILogger<FlagsmithProxy> _logger;
       
        public FlagsmithProxy(ILogger<FlagsmithProxy> logger, string apiKey, string apiUrl)
        {
            _logger = logger;
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };
            _client = new FlagsmithClient(apiKey, apiUrl: apiUrl, httpClient: httpClient);
        }      

        public async Task<bool> IsFeatureEnabled(string featureName)
        {
            try
            {
                _logger.LogInformation($"IsFeatureEnabled started");
                var flags = await _client.GetEnvironmentFlags();
                _logger.LogInformation($"IsFeatureEnabled completed");
                return await flags.IsFeatureEnabled(featureName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking feature flag: {featureName}", featureName);
                throw;
            }
        }
        
    }
}
