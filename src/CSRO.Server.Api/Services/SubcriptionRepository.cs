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
        private readonly BillingContext _context;

        public SubcriptionRepository(
            ISubcriptionService subcriptionService,
            IAtCodecmdbReferenceRepository atCodecmdbReferenceRepository,
            ICacheProvider cacheProvider)
        {
            _subcriptionService = subcriptionService;            
            _cacheProvider = cacheProvider;
            _atCodecmdbReferenceRepository = atCodecmdbReferenceRepository;
            _context = _atCodecmdbReferenceRepository.DatabaseContext as BillingContext;
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
                //var list = await _subcriptionService.GetTags(subscriptionIds, cancelToken);
                //if (list.IsNullOrEmptyCollection())
                //    return null;
                //else
                //    return list;

                var dic = await _subcriptionService.GetTagsDictionary(subscriptionIds, cancelToken);
                if (dic.IsNullOrEmptyCollection())
                    return null;
                else
                {                    
                    List<string> atCodes = new();
                    foreach(var item in dic.Values)
                    {
                        var codes = item.cmdbReferenceList.Select(a => a.AtCode);
                        if (codes.HasAnyInCollection())
                            atCodes.AddRange(codes);
                    }
                    //TODO pass all at codes
                    //var q = _context.AtCodecmdbReferences.Where(a => a.AtCode.Contains(atCodes));                    
                    //var q = _context.AtCodecmdbReferences.ToListAsync();
                    //var all = await q;

                    foreach(var cus in dic.Values)
                    {
                        foreach(var item in cus.cmdbReferenceList)
                        {
                            var refCode = await _context.AtCodecmdbReferences.FirstOrDefaultAsync(a => a.AtCode == item.AtCode);
                            if (refCode != null)
                            {
                                item.Email = refCode.Email;
                            }
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
