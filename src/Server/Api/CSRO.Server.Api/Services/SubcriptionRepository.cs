using CSRO.Common;
using CSRO.Server.Domain;
using CSRO.Server.Entities;
using CSRO.Server.Services;
using CSRO.Server.Services.AzureRestServices;
using CSRO.Server.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CSRO.Common.AzureSdkServices;

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
        private readonly ISubscriptionSdkService _subscriptionSdkService;
        private readonly IAtCodecmdbReferenceRepository _atCodecmdbReferenceRepository;
        private readonly ICacheProvider _cacheProvider;
        const string cacheKeyProcess = nameof(IdName);        

        public SubcriptionRepository(
            ISubcriptionService subcriptionService,
            ISubscriptionSdkService subscriptionSdkService,
            IAtCodecmdbReferenceRepository atCodecmdbReferenceRepository,
            ICacheProvider cacheProvider)
        {
            _subcriptionService = subcriptionService;
            _subscriptionSdkService = subscriptionSdkService;
            _cacheProvider = cacheProvider;
            _atCodecmdbReferenceRepository = atCodecmdbReferenceRepository;            
        }

        public async Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default)
        {
            var cache = _cacheProvider.GetFromCache<List<IdName>>(cacheKeyProcess);
            if (cache.HasAnyInCollection())
                return cache;

            var subsSdk = await _subscriptionSdkService.GetAllSubcriptions(null, cancelToken).ConfigureAwait(false);
            var subs = await _subcriptionService.GetSubcriptions(cancelToken);
            _cacheProvider.SetCache(cacheKeyProcess, subs, Core.ConstatCsro.CacheSettings.DefaultDuration);
            return subs;
        }

        public async Task<List<CustomerModel>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            try
            {
                var dic = await _subcriptionService.GetTagsDictionary(subscriptionIds, cancelToken);
                if (dic.IsNullOrEmptyCollection())
                    return null;
                else
                {                    
                    List<string> modelAtCodes = new();
                    foreach(var item in dic.Values)
                    {
                        var codes = item.cmdbReferenceList.Select(a => a.AtCode);
                        if (codes.HasAnyInCollection())
                            modelAtCodes.AddRange(codes);
                    }

                    //pass all at codes
                    var q = _atCodecmdbReferenceRepository.Context.ResourceSWIs.Where(a => modelAtCodes.Contains(a.AtCode)).ToListAsync();
                    var dbAtCodes = await q;

                    foreach (var cus in dic.Values)
                    {
                        foreach (var item in cus.cmdbReferenceList)
                        {
                            var found = dbAtCodes.FirstOrDefault(a => a.AtCode == item.AtCode);
                            if (found != null)
                                item.Email = found.Email;
                        }
                    }

                    return dic.Values.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
