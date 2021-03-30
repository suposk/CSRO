using CSRO.Server.Domain;
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
        Task<List<Customer>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default);
    }

    public class SubcriptionRepository : ISubcriptionRepository
    {
        private readonly ISubcriptionService _subcriptionService;

        public SubcriptionRepository(ISubcriptionService subcriptionService)
        {
            _subcriptionService = subcriptionService;
        }

        public async Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default)
        {
            var subs = await _subcriptionService.GetSubcriptions(cancelToken);
            return subs;
        }

        public async Task<List<Customer>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default)
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
