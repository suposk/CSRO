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

namespace CSRO.Common.AdoServices
{
    public interface IProjectAdoServices
    {
        Task<ProjectAdo> CreateProject(ProjectAdo projectAdoCreate);
    }

    public class ProjectAdoServices : IProjectAdoServices
    {
        //private readonly ITokenAcquisition _tokenAcquisition;
        private readonly AdoConfig _adoConfig;
        private readonly IMapper _mapper;
        private readonly IProcessAdoServices _processAdoServices;
        private readonly ILogger<ProjectAdoServices> _logger;
        internal const string azureDevOpsOrganizationUrl = "https://dev.azure.com/organization"; //change to the URL of your Azure DevOps account; NOTE: This must use HTTPS

        public ProjectAdoServices(
            IConfiguration configuration, 
            IMapper mapper,
            IProcessAdoServices processAdoServices,
            ILogger<ProjectAdoServices> logger = null
            )
        {
            _mapper = mapper;
            _processAdoServices = processAdoServices;
            _logger = logger;
            _adoConfig = configuration.GetSection(nameof(AdoConfig)).Get<AdoConfig>();            
        }

        public async Task<ProjectAdo> CreateProject(ProjectAdo projectAdoCreate)
        {
            VssConnection connection = null;            
            ProjectAdo result = null;
            try
            {
                string projectName = projectAdoCreate.Name;
                string projectDescription = projectAdoCreate.Description;
                string processName = projectAdoCreate.ProcessName ?? "Agile";
                var organization = projectAdoCreate.Organization;                 
                string url = $"https://dev.azure.com/{organization}";

                // Setup version control properties
                Dictionary<string, string> versionControlProperties = new Dictionary<string, string>();
                versionControlProperties[TeamProjectCapabilitiesConstants.VersionControlCapabilityAttributeName] =
                    SourceControlTypes.Git.ToString();

                if (_adoConfig.UsePta)                
                    connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, _adoConfig.AdoPersonalAccessToken));                
                else                
                    //connection = new VssConnection(new Uri(url), new VssCredentials(true));
                    connection = new VssConnection(new Uri(url), new VssClientCredentials(true));
                
                //var scope = "vso.project_manage";
                //var token = await _tokenAcquisition.GetAccessTokenForAppAsync(scope);
                //var accessTokenCredential = new VssOAuthAccessTokenCredential(token);
                //connection = new VssConnection(new Uri(url), accessTokenCredential);

                // Setup process properties                                 
                Guid? processId = null;
                var process = await _processAdoServices.GetAdoProcesByName(organization, processName);
                processId = process?.Id;

                Dictionary<string, string> processProperaties = new Dictionary<string, string>();
                processProperaties[TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityTemplateTypeIdAttributeName] =
                    processId.ToString();

                // Construct capabilities dictionary
                Dictionary<string, Dictionary<string, string>> capabilities = new Dictionary<string, Dictionary<string, string>>();

                capabilities[TeamProjectCapabilitiesConstants.VersionControlCapabilityName] =
                    versionControlProperties;
                capabilities[TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityName] =
                    processProperaties;

                // Construct object containing properties needed for creating the project
                TeamProject projectCreateParameters = new TeamProject()
                {
                    Name = projectName,
                    Description = projectDescription,
                    Capabilities = capabilities
                };

                // Get a client            
                ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();                                               
                _logger?.LogDebug("Queuing project creation...");

                // Queue the project creation operation 
                // This returns an operation object that can be used to check the status of the creation
                OperationReference operation = await projectClient.QueueCreateProject(projectCreateParameters).ConfigureAwait(false);

                //ClientSampleHttpLogger.SetSuppressOutput(Context, true);

                // Check the operation status every 5 seconds (for up to 30 seconds)
                Operation completedOperation = await WaitForLongRunningOperation(connection, operation.Id, 5, 30).ConfigureAwait(false);

                // Check if the operation succeeded (the project was created) or failed
                if (completedOperation.Status == OperationStatus.Succeeded)
                {
                    // Get the full details about the newly created project
                    var project = await projectClient.GetProject(
                                projectCreateParameters.Name,
                                includeCapabilities: true,
                                includeHistory: true);
                                        
                    _logger?.LogDebug("Project created (ID: {0})", project.Id);

                    // Save the newly created project (other sample methods will use it)
                    //Context.SetValue<TeamProject>("$newProject", project);
                    result = _mapper.Map<ProjectAdo>(project);
                    result.Organization = projectAdoCreate.Organization;
                    result.ProcessName = projectName;
                }
                else                                    
                    _logger?.LogError("Project creation operation failed: " + completedOperation.ResultMessage);                
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
            return result;            
        }

        private async Task<Operation> WaitForLongRunningOperation(VssConnection connection, Guid operationId, int interavalInSec = 5, int maxTimeInSeconds = 60, CancellationToken cancellationToken = default(CancellationToken))
        {
            OperationsHttpClient operationsClient = connection.GetClient<OperationsHttpClient>();
            DateTime expiration = DateTime.Now.AddSeconds(maxTimeInSeconds);
            int checkCount = 0;

            while (true)
            {                
                _logger?.LogDebug(" Checking status ({0})... ", (checkCount++));
                Operation operation = await operationsClient.GetOperation(operationId, cancellationToken);

                if (!operation.Completed)
                {                    
                    _logger?.LogDebug("   Pausing {0} seconds", interavalInSec);
                    await Task.Delay(interavalInSec * 1000);

                    if (DateTime.Now > expiration)                    
                       throw new Exception(String.Format("Operation did not complete in {0} seconds.", maxTimeInSeconds));                    
                }
                else                
                    return operation;                
            }
        }
    }
}
