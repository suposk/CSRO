using CSRO.Common;
using CSRO.Server.Domain;
using CSRO.Server.Services;
using CSRO.Server.Services.AzureRestServices;
using CSRO.Server.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Server.Api.Services
{
    public interface ISubcriptionRepository 
    {
        Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default);
        Task<List<CustomerModel>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default);
    }

    public class SubcriptionRepository : ISubcriptionRepository
    {
        private readonly ISubcriptionService _subcriptionService;
        private readonly IAtCodecmdbReferenceRepository _atCodecmdbReferenceRepository;
        private readonly ICacheProvider _cacheProvider;
        const string cacheKeyProcess = nameof(IdName);

        public SubcriptionRepository(
            ISubcriptionService subcriptionService,
            IAtCodecmdbReferenceRepository atCodecmdbReferenceRepository,
            ICacheProvider cacheProvider)
        {
            _subcriptionService = subcriptionService;
            _atCodecmdbReferenceRepository = atCodecmdbReferenceRepository;
            _cacheProvider = cacheProvider;
        }

        public async Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default)
        {
            var cache = _cacheProvider.GetFromCache<List<IdName>>(cacheKeyProcess);
            if (cache.HasAnyInCollection())
                return cache;

            var subs = await _subcriptionService.GetSubcriptions(cancelToken);
            _cacheProvider.SetCache(cacheKeyProcess, subs, Core.ConstatCsro.CacheSettings.DefaultDuration);
            return subs;
        }

        public async Task<List<CustomerModel>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            try
            {
                var list = await _subcriptionService.GetTags(subscriptionIds, cancelToken);
                if (list.IsNullOrEmptyCollection())
                    return null;
                else
                    return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
