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
        private readonly AzureAd _azureAd;

        public ChainnedCsroTokenCredentialProvider(AzureAd azureAd)
        {
            _azureAd = azureAd;
        }

        public TokenCredential GetCredential()
        {
            // Bind the configuration section to an instance of AzureAdAuthenticationOptions
            var azureAdOptions = _azureAd;

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
