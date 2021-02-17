using Azure;
using Azure.Core;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using CSRO.Common.AzureSdkServices.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace CSRO.Common.AzureSdkServices
{
    public interface ISubscriptionSdkService
    {
        Task<List<IdNameSdk>> GetAllSubcriptions(string subscriptionId = null, CancellationToken cancelToken = default);
        Task<IReadOnlyDictionary<string, string>> GetTags(string subscriptionId, CancellationToken cancelToken = default);
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IReadOnlyDictionary<string, string>> GetTags(string subscriptionId, CancellationToken cancelToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionId))
                    throw new Exception($"missing {nameof(subscriptionId)} parameter");

                subscriptionId = subscriptionId ?? ConstantsCommon.DEFAULT_SubscriptionId;
                var resourcesClient = new ResourcesManagementClient(subscriptionId, _tokenCredential);
                var resourceGroupClient = resourcesClient.Subscriptions;

                var info = await resourceGroupClient.GetAsync(subscriptionId, cancelToken);
                return info?.Value?.Tags;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}