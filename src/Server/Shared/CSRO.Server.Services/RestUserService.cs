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
        Task<List<Claim>> GetClaimsByUserNameAsync(string userName, ClaimsPrincipal principal, CancellationToken cancelToken = default);
    }
    public class RestUserService : BaseDataService, IRestUserService
    {               

        public RestUserService(
            IHttpClientFactory httpClientFactory,
            IApiIdentity apiIdentity,
            ITokenAcquisition tokenAcquisition,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, tokenAcquisition, apiIdentity, configuration)
        {                        
            Mapper = mapper;

            ApiPart = "api/userclaim/";            
            Scope = Configuration.GetValue<string>(Core.ConstatCsro.Scopes.Scope_Auth_Api);
            ClientName = Core.ConstatCsro.EndPoints.ApiEndpointAuth;
            base.Init();
        }

        public IMapper Mapper { get; }

        public async Task<List<Claim>> GetClaimsByUserNameAsync(string userName, CancellationToken cancelToken = default)
        {
            try
            {          
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

        public async Task<List<Claim>> GetClaimsByUserNameAsync(string userName, ClaimsPrincipal principal, CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                //await base.AddAuthHeaderAsync();
                var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new List<string> { Scope }, null, null, principal);
                HttpClientBase.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

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
