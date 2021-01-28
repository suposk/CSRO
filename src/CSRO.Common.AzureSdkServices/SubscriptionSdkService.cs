using Azure;
using Azure.Core;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using CSRO.Client.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Common.AzureSdkServices
{
    public interface ISubscriptionSdkService
    {
        Task<List<IdName>> GetAllSubcriptions(string subscriptionId = null, CancellationToken cancelToken = default);
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

        public async Task<List<IdName>> GetAllSubcriptions(string subscriptionId = null, CancellationToken cancelToken = default)
        {
            try
            {
                subscriptionId = subscriptionId ?? "33fb38df-688e-4ca1-8dd8-b46e26262ff8";
                var resourcesClient = new ResourcesManagementClient(subscriptionId, _tokenCredential);                
                var resourceGroupClient = resourcesClient.ResourceGroups;
                var result = new List<IdName>();

                AsyncPageable<Subscription> subs = resourcesClient.Subscriptions.ListAsync();
                await foreach (var sub in subs)
                {
                    result.Add(new IdName { Id = sub.SubscriptionId, Name = sub.DisplayName });
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