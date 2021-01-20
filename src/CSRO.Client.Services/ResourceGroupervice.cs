﻿using AutoMapper;
using CSRO.Client.Services.Dtos.AzureDtos;
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
    public interface IResourceGroupervice
    {
        Task<List<ResourceGroup>> GetResourceGroups(string subscriptionId, CancellationToken cancelToken = default);
        Task<List<ResourceGroup>> GetResourceGroups(string subscriptionId, string location, CancellationToken cancelToken = default);
        Task<List<IdName>> GetResourceGroupsIdName(string subscriptionId, CancellationToken cancelToken = default);
    }

    public class ResourceGroupervice : BaseDataService, IResourceGroupervice
    {

        public ResourceGroupervice(
            IHttpClientFactory httpClientFactory,
            IAuthCsroService authCsroService,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "--";
            //Scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
            Scope = Core.ConstatCsro.Scopes.MANAGEMENT_AZURE_SCOPE;
            //ClientName = "api";
            ClientName = Core.ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint;

            base.Init();
        }

        public async Task<List<ResourceGroup>> GetResourceGroups(string subscriptionId, CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups?api-version=2020-06-01
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups?api-version=2020-06-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();                    
                    var ser = JsonSerializer.Deserialize<ResourceGroupsDto>(content, _options);
                    if (ser?.Value?.Count > 0)
                    {
                        //exception
                        var result = Mapper.Map<List<ResourceGroup>>(ser.Value);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<List<ResourceGroup>> GetResourceGroups(string subscriptionId, string location, CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups?api-version=2020-06-01
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups?api-version=2020-06-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<ResourceGroupsDto>(content, _options);
                    if (ser?.Value?.Count > 0)
                    {
                        //exception
                        var result = Mapper.Map<List<ResourceGroup>>(ser.Value);
                        if (result?.Count > 0)
                        {
                            var list = result.Where(a => a.Location == location);
                            return list.ToList();
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<List<IdName>> GetResourceGroupsIdName(string subscriptionId, CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups?api-version=2020-06-01
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups?api-version=2020-06-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();                    
                    var ser = JsonSerializer.Deserialize<ResourceGroupsDto>(content, _options);
                    if (ser?.Value?.Count > 0)
                    {
                        //"VM running"
                        //var last = ser.Statuses.Last();
                        var idNameList = ser.Value.Select(a => new IdName(a.Id, a.Name)).ToList();
                        return idNameList;
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
