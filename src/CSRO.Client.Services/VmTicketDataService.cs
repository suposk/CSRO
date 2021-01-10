﻿using AutoMapper;
using CSRO.Client.Services;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IVmTicketDataService : IBaseDataService<VmTicket>
    {
        /// <summary>
        /// Call directly azure api and updated status
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> VerifyRestartStatus(VmTicket item);
    }

    public class VmTicketDataService : BaseDataService, IVmTicketDataService
    {
        const string MANAGEMENT_AZURE_SCOPE = "https://management.azure.com//.default";
        private readonly IAzureVmManagementService _azureVmManagementService;

        public VmTicketDataService(IAzureVmManagementService azureVmManagementService, 
            IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper, IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/vmticket/";
            //Scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
            Scope = Configuration.GetValue<string>("Scope_Api");
            ClientName = "api";

            base.Init();

            _azureVmManagementService = azureVmManagementService;
        }


        public async Task<bool> VerifyRestartStatus(VmTicket item)
        {
            
            try
            {
                //1. Call azure api
                //await base.AddAuthHeaderAsync();
                var azureApiToken = await AuthCsroService.GetAccessTokenForUserAsync(MANAGEMENT_AZURE_SCOPE);
                HttpClientBase.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", azureApiToken);

                var url = $"https://management.azure.com/subscriptions/{item.SubcriptionId}/resourceGroups/{item.ResorceGroup}/providers/Microsoft.Compute/virtualMachines/{item.VmName}/instanceView?api-version=2020-06-01";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<AzureInstanceViewDto>(content, _options);    
                    if (ser?.Statuses.Count > 0)
                    {
                        //"VM running"
                        var last = ser.Statuses.Last();
                        if (last.Code.Contains("PowerState"))
                        {
                            var server = await GetItemByIdAsync(item.Id);
                            if (server == null)
                                return false;

                            server.VmState = last.DisplayStatus;
                            var up = await UpdateItemAsync(server);
                            if (server.VmState.ToLower().Contains("running"))
                            {
                                item = server;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
        }

        // generic methods

        public async Task<VmTicket> AddItemAsync(VmTicket item)
        {

            try
            {
                var sent = await _azureVmManagementService.RestarVmInAzure(item);
                if (!sent.suc)
                    throw new Exception(sent.errorMessage);
            }
            catch
            {
                throw;
            }

            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var add = Mapper.Map<VmTicketDto>(item);
                var httpcontent = new StringContent(JsonSerializer.Serialize(add, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<VmTicketDto>(content, _options);
                    var result = Mapper.Map<VmTicket>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<bool> UpdateItemAsync(VmTicket item)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var add = Mapper.Map<VmTicketDto>(item);
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

        public async Task<VmTicket> GetItemByIdAsync(int id)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{id}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<VmTicketDto>(content, _options);
                    var result = Mapper.Map<VmTicket>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<List<VmTicket>> GetItemsAsync()
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<VmTicketDto>>(content, _options);
                    var result = Mapper.Map<List<VmTicket>>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public Task<List<VmTicket>> GetItemsByParrentIdAsync(int parrentId)
        {
            throw new NotImplementedException();
        }

        public Task<List<VmTicket>> GetItemsByTypeAsync(string type)
        {
            throw new NotImplementedException();
        }
    }
}
