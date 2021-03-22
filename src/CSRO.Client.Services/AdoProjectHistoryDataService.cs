using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using CSRO.Common.AdoServices.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{

    //public interface IAdoProjectHistoryDataService: IBaseDataService<AdoProjectHistory>
    //{

    //}

    public class AdoProjectHistoryDataService : BaseDataService, IBaseDataService<AdoProjectHistory> //, IAdoProjectHistoryDataService
    {
        public AdoProjectHistoryDataService(
            IHttpClientFactory httpClientFactory,
            IAuthCsroService authCsroService,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/adoproject/{adoProjectId}/adoprojectHistory";
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Ado_Api);
            ClientName = ConstatCsro.EndPoints.ApiEndpointAdo;

            base.Init();
        }


        public override Task AddAuthHeaderAsync()
        {
            return base.AddAuthHeaderAsync();
        }

        public Task<AdoProjectHistory> AddItemAsync(AdoProjectHistory item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Task<AdoProjectHistory> GetItemByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AdoProjectHistory>> GetItemsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<AdoProjectHistory>> GetItemsByParrentIdAsync(int parrentId)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                ApiPart = $"api/adoproject/{parrentId}/adoprojectHistory";
                var url = $"{ApiPart}";                
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<AdoProjectHistory>>(content, _options);                    
                    var result = Mapper.Map<List<AdoProjectHistory>>(ser);
                    return result;
                }
                else
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    throw new Exception(content);
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
                throw;
            }

        }

        public Task<List<AdoProjectHistory>> GetItemsByTypeAsync(string type)
        {
            throw new NotImplementedException();
        }

        public override void HandleException(Exception ex)
        {
            base.HandleException(ex);
        }

        public override void Init()
        {
            base.Init();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public Task<bool> UpdateItemAsync(AdoProjectHistory item)
        {
            throw new NotImplementedException();
        }
    }
}
