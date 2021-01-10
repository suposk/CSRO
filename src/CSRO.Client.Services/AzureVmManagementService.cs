using AutoMapper;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public class AzureVmManagementService : BaseDataService, IAzureVmManagementService
    {

        public const string MANAGEMENT_AZURE_SCOPE = "https://management.azure.com//.default";

        public AzureVmManagementService(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/vmticket/";
            //Scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
            Scope = MANAGEMENT_AZURE_SCOPE;
            ClientName = "api";

            base.Init();
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
                    if (ser?.Statuses.Count > 0)
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
                    return (false, ser.Error.ToString());
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return (false, null);
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
                    return (true, ser);
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return (false, null);
        }

        public async Task<(bool suc, string errorMessage)> RestarVmInAzure(VmTicket item)
        {
            var res = await RestarVmInAzure2(item);
            if (res.suc)
                return (res.suc, null);
            else
                return (res.suc, $"{res.error}");
        }
    }
}
