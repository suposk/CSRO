using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services.Dtos.AzureDtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface INetworkService
    {
        Task<List<Network>> GetNetworks(string subscriptionId, string location, CancellationToken cancelToken = default);
    }

    public class NetworkService : BaseDataService, INetworkService
    {
        public NetworkService(
            IHttpClientFactory httpClientFactory,
            IAuthCsroService authCsroService,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "--";
            //Scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
            Scope = ConstatCsro.Scopes.MANAGEMENT_AZURE_SCOPE;
            //ClientName = "api";
            ClientName = ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint;

            base.Init();
        }

        public async Task<List<Network>> GetNetworks(string subscriptionId, string location, CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Network/virtualNetworks?api-version=2020-07-01
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Network/virtualNetworks?api-version=2020-07-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<VirtualNetworksDto>(content, _options);
                    if (ser?.Value?.Count > 0)
                    {
                        List<Network> list = new List<Network>();
                        foreach (var item in ser.Value)
                        {
                            var net = new Network 
                            {
                                Location = item.Location, 
                                VirtualNetwork = item.Name,                                
                                Subnets = new List<string>(item.Properties.Subnets.Select(a => a.Name)),
                            };
                            var rg = Core.Helpers.GetValueHelper.GetVal(item.Id, "resourceGroups");
                            net.NetworkResourceGroup = rg;

                            list.Add(net);
                        }
                        return list;

                        ////exception
                        //var result = Mapper.Map<List<Network>>(ser.Value);
                        //if (result?.Count > 0)
                        //{
                        //    var list = result.Where(a => a.Location == location).ToList();
                        //    return list;
                        //}
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
