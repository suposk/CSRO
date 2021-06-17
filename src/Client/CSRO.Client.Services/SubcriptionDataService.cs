using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Core.Models;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using CSRO.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface ISubcriptionDataService
    {
        Task<bool> SubcriptionExist(string subscriptionId, CancellationToken cancelToken = default);
        Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default);
        Task<Subscription> GetSubcription(string subscriptionId, CancellationToken cancelToken = default);
        Task<List<TagNameWithValueList>> GetTags(string subscriptionId, CancellationToken cancelToken = default);
        Task<DefaultTags> GetDefualtTags(string subscriptionId, CancellationToken cancelToken = default);
        Task<Dictionary<string, DefaultTags>> GetDefualtTags(List<string> subscriptionIds, CancellationToken cancelToken = default);
    }
    public class SubcriptionDataService : BaseDataService, ISubcriptionDataService
    {
        private readonly ICacheProvider _cacheProvider;

        public SubcriptionDataService(
            IHttpClientFactory httpClientFactory, 
            IAuthCsroService authCsroService, 
            IMapper mapper,
            ICacheProvider cacheProvider,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/subcription/";                   
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Api);            
            ClientName = ConstatCsro.EndPoints.ApiEndpoint;
            _cacheProvider = cacheProvider;
            base.Init();            
        }

        public Task<DefaultTags> GetDefualtTags(string subscriptionId, CancellationToken cancelToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, DefaultTags>> GetDefualtTags(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Subscription> GetSubcription(string subscriptionId, CancellationToken cancelToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default)
        {
            var key = nameof(GetSubcriptions);
            var id = await base.AuthCsroService.GetCurrentUserId();
            var cache = _cacheProvider.GetFromCache<List<IdName>>(key, id);
            if (cache.HasAnyInCollection())
                return cache;
                        
            var subs = await base.RestGetListById<IdName, IdNameDto>();
            _cacheProvider.SetCache(key, id, subs, 2 * 60);
            return subs;            
        }


        public Task<List<TagNameWithValueList>> GetTags(string subscriptionId, CancellationToken cancelToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SubcriptionExist(string subscriptionId, CancellationToken cancelToken = default)
        {
            throw new NotImplementedException();
        }
    }


}
