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

        public async Task<AdoProjectAccessModel> AddItemAsync(AdoProjectAccessModel item)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                string url = null;
                if (item.Status == Models.Status.Draft)
                    url = $"{ApiPart}SaveDraftAdoProjectAccess";
                else
                    url = $"{ApiPart}RequestAdoProjectAccess";
                //var url = $"{ApiPart}";
                var httpcontent = new StringContent(JsonSerializer.Serialize(item, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);
                //HttpResponseMessage
                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<AdoProjectAccessDto>(stream, _options);
                    var result = Mapper.Map<AdoProjectAccessModel>(ser);
                    return result;
                }
                else
                    throw new Exception(GetErrorText(apiData));

            }
            catch (Exception ex)
            {
                base.HandleException(ex);
                throw;
            }
        }

        //RejectAdoProject

        public async Task<List<AdoProjectAccessModel>> RejectAdoProject(List<int> toReject, string rejectReason)
        {
            try
            {
                await base.AddAuthHeaderAsync();
                var data = new RejectededListDto { ToReject = toReject, Reason = rejectReason };

                var url = $"{ApiPart}RejectAdoProjectAccess";
                var httpcontent = new StringContent(JsonSerializer.Serialize(data, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<List<AdoProjectAccessDto>>(stream, _options);
                    var result = Mapper.Map<List<AdoProjectAccessModel>>(ser);
                    return result;                    
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

        public async Task<List<AdoProjectAccessModel>> ApproveAdoProject(List<int> toApprove)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}ApproveAdoProjectAccess";
                var httpcontent = new StringContent(JsonSerializer.Serialize(toApprove, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<List<AdoProjectAccessDto>>(stream, _options);                    
                    var result = Mapper.Map<List<AdoProjectAccessModel>>(ser);
                    return result;
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

        public async Task<bool> UpdateItemAsync(AdoProjectAccessModel item)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                //var add = Mapper.Map<ProjectAdo>(item);
                var httpcontent = new StringContent(JsonSerializer.Serialize(item, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PutAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                    return true;
                else
                    throw new Exception(base.GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
                throw;
            }
            //return false;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{id}";
                var apiData = await HttpClientBase.DeleteAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                    return true;
                else
                    throw new Exception(base.GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
        }

        public async Task<AdoProjectAccessModel> GetItemByIdAsync(int id)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{id}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<AdoProjectAccessDto>(stream, _options);
                    var result = Mapper.Map<AdoProjectAccessModel>(ser);
                    return result;
                }
                else
                    throw new Exception(base.GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<List<AdoProjectAccessModel>> GetItemsAsync()
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
                    return result;
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

        public async Task<List<AdoProjectAccessModel>> GetItemsByUserId(string userId) 
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
                    return result;
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
