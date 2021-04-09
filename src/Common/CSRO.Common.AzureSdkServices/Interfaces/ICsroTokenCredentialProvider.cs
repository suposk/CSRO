using Azure.Core;

namespace CSRO.Common.AzureSdkServices
{
    public interface ICsroTokenCredentialProvider
    {
        TokenCredential GetCredential();
    }
}
