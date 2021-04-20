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
        Task<List<Customer>> GetCustomersByRegion(List<string> regions, CancellationToken cancelToken = default);
        Task<List<Customer>> GetCustomersBySubIds(List<string> subscriptionIds, CancellationToken cancelToken = default);
        Task<List<Customer>> GetCustomersBySubNames(List<string> subscriptionIds, CancellationToken cancelToken = default);
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

        public async Task<List<Customer>> GetCustomersBySubIds(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}GetCustomersBySubIds";                
                var httpcontent = new StringContent(JsonSerializer.Serialize(subscriptionIds, _options), Encoding.UTF8, "application/json");
                HttpRequestMessage requestMessage = new HttpRequestMessage { Method = HttpMethod.Get, Content = httpcontent, RequestUri = new Uri(HttpClientBase.BaseAddress + url) };
                var apiData = await HttpClientBase.SendAsync(requestMessage, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<List<CustomerDto>>(stream, _options);
                    var result = Mapper.Map<List<Customer>>(ser);
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

        public async Task<List<Customer>> GetCustomersBySubNames(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}GetCustomersBySubNames";                
                var httpcontent = new StringContent(JsonSerializer.Serialize(subscriptionIds, _options), Encoding.UTF8, "application/json");
                HttpRequestMessage requestMessage = new HttpRequestMessage { Method = HttpMethod.Get, Content = httpcontent, RequestUri = new Uri(HttpClientBase.BaseAddress + url) };
                var apiData = await HttpClientBase.SendAsync(requestMessage, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<List<CustomerDto>>(stream, _options);
                    var result = Mapper.Map<List<Customer>>(ser);
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

        public async Task<List<Customer>> GetCustomersByRegion(List<string> regions, CancellationToken cancelToken = default)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}GetCustomersByRegion";
                var httpcontent = new StringContent(JsonSerializer.Serialize(regions, _options), Encoding.UTF8, "application/json");
                HttpRequestMessage requestMessage = new HttpRequestMessage { Method = HttpMethod.Get, Content = httpcontent, RequestUri = new Uri(HttpClientBase.BaseAddress + url) };
                var apiData = await HttpClientBase.SendAsync(requestMessage, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<List<CustomerDto>>(stream, _options);
                    var result = Mapper.Map<List<Customer>>(ser);
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

    }


}
