using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace CSRO.Common.AzureSdkServices
{

    /// <summary>
    /// Wokrs in Some Tenanant
    /// </summary>
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
        }
    }
}
