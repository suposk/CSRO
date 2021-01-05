using AutoMapper;
using CSRO.Client.Services;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
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
    public class BaseDataStore
    {
        public string ClientName { get; set; }
        public string _apiPart { get; set; }
        public string scope { get; set; }

        public JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public readonly IHttpClientFactory _httpClientFactory;
        public readonly IAuthCsroService _authCsroService;
        public readonly IMapper _mapper;
        public HttpClient _httpClient { get; private set; }

        public BaseDataStore(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _authCsroService = authCsroService;
            _mapper = mapper;            
        }

        /// <summary>
        /// verify all params and create httpclient        
        /// </summary>
        public virtual void Init()
        {
            if (string.IsNullOrWhiteSpace(ClientName))
                throw new Exception($"{ClientName} must be set in before calling method.");

            if (string.IsNullOrWhiteSpace(_apiPart))
                throw new Exception($"{_apiPart} must be set in before calling method.");

            if (string.IsNullOrWhiteSpace(scope))
                throw new Exception($"{scope} must be set in before calling method.");

            if (_httpClient == null)
                _httpClient = _httpClientFactory.CreateClient(ClientName);

            //todo read from config
        }

        public virtual void HandleException(Exception ex)
        {
            Console.WriteLine($"{nameof(HandleException)}: {ex}");
        }

        public virtual async Task AddAuthHeader()
        {
            //user_impersonation
            var apiToken = await _authCsroService.GetAccessTokenForUserAsync(scope);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
        }
    }

    public class TicketDataStore : BaseDataStore, IBaseDataStore<Ticket>
    {
        public TicketDataStore(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper)
            : base(httpClientFactory, authCsroService, mapper)
        {
            _apiPart = "api/ticket/";
            scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
            ClientName = "api";

            base.Init();
        }

        public async Task<Ticket> AddItemAsync(Ticket item)
        {
            try
            {
                await base.AddAuthHeader();

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
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                await base.AddAuthHeader();

                var url = $"{_apiPart}{id}";
                var apiData = await _httpClient.DeleteAsync(url).ConfigureAwait(false);

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

        public async Task<Ticket> GetItemByIdAsync(int id)
        {
            try
            {
                await base.AddAuthHeader();

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
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<List<Ticket>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                await base.AddAuthHeader();

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
                base.HandleException(ex);
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

        public async Task<bool> UpdateItemAsync(Ticket item)
        {
            try
            {
                await base.AddAuthHeader();

                var url = $"{_apiPart}";
                var add = _mapper.Map<TicketDto>(item);
                var httpcontent = new StringContent(JsonSerializer.Serialize(add, _options), Encoding.UTF8, "application/json");
                var apiData = await _httpClient.PutAsync(url, httpcontent).ConfigureAwait(false);

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
    }
}
