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

    public interface IAdoProjectDataService : IBaseDataService<ProjectAdo>
    {
        Task<List<ProjectAdo>> ApproveAdoProject(List<int> toApprove);
        Task<List<ProjectAdo>> GetProjectsForApproval();
        Task<bool> ProjectExists(string organization, string projectName);
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

        //public async Task<ProjectAdo> RebootVmAndWaitForConfirmation(ProjectAdo item)
        //{
        //    try
        //    {     
        //        await base.AddAuthHeaderAsync();

        //        var url = $"{ApiPart}RebootVmAndWaitForConfirmation";
        //        var add = Mapper.Map<ProjectAdo>(item);
        //        var httpcontent = new StringContent(JsonSerializer.Serialize(add, _options), Encoding.UTF8, "application/json");
        //        var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);

        //        if (apiData.IsSuccessStatusCode)
        //        {
        //            var content = await apiData.Content.ReadAsStringAsync();
        //            var ser = JsonSerializer.Deserialize<ProjectAdo>(content, _options);
        //            var result = Mapper.Map<ProjectAdo>(ser);
        //            return result;
        //        }
        //        else
        //        {
        //            var content = await apiData.Content.ReadAsStringAsync();
        //            //var ser = JsonSerializer.Deserialize<AzureManagErrorDto>(content, _options);
        //            throw new Exception(content);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        base.HandleException(ex);
        //        throw;
        //    }            
        //}

        // generic methods

        public async Task<ProjectAdo> AddItemAsync(ProjectAdo item)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}RequestAdoProject";
                //var url = $"{ApiPart}";
                var httpcontent = new StringContent(JsonSerializer.Serialize(item, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<ProjectAdo>(content, _options);
                    return ser;
                    //var result = Mapper.Map<ProjectAdo>(ser);
                    //return result;
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

        public async Task<bool> UpdateItemAsync(ProjectAdo item)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var add = Mapper.Map<ProjectAdo>(item);
                var httpcontent = new StringContent(JsonSerializer.Serialize(add, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PutAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{id}";
                var apiData = await HttpClientBase.DeleteAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
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
                    var result = Mapper.Map<ProjectAdo>(ser);
                    return result;
                }
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

        public Task<List<ProjectAdo>> GetItemsByParrentIdAsync(int parrentId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProjectAdo>> GetItemsByTypeAsync(string type)
        {
            throw new NotImplementedException();
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
                    return ser?.Where(a => a.State == ProjectState.CreatePending)?.ToList();
                    //var result = Mapper.Map<List<ProjectAdo>>(ser);
                    //return result;
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

        public async Task<bool> ProjectExists(string organization, string projectName)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{projectName}/{organization}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else if (apiData.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;
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
    }
}
