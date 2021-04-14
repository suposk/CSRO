using AutoMapper;
using CSRO.Server.Domain;
using CSRO.Server.Infrastructure;
using CSRO.Server.Services.AzureRestServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Server.Services
{

    public interface IRestUserService
    {
        Task<List<Claim>> GetClaimsByUserNameAsync(string userName, CancellationToken cancelToken = default);
    }
    public class RestUserService : BaseDataService, IRestUserService
    {
        private readonly IApiIdentity _apiIdentity;
        private readonly ITokenAcquisition _tokenAcquisition;

        public RestUserService(
            IHttpClientFactory httpClientFactory,
            IApiIdentity apiIdentity,
            ITokenAcquisition tokenAcquisition,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, tokenAcquisition, configuration)
        {
            _apiIdentity = apiIdentity;
            _tokenAcquisition = tokenAcquisition;
            Mapper = mapper;

            ApiPart = "api/userclaim/";
            Scope = Core.ConstatCsro.Scopes.Scope_Auth_Api;
            ClientName = Core.ConstatCsro.EndPoints.ApiEndpointAuth;

            base.Init();
        }

        public IMapper Mapper { get; }

        public async Task<List<Claim>> GetClaimsByUserNameAsync(string userName, CancellationToken cancelToken = default)
        {
            try
            {
                var un = _apiIdentity.GetUserName();
                if (string.IsNullOrWhiteSpace(un))
                    return null; // user is not autenticated or context of auth was not trasferd

                //var us = await _tokenAcquisition.GetAuthenticationResultForUserAsync(new List<string> { Scope });

                //1. Call azure api
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{userName}";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<List<UserClaimDto>>(stream, _options);
                    if (ser.HasAnyInCollection())
                    {
                        List<Claim> list = new();
                        ser.ForEach(a => list.Add(new Claim(a.Type, a.Value)));
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }
    }
}
