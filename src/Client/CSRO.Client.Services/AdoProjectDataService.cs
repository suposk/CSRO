using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services;
using CSRO.Client.Services.Dtos;
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

    public interface IAdoProjectDataService : IBaseDataService<ProjectAdo>
    {
        Task<List<ProjectAdo>> ApproveAdoProject(List<int> toApprove);
        Task<List<ProjectAdo>> RejectAdoProject(List<int> toReject, string rejectReason);
        Task<List<ProjectAdo>> GetProjectsForApproval();
        Task<bool> ProjectExists(string organization, string projectName, int projectId);
    }

    public class AdoProjectDataService : BaseDataService, IAdoProjectDataService
    {        

        public AdoProjectDataService(            
            IHttpClientFactory httpClientFactory, 
            IAuthCsroService authCsroService, 
            IMapper mapper, 
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/adoproject/";            
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Ado_Api);
            ClientName = ConstatCsro.EndPoints.ApiEndpointAdo;

            base.Init();                        
        }

        public async Task<ProjectAdo> AddItemAsync(ProjectAdo item)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                string url = null;
                if (item.Status == Common.AdoServices.Models.Status.Draft)
                    url = $"{ApiPart}SaveDraftAdoProject";
                else
                    url = $"{ApiPart}RequestAdoProject";
                //var url = $"{ApiPart}";
                var httpcontent = new StringContent(JsonSerializer.Serialize(item, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);
                //HttpResponseMessage
                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<ProjectAdo>(content, _options);
                    return ser;
                    //var result = Mapper.Map<ProjectAdo>(ser);
                    //return result;
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

        public async Task<List<ProjectAdo>> RejectAdoProject(List<int> toReject, string rejectReason)
        {
            try
            {
                await base.AddAuthHeaderAsync();
                var data = new RejectededListDto { ToReject = toReject, Reason = rejectReason };

                var url = $"{ApiPart}RejectAdoProject";
                var httpcontent = new StringContent(JsonSerializer.Serialize(data, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<ProjectAdo>>(content, _options);
                    return ser;
                    //var result = Mapper.Map<List<ProjectAdo>>(ser);
                    //return result;
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

        public async Task<List<ProjectAdo>> ApproveAdoProject(List<int> toApprove)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}ApproveAdoProject";
                var httpcontent = new StringContent(JsonSerializer.Serialize(toApprove, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<ProjectAdo>>(content, _options);
                    return ser;
                    //var result = Mapper.Map<List<ProjectAdo>>(ser);
                    //return result;
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

        public async Task<bool> UpdateItemAsync(ProjectAdo item)
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

        public Task<bool> DeleteItemAsync(int id)
        {
            return base.RestDeleteById(id);
        }

        public async Task<ProjectAdo> GetItemByIdAsync(int id)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{id}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<ProjectAdo>(content, _options);
                    return ser;
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

        public async Task<List<ProjectAdo>> GetItemsAsync()
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<ProjectAdo>>(content, _options);
                    return ser;
                    //var result = Mapper.Map<List<ProjectAdo>>(ser);
                    //return result;
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

        public async Task<List<ProjectAdo>> GetProjectsForApproval()
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<ProjectAdo>>(content, _options);
                    return ser?.Where(a => a.State == ProjectState.CreatePending && a.Status == Common.AdoServices.Models.Status.Submitted)?.ToList();
                    //var result = Mapper.Map<List<ProjectAdo>>(ser);
                    //return result;
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

        public async Task<bool> ProjectExists(string organization, string projectName, int projectId)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{organization}/{projectName}/{projectId}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else if (apiData.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;
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
