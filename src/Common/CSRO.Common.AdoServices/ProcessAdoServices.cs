﻿using CSRO.Common.AdoServices.Models;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.Identity.Web;
using Microsoft.VisualStudio.Services.Client; //it ,ay be removed perhaps
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.Account.Client;
using Microsoft.VisualStudio.Services.Profile.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.IO;
using CSRO.Common.AdoServices.Dtos;

namespace CSRO.Common.AdoServices
{
    public interface IProcessAdoServices
    {
        Task<ProcessAdo> GetAdoProcesByName(string organization, string name);
        Task<List<ProcessAdo>> GetAdoProcesses(string organization);
        Task<List<string>> GetAdoProcessesName(string organization);
        Task<List<string>> GetOrganizationNames();
    }

    public class ProcessAdoServices : IProcessAdoServices
    {
        private readonly AdoConfig _adoConfig;
        private readonly IMapper _mapper;
        private readonly ICacheProvider _cacheProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProcessAdoServices> _logger;
        //private List<ProcessAdo> _processAdos = null;
        const string cacheKeyProcess = nameof(ProcessAdo);
        const string cacheKeyOrganization = nameof(OrganizationDto);
        public JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        readonly string DefAdoOrganization;

        public ProcessAdoServices(
            IConfiguration configuration, 
            IMapper mapper,
            ICacheProvider cacheProvider,
            IHttpClientFactory httpClientFactory,
            ILogger<ProcessAdoServices> logger = null)
        {
            _mapper = mapper;
            _cacheProvider = cacheProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _adoConfig = configuration.GetSection(nameof(AdoConfig)).Get<AdoConfig>();
            DefAdoOrganization = configuration.GetValue<string>("DefAdoOrganization");
        }

        public async Task<List<ProcessAdo>> GetAdoProcesses(string organization)
        {                                  
            organization ??= DefAdoOrganization;
            VssConnection connection = null;
            try
            {
                //var or = await GetOrganizations();

                var processAdos = _cacheProvider.GetFromCache<List<ProcessAdo>>(cacheKeyProcess);
                if (processAdos?.Count > 0)
                    return processAdos;

                string url = $"https://dev.azure.com/{organization}";
                if (_adoConfig.UsePta)
                    connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, _adoConfig.AdoPersonalAccessToken));
                else
                    //connection = new VssConnection(new Uri(url), new VssCredentials(true));
                    connection = new VssConnection(new Uri(url), new VssClientCredentials(true));

                // Setup process properties       
                var processClient = connection.GetClient<ProcessHttpClient>();
                var prs = await processClient.GetProcessesAsync().ConfigureAwait(false);
                if (prs?.Count > 0)
                {
                    processAdos = _mapper.Map<List<ProcessAdo>>(prs);
                    _cacheProvider.SetCache(cacheKeyProcess, processAdos);
                }
                return processAdos;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                connection?.Dispose();
            }            
        }

        public async Task<List<string>> GetOrganizationNames()
        {
            var orgs = await GetOrganizations();
            return orgs == null ? null : orgs.Select(a => a.Name)?.ToList();
        }

        public async Task<List<OrganizationDto>> GetOrganizations()
        {
            var organization = DefAdoOrganization;
            HttpClient httpClient = null;
            try
            {
                var organizationDtos = _cacheProvider.GetFromCache<List<OrganizationDto>>(cacheKeyOrganization);
                if (organizationDtos?.Count > 0)
                    return organizationDtos;

                var personalaccesstoken = _adoConfig.AdoPersonalAccessToken;
                httpClient = _httpClientFactory.CreateClient(ConstatAdo.ClientNames.DEVOPS_EndPoint);
                //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalaccesstoken))));
                    
                string body = null;
                string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RequestAllOrgBody.json");
                body = File.ReadAllText(jsonFilePath);
                var httpcontent = new StringContent(body, Encoding.UTF8, "application/json");
                var uri = $"https://dev.azure.com/{organization}/_apis/Contribution/HierarchyQuery?api-version=5.0-preview.1";
                var response = await httpClient.PostAsync(uri, httpcontent).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    //var ser = JsonSerializer.Deserialize<OrganizationResponseDto>(content, _options);
                    var ser = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponseDto>(content);
                    var organizations  = ser?.DataProviders?.MsVssFeaturesMyOrganizationsDataProvider?.Organizations;
                    _cacheProvider.SetCache(cacheKeyOrganization, organizations);
                    return organizations;
                }
                else
                    _logger.LogWarning($"{nameof(GetOrganizations)} returned {response.Content}");
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                httpClient?.Dispose();
            }
            return null;

            #region sdk not working for GetOrganizationsAsync
            //VssConnection connection = null;
            //try
            //{                
            //    string url = $"https://dev.azure.com/{organization}";
            //    if (_adoConfig.UsePta)
            //        connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, _adoConfig.AdoPersonalAccessToken));                    
            //        //connection = new VssConnection()                    
            //    else                    
            //        connection = new VssConnection(new Uri(url), new VssClientCredentials(true));

            //    //AccountHttpClient accountHttpClient = connection.GetClient<AccountHttpClient>();                
            //    //ProfileHttpClient profileHttpClient = connection.GetClient<ProfileHttpClient>();         
            //    //var pr = await profileHttpClient

            //    OrganizationHttpClient organizatioClient = connection.GetClient<OrganizationHttpClient>();
            //    //var curOrg = await organizatioClient.GetOrganizationAsync("/Me");
            //    //var col = await organizatioClient.GetCollectionsAsync("Contribution");

            //    List<Organization> orgs = null;
            //    orgs = await organizatioClient.GetOrganizationsAsync(Microsoft.VisualStudio.Services.Organization.OrganizationSearchKind.ByName, organization);
            //    orgs = await organizatioClient.GetOrganizationsAsync(Microsoft.VisualStudio.Services.Organization.OrganizationSearchKind.ById, "7a26cf8d-e2c1-48f5-b2b0-5f933898c6a6");
            //    orgs = await organizatioClient.GetOrganizationsAsync(Microsoft.VisualStudio.Services.Organization.OrganizationSearchKind.ByTenantId, "1ff35017-cbf2-4700-9930-3210afb6182b");
            //    orgs = await organizatioClient.GetOrganizationsAsync(Microsoft.VisualStudio.Services.Organization.OrganizationSearchKind.Unknown, "1ff35017-cbf2-4700-9930-3210afb6182b");
            //    var res = orgs.Select(a => a.Name).ToList();
            //    return res;
            //}
            //catch (Exception ex)
            //{
            //    //throw;
            //}
            //finally
            //{
            //    connection?.Dispose();
            //}
            //return null;
            #endregion
        }

        public async Task<List<string>> GetAdoProcessesName(string organization)
        {
            var prs = await GetAdoProcesses(organization);
            return prs?.Select(a => a.Name).ToList();
        }

        public async Task<ProcessAdo> GetAdoProcesByName(string organization, string name)
        {
            var prs = await GetAdoProcesses(organization);
            return prs?.FirstOrDefault(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase));
        }

    }
}
