using AutoMapper;
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
    public class VersionService : IVersionService
    {
        const string _apiPart = "api/version/";
        const string scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
        JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IMapper _mapper;
        private HttpClient _httpClient;

        public VersionService(IHttpClientFactory httpClientFactory, ITokenAcquisition tokenAcquisition, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _tokenAcquisition = tokenAcquisition;
            _mapper = mapper;
            if (_httpClient == null)
                _httpClient = _httpClientFactory.CreateClient("api");
        }

        public async Task<AppVersion> GetVersion(string version = "0")
        {
            try
            {
                //user_impersonation
                var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
                var url = $"{_apiPart}{version}";
                var apiData = await _httpClient.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<AppVersionDto>(content, _options);
                    var result = _mapper.Map<AppVersion>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public async Task<List<AppVersion>> GetAllVersion()
        {
            try
            {
                //user_impersonation
                var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
                var url = $"{_apiPart}";
                var apiData = await _httpClient.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<AppVersionDto>>(content, _options);
                    var version = _mapper.Map<List<AppVersion>>(ser);
                    return version;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }


        public async Task<AppVersion> AddVersion(AppVersion add)
        {
            try
            {
                //user_impersonation
                var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
                var url = $"{_apiPart}";
                var httpcontent = new StringContent(JsonSerializer.Serialize(add, _options), Encoding.UTF8, "application/json");
                var apiData = await _httpClient.PostAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<AppVersionDto>(content, _options);
                    var result = _mapper.Map<AppVersion>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public async Task<bool> DeleteVersion(int id)
        {
            try
            {
                //user_impersonation
                var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
                var url = $"{_apiPart}{id}";
                var apiData = await _httpClient.DeleteAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                    return true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return false;
        }
    }
}
