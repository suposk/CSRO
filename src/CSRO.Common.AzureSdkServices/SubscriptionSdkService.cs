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
using System.Collections.Concurrent;

namespace CSRO.Common.AzureSdkServices
{
    public interface ISubscriptionSdkService
    {
        Task<List<IdNameSdk>> GetAllSubcriptions(string subscriptionId = null, CancellationToken cancelToken = default);
        Task<IReadOnlyDictionary<string, string>> GetTags(string subscriptionId, CancellationToken cancelToken = default);
        Task<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default);
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

        public async Task<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> result = new();            
            try
            {
                if (subscriptionIds?.Count <= 0)
                    throw new Exception($"missing {nameof(subscriptionIds)} parameter");

                Parallel.ForEach(subscriptionIds, (subscriptionId) =>
                {
                    try
                    {
                        var resourcesClient = new ResourcesManagementClient(subscriptionId, _tokenCredential);
                        var resourceGroupClient = resourcesClient.Subscriptions;

                        //var info = resourceGroupClient.GetAsync(subscriptionId, cancelToken).Result; //freeze
                        resourceGroupClient.GetAsync(subscriptionId, cancelToken).Wait();
                        var info = resourceGroupClient.GetAsync(subscriptionId, cancelToken).Result;
                        var tags = info?.Value?.Tags;

                        //cd.AddOrUpdate(1, 1, (key, oldValue) => oldValue + 1);
                        //var sessionId = a.Session.SessionID.ToString();
                        //userDic.AddOrUpdate(authUser.UserId, sessionId, (key, oldValue) => sessionId);
                        if (tags?.Count > 0)
                            //result.TryAdd(subscriptionId, tags);
                            result.AddOrUpdate(subscriptionId, tags, (key, oldValue) => tags);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                });

                Dictionary<string, IReadOnlyDictionary<string, string>> d = result.ToDictionary(pair => pair.Key, pair => pair.Value);
                return d;
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}