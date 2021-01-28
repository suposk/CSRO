using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace CSRO.Client.Services
{
    public interface ICsroTokenCredentialProvider
    {
        TokenCredential GetCredential();
    }

    public class CsroTokenCredentialProvider : ICsroTokenCredentialProvider
    {
        private readonly IConfiguration _configuration;
        private readonly DefaultAzureCredential _tokenCredential;

        public CsroTokenCredentialProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _tokenCredential = new DefaultAzureCredential(includeInteractiveCredentials:true);
        }

        public TokenCredential GetCredential()
        {
            return _tokenCredential;

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
