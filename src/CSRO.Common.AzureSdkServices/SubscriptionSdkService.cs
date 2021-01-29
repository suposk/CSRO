using Azure;
using Azure.Core;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using CSRO.Common.AzureSdkServices.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Common.AzureSdkServices
{
    public interface ISubscriptionSdkService
    {
        Task<List<IdNameSdk>> GetAllSubcriptions(string subscriptionId = null, CancellationToken cancelToken = default);
    }

    public class SubscriptionSdkService : ISubscriptionSdkService
    {
        private readonly ICsroTokenCredentialProvider _csroTokenCredentialProvider;
        private readonly TokenCredential _tokenCredential;

        public SubscriptionSdkService(ICsroTokenCredentialProvider csroTokenCredentialProvider)
        {
            _csroTokenCredentialProvider = csroTokenCredentialProvider;
            _tokenCredential = _csroTokenCredentialProvider.GetCredential();
        }

        public async Task<List<IdNameSdk>> GetAllSubcriptions(string subscriptionId = null, CancellationToken cancelToken = default)
        {
            try
            {
                subscriptionId = subscriptionId ?? ConstantsCommon.DEFAULT_SubscriptionId;
                var resourcesClient = new ResourcesManagementClient(subscriptionId, _tokenCredential);                
                var resourceGroupClient = resourcesClient.ResourceGroups;
                var result = new List<IdNameSdk>();

                AsyncPageable<Subscription> subs = resourcesClient.Subscriptions.ListAsync();
                await foreach (var sub in subs)
                {
                    result.Add(new IdNameSdk { Id = sub.SubscriptionId, Name = sub.DisplayName });
                }
                return result;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}