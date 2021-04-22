﻿using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Core.Models;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
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
        public SubcriptionDataService(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper, 
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/subcription/";                   
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Api);            
            ClientName = ConstatCsro.EndPoints.ApiEndpoint;
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

        public Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default)
        {
            return base.RestGetListById<IdName, IdNameDto>();
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
