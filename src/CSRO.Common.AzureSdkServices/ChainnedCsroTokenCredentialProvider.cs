using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace CSRO.Common.AzureSdkServices
{
    /// <summary>
    /// Works in Personal tenatnt.  ManagedIdentityCredential and ClientSecretCredential
    /// </summary>
    public class ChainnedCsroTokenCredentialProvider : ICsroTokenCredentialProvider
    {
        private readonly IConfiguration _configuration;        

        public ChainnedCsroTokenCredentialProvider(IConfiguration configuration)
        {
            _configuration = configuration;            
        }

        public TokenCredential GetCredential()
        {            
            // Bind the configuration section to an instance of AzureAdAuthenticationOptions
            var azureAdOptions = _configuration.GetSection(nameof(AzureAd)).Get<AzureAd>();

            // If all three values are present, use both the Managed Identity and client secret credentials
            if (!string.IsNullOrEmpty(azureAdOptions.TenantId) &&
                !string.IsNullOrEmpty(azureAdOptions.ClientId) &&
                !string.IsNullOrEmpty(azureAdOptions.ClientSecret))
            {
                return new ChainedTokenCredential(
                    new ManagedIdentityCredential(),
                    new ClientSecretCredential(
                        azureAdOptions.TenantId,
                        azureAdOptions.ClientId,
                        azureAdOptions.ClientSecret));
            }

            // Otherwise, only use the Managed Identity credential
            return new ManagedIdentityCredential();
        }
    }
}
