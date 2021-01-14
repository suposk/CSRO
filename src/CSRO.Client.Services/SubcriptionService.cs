using AutoMapper;
using CSRO.Client.Services.Dtos;
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
    public interface ISubcriptionService
    {
        Task<bool> SubcriptionExist(string subscriptionId, CancellationToken cancelToken = default);

        Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default);

        Task<Subscription> GetSubcription(string subscriptionId, CancellationToken cancelToken = default);
    }

    public class SubcriptionService : BaseDataService, ISubcriptionService
    {

        public SubcriptionService(
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

        public async Task<Subscription> GetSubcription(string subscriptionId, CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions/{subscriptionId}?api-version=2020-01-01
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}?api-version=2020-01-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<SubscriptionsDto>(content, _options);
                    if (ser?.Value?.Count > 0)
                    {                                               
                        var first = ser.Value.FirstOrDefault();                        
                        var result = Mapper.Map<Subscription>(first);
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

        public async Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions?api-version=2020-01-01
                var url = $"https://management.azure.com/subscriptions?api-version=2020-01-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    //var ser = JsonSerializer.Deserialize<SubscriptionsDto>(content, _options);
                    var ser = JsonSerializer.Deserialize<SubscriptionsIdNameDto>(content, _options);
                    if (ser?.Value?.Count > 0)
                    {
                        //"VM running"
                        //var last = ser.Statuses.Last();
                        var idNameList = ser.Value.Where(a => a.State == "Enabled").Select(a => new IdName(a.SubscriptionId.ToString(), a.DisplayName)).ToList();
                        return idNameList;
                    }
                }
                else
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    //var ser = JsonSerializer.Deserialize<AzureManagErrorDto>(content, _options);
                    //return (false, ser?.Error?.ToString());
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<bool> SubcriptionExist(string subscriptionId, CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions/{subscriptionId}?api-version=2020-01-01
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}?api-version=2020-01-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    return true;
                    //var ser = JsonSerializer.Deserialize<AzureInstanceViewDto>(content, _options);
                    //if (ser?.Statuses?.Count > 0)
                    //{
                    //    //"VM running"
                    //    //var last = ser.Statuses.Last();
                    //    var last = ser.Statuses.LastOrDefault(a => a.Code.Contains("PowerState"));
                    //    if (last != null)
                    //    {
                    //        return (true, last.DisplayStatus);
                    //    }
                    //}
                }
                else
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    //var ser = JsonSerializer.Deserialize<AzureManagErrorDto>(content, _options);
                    //return (false, ser?.Error?.ToString());
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
        }

    }

}
