using AutoMapper;
using CSRO.Client.Services;
using CSRO.Client.Services.Dtos;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{
    public class TicketDataStore : IBaseDataStore<Ticket>
    {
        const string _apiPart = "api/ticket/";
        const string scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
        JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IMapper _mapper;
        private HttpClient _httpClient;

        public TicketDataStore(IHttpClientFactory httpClientFactory, ITokenAcquisition tokenAcquisition, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _tokenAcquisition = tokenAcquisition;
            _mapper = mapper;
            if (_httpClient == null)
                _httpClient = _httpClientFactory.CreateClient("api");
        }

        public async Task<Ticket> AddItemAsync(Ticket item)
        {
            try
            {
                //user_impersonation
                //var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });
                //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

                var url = $"{_apiPart}";
                var add = _mapper.Map<TicketDto>(item);
                var httpcontent = new StringContent(JsonSerializer.Serialize(add, _options), Encoding.UTF8, "application/json");
                var apiData = await _httpClient.PostAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<TicketDto>(content, _options);
                    var result = _mapper.Map<Ticket>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Ticket> GetItemByIdAsync(int id)
        {
            try
            {
                //user_impersonation
                //var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });
                //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

                var url = $"{_apiPart}{id}";
                var apiData = await _httpClient.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<TicketDto>(content, _options);
                    var result = _mapper.Map<Ticket>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public async Task<List<Ticket>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                //user_impersonation
                //var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });
                //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

                var url = $"{_apiPart}";
                var apiData = await _httpClient.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<TicketDto>>(content, _options);
                    var result = _mapper.Map<List<Ticket>>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public Task<List<Ticket>> GetItemsByParrentIdAsync(int parrentId, bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetItemsByTypeAsync(Enum type, bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> UpdateItemAsync(Ticket item)
        {
            throw new NotImplementedException();
        }
    }
}
