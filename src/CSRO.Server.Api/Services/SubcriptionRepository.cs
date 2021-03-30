using CSRO.Server.Domain;
using CSRO.Server.Services.AzureRestServices;
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
    }
}
