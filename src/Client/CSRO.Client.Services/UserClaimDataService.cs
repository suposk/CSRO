using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{

    public interface IUserClaimDataService : IBaseDataService<UserClaim>
    {
        Task<List<Claim>> GetUserClaimsByUserName(string userName);
    }

    public class UserClaimDataService : BaseDataService, IUserClaimDataService
    {

        public UserClaimDataService(
            IHttpClientFactory httpClientFactory,
            IAuthCsroService authCsroService,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/UserClaim/";
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Auth_Api);
            ClientName = ConstatCsro.EndPoints.ApiEndpointAuth;

            base.Init();
        }

        public Task<UserClaim> AddItemAsync(UserClaim item)
        {
            return base.RestAdd<UserClaim, UserClaimDto>(item);
        }


        public Task<bool> UpdateItemAsync(UserClaim item)
        {
            return base.RestUpdate<UserClaim, UserClaimDto>(item);
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            return base.RestDeleteById(id);
        }

        public Task<UserClaim> GetItemByIdAsync(int id)
        {
            return base.RestGetById<UserClaim, UserClaimDto>(id.ToString());
        }

        public Task<List<UserClaim>> GetItemsAsync()
        {
            return base.RestGetListById<UserClaim, UserClaimDto>();
        }

        public async Task<List<Claim>> GetUserClaimsByUserName(string userName)
        {
            try
            {
                await base.AddAuthHeaderAsync();
                
                var url = $"{ApiPart}{userName}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

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
                else
                    throw new Exception(base.GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
                //throw;
            }
            return null;
        }

    }

}
