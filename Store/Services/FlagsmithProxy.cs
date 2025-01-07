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
        private string _apiKey;
        private string _apiUrl;
        private FlagsmithClient? _flagsmithClient = null;
        private FlagsmithClient FlagsmithClient {
            get 
            {
                if (_flagsmithClient == null)
                {
                    // another way to obtain the configuration values
                    //var apiKey = configuration.GetSection("Flagsmith")["EnvironmentKey"]; 
                    //var apiUrl = configuration.GetSection("Flagsmith")["ApiUrl"];

                    _flagsmithClient = new FlagsmithClient(_apiKey, _apiUrl);
                }
                return _flagsmithClient;
            } 
        }        

        private IFlags? _flags = null;
        private IFlags Flags
        {
            get
            {
                if (_flags == null)
                {
                    _flags = GetEnvironmentFlags().Result;
                }
                return _flags;
            }
        }
        
        public FlagsmithProxy(string apiKey, string apiUrl)
        {            
            _apiKey = apiKey;
            _apiUrl = apiUrl;
        }

        private async Task<IFlags> GetEnvironmentFlags()
        {
            IFlags flags = null;
            try
            {

                flags = await this.FlagsmithClient.GetEnvironmentFlags(); // This method triggers a network request
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flags;
        }

        public async Task<bool> IsFeatureEnabled(string featureName)
        {
            // Assuming there's a method in FlagsmithClient to check if a feature is enabled
            var feature = await Flags.IsFeatureEnabled(featureName).ConfigureAwait(false);
            return feature;
        }
        
    }
}
