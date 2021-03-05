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
        private readonly ILogger<ProcessAdoServices> _logger;
        private List<ProcessAdo> _processAdos = null;

        public ProcessAdoServices(IConfiguration configuration, IMapper mapper, ILogger<ProcessAdoServices> logger = null)
        {
            _mapper = mapper;
            _logger = logger;
            _adoConfig = configuration.GetSection(nameof(AdoConfig)).Get<AdoConfig>();
        }

        public async Task<List<ProcessAdo>> GetAdoProcesses(string organization)
        {
            if (_processAdos != null)
                return _processAdos;

            VssConnection connection = null;
            try
            {
                string url = $"https://dev.azure.com/{organization}";
                if (_adoConfig.UsePta)
                    connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, _adoConfig.AdoPersonalAccessToken));
                else
                    //connection = new VssConnection(new Uri(url), new VssCredentials(true));
                    connection = new VssConnection(new Uri(url), new VssClientCredentials(true));

                // Setup process properties       
                ProcessHttpClient processClient = connection.GetClient<ProcessHttpClient>();
                var prs = await processClient.GetProcessesAsync().ConfigureAwait(false);
                if (prs?.Count > 0)
                    _processAdos = _mapper.Map<List<ProcessAdo>>(prs);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exception during create project: ", ex.Message);
                throw;
            }
            finally
            {
                connection?.Dispose();
            }
            return _processAdos;
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
