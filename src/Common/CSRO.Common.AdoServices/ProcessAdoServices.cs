using CSRO.Common.AdoServices.Models;
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

namespace CSRO.Common.AdoServices
{
    public interface IProcessAdoServices
    {
        Task<ProcessAdo> GetAdoProcesByName(string organization, string name);
        Task<List<ProcessAdo>> GetAdoProcesses(string organization);
        Task<List<string>> GetAdoProcessesName(string organization);
    }

    public class ProcessAdoServices : IProcessAdoServices
    {
        private readonly AdoConfig _adoConfig;
        private readonly IMapper _mapper;
        private readonly ICacheProvider _cacheProvider;
        private readonly ILogger<ProcessAdoServices> _logger;
        //private List<ProcessAdo> _processAdos = null;
        const string cacheKey = nameof(ProcessAdo);

        public ProcessAdoServices(
            IConfiguration configuration, 
            IMapper mapper,
            ICacheProvider cacheProvider,
            ILogger<ProcessAdoServices> logger = null)
        {
            _mapper = mapper;
            _cacheProvider = cacheProvider;
            _logger = logger;
            _adoConfig = configuration.GetSection(nameof(AdoConfig)).Get<AdoConfig>();
        }

        public async Task<List<ProcessAdo>> GetAdoProcesses(string organization)
        {                                  
            organization ??= "jansupolikAdo";
            VssConnection connection = null;
            try
            {
                //var or = await GetOrganizations();

                var processAdos = _cacheProvider.GetFromCache<List<ProcessAdo>>(cacheKey);
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
                    _cacheProvider.SetCache(cacheKey, processAdos);
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

        public async Task<List<string>> GetOrganizations()
        {            
            VssConnection connection = null;
            try
            {
                var organization = "jansupolikAdo";
                string url = $"https://dev.azure.com/{organization}";
                if (_adoConfig.UsePta)
                    connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, _adoConfig.AdoPersonalAccessToken));
                else                    
                    connection = new VssConnection(new Uri(url), new VssClientCredentials(true));

                OrganizationHttpClient organizatioClient = connection.GetClient<OrganizationHttpClient>();
                var orgs = await organizatioClient.GetOrganizationsAsync(Microsoft.VisualStudio.Services.Organization.OrganizationSearchKind.ByName, organization);
                return orgs.Select(a => a.Name).ToList();
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
