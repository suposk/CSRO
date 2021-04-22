using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface ICustomerDataService
    {
        Task<List<Customer>> GetCustomersByRegions(List<string> regions, CancellationToken cancelToken = default);
        Task<List<Customer>> GetCustomersBySubIds(List<string> subscriptionIds, CancellationToken cancelToken = default);
        Task<List<Customer>> GetCustomersBySubId(string subscriptionId, CancellationToken cancelToken = default);
        Task<List<Customer>> GetCustomersBySubNames(List<string> subscriptionIds, CancellationToken cancelToken = default);
        Task<List<Customer>> GetCustomersBySubName(string subscriptionName, CancellationToken cancelToken = default);
        Task<List<Customer>> GetCustomersByAtCode(string atCode, CancellationToken cancelToken = default);
    }

    public class CustomerDataService : BaseDataService, ICustomerDataService
    {
        public CustomerDataService(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/customer/";
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Api);
            ClientName = ConstatCsro.EndPoints.ApiEndpoint;
            base.Init();
        }

        public Task<List<Customer>> GetCustomersBySubIds(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            return base.RestGenericSend<List<Customer>, List<CustomerDto>, List<string>>(HttpMethod.Get, subscriptionIds, "GetCustomersBySubIds");
        }
                
        public Task<List<Customer>> GetCustomersBySubId(string subscriptionId, CancellationToken cancelToken = default)
        {
            return base.RestGetListById<Customer, CustomerDto>(subscriptionId, "GetCustomersBySubId");
        }

        public Task<List<Customer>> GetCustomersBySubNames(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            return base.RestGenericSend<List<Customer>, List<CustomerDto>, List<string>>(HttpMethod.Get, subscriptionIds, "GetCustomersBySubNames");
            //try
            //{
            //    await base.AddAuthHeaderAsync();

            //    var url = $"{ApiPart}GetCustomersBySubNames";                
            //    var httpcontent = new StringContent(JsonSerializer.Serialize(subscriptionIds, _options), Encoding.UTF8, "application/json");
            //    HttpRequestMessage requestMessage = new HttpRequestMessage { Method = HttpMethod.Get, Content = httpcontent, RequestUri = new Uri(HttpClientBase.BaseAddress + url) };
            //    var apiData = await HttpClientBase.SendAsync(requestMessage, cancelToken).ConfigureAwait(false);

            //    if (apiData.IsSuccessStatusCode)
            //    {
            //        var stream = await apiData.Content.ReadAsStreamAsync();
            //        var ser = await JsonSerializer.DeserializeAsync<List<CustomerDto>>(stream, _options);
            //        var result = Mapper.Map<List<Customer>>(ser);
            //        return result;
            //    }
            //    else
            //        throw new Exception(GetErrorText(apiData));
            //}
            //catch (Exception ex)
            //{
            //    base.HandleException(ex);
            //    throw;
            //}
        }

        public Task<List<Customer>> GetCustomersBySubName(string subscriptionName, CancellationToken cancelToken = default)
        {
            return base.RestGetListById<Customer, CustomerDto>(subscriptionName, "GetCustomersBySubName");
        }

        public Task<List<Customer>> GetCustomersByRegions(List<string> regions, CancellationToken cancelToken = default)
        {
            return base.RestGenericSend<List<Customer>, List<CustomerDto>, List<string>>(HttpMethod.Get, regions, "GetCustomersByRegions");
        }

        public Task<List<Customer>> GetCustomersByAtCode(string atCode, CancellationToken cancelToken = default)
        {
            return base.RestGetListById<Customer, CustomerDto>(atCode, "GetCustomersByAtCode");
        }
    }


}
