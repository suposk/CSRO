using AutoMapper;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IAzureVmManagementService
    {
        //Task<(bool, AzureManagErrorDto)> RestarVmInAzure2(VmTicket item);
        Task<(bool suc, string errorMessage)> RestarVmInAzure(VmTicket item);
        Task<(bool suc, string status)> GetVmDisplayStatus(VmTicket item);

        Task<bool> SubcriptionExist(string subscriptionId, CancellationToken cancelToken = default(CancellationToken));
    }


    public class AzureVmManagementService : BaseDataService, IAzureVmManagementService
    {

        public AzureVmManagementService(
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

        public async Task<bool> SubcriptionExist(string subscriptionId, CancellationToken cancelToken = default(CancellationToken))
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

        public async Task<(bool suc, string status)> GetVmDisplayStatus(VmTicket item)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                var url = $"https://management.azure.com/subscriptions/{item.SubcriptionId}/resourceGroups/{item.ResorceGroup}/providers/Microsoft.Compute/virtualMachines/{item.VmName}/instanceView?api-version=2020-06-01";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<AzureInstanceViewDto>(content, _options);
                    if (ser?.Statuses?.Count > 0)
                    {
                        //"VM running"
                        //var last = ser.Statuses.Last();
                        var last = ser.Statuses.LastOrDefault(a => a.Code.Contains("PowerState"));
                        if (last!= null)
                        {
                            return (true, last.DisplayStatus);
                        }
                    }
                }
                else
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<AzureManagErrorDto>(content, _options);
                    return (false, ser?.Error?.ToString());
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return (false, "Unable to verify Vm Status");
        }

        public async Task<(bool suc, string errorMessage)> RestarVmInAzure(VmTicket item)
        {
            var res = await RestarVmInAzure2(item);
            if (res.suc)
                return (res.suc, null);
            else
                return (res.suc, $"{res.error}");
        }


        private async Task<(bool suc, AzureManagErrorDto error)> RestarVmInAzure2(VmTicket item)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                var url = $"https://management.azure.com/subscriptions/{item.SubcriptionId}/resourceGroups/{item.ResorceGroup}/providers/Microsoft.Compute/virtualMachines/{item.VmName}/restart?api-version=2020-06-01";
                var apiData = await HttpClientBase.PostAsync(url, null).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {

                    if (apiData.StatusCode == System.Net.HttpStatusCode.OK || apiData.StatusCode == System.Net.HttpStatusCode.Accepted)
                        return (true, null);
                }
                else
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<AzureManagErrorDto>(content, _options);
                    return (false, ser);
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return (false, null);
        }


    }
}
