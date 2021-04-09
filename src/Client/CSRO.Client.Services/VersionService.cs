﻿using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
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
        //string _scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
        readonly string _scope;
        JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthCsroService _authCsroService;
        private readonly IMapper _mapper;
        private HttpClient _httpClient;

        public VersionService(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper, 
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _authCsroService = authCsroService;
            _mapper = mapper;
            _httpClient = _httpClientFactory.CreateClient(ConstatCsro.EndPoints.ApiEndpoint);
            _scope = configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Api);
        }

        public async Task<AppVersion> GetVersion(string version = "0")
        {
            try
            {
                //user_impersonation
                var apiToken = await _authCsroService.GetAccessTokenForUserAsync(_scope);

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
                else
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(content)) content = apiData.ReasonPhrase;
                    throw new Exception(content);
                }
            }
            catch (Exception ex)
            {
                throw;
            }            
        }

        public async Task<List<AppVersion>> GetAllVersion()
        {
            try
            {
                //user_impersonation
                var apiToken = await _authCsroService.GetAccessTokenForUserAsync(_scope);

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
                else
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(content)) content = apiData.ReasonPhrase;
                    throw new Exception(content);
                }
            }
            catch (Exception ex)
            {
                throw;
            }            
        }


        public async Task<AppVersion> AddVersion(AppVersion add)
        {
            try
            {
                //user_impersonation
                var apiToken = await _authCsroService.GetAccessTokenForUserAsync(_scope);

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
                var apiToken = await _authCsroService.GetAccessTokenForUserAsync(_scope);

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
