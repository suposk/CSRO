using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IAdoProjectAccessDataService : IBaseDataService<AdoProjectAccessModel>
    {
        Task<List<AdoProjectAccessModel>> ApproveAdoProject(List<int> toApprove);
        Task<List<AdoProjectAccessModel>> RejectAdoProject(List<int> toReject, string rejectReason);
        Task<List<AdoProjectAccessModel>> GetProjectsForApproval();
    }

    public class AdoProjectAccessDataService : BaseDataService, IAdoProjectAccessDataService
    {

        public AdoProjectAccessDataService(
            IHttpClientFactory httpClientFactory,
            IAuthCsroService authCsroService,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/adoprojectaccess/";
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Ado_Api);
            ClientName = ConstatCsro.EndPoints.ApiEndpointAdo;

            base.Init();
        }

        public Task<AdoProjectAccessModel> AddItemAsync(AdoProjectAccessModel item)
        {
            return base.RestAdd<AdoProjectAccessModel, AdoProjectAccessDto>(item, "RequestAdoProjectAccess");
        }

        //RejectAdoProject

        public Task<List<AdoProjectAccessModel>> RejectAdoProject(List<int> toReject, string rejectReason)
        {
            var data = new RejectededListDto { ToReject = toReject, Reason = rejectReason };
            return base.RestSend<List<AdoProjectAccessModel>, List<AdoProjectAccessDto>, RejectededListDto>(HttpMethod.Post, data, "RejectAdoProjectAccess");

            //try
            //{
            //    await base.AddAuthHeaderAsync();                

            //    var url = $"{ApiPart}RejectAdoProjectAccess";
            //    var httpcontent = new StringContent(JsonSerializer.Serialize(data, _options), Encoding.UTF8, "application/json");
            //    var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);

            //    if (apiData.IsSuccessStatusCode)
            //    {
            //        var stream = await apiData.Content.ReadAsStreamAsync();
            //        var ser = await JsonSerializer.DeserializeAsync<List<AdoProjectAccessDto>>(stream, _options);
            //        var result = Mapper.Map<List<AdoProjectAccessModel>>(ser);
            //        return result;                    
            //    }
            //    else
            //        throw new Exception(base.GetErrorText(apiData));
            //}
            //catch (Exception ex)
            //{
            //    base.HandleException(ex);
            //    throw;
            //}
        }

        public Task<List<AdoProjectAccessModel>> ApproveAdoProject(List<int> toApprove)
        {
            return base.RestSend<List<AdoProjectAccessModel>, List<AdoProjectAccessDto>, List<int>>(HttpMethod.Post, toApprove, "RejectAdoProjectAccess");
        }

        public Task<bool> UpdateItemAsync(AdoProjectAccessModel item)
        {
            return base.RestUpdate<AdoProjectAccessModel, AdoProjectAccessDto>(item);
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            return base.RestDeleteById(id);
        }

        public Task<AdoProjectAccessModel> GetItemByIdAsync(int id)
        {
            return base.RestGetById<AdoProjectAccessModel, AdoProjectAccessDto>(id.ToString());
        }

        public Task<List<AdoProjectAccessModel>> GetItemsAsync()
        {
            return base.RestGetListById<AdoProjectAccessModel, AdoProjectAccessDto>();
        }

        public Task<List<AdoProjectAccessModel>> GetItemsByUserId(string userId) 
        {
            return base.RestGetListById<AdoProjectAccessModel, AdoProjectAccessDto>(userId, "GetByUserId");
        }

        public async Task<List<AdoProjectAccessModel>> GetProjectsForApproval()
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<List<AdoProjectAccessDto>>(stream, _options);                    
                    var result = Mapper.Map<List<AdoProjectAccessModel>>(ser);
                    return result.Where(a => a.Status == Models.Status.Submitted)?.ToList();
                }
                else
                    throw new Exception(base.GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
                throw;
            }
        }
    }
}
