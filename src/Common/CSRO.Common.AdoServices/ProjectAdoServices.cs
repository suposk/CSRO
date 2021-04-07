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
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Graph.Client;
using CSRO.Common.Helpers;

namespace CSRO.Common.AdoServices
{
    public interface IProjectAdoServices
    {
        Task<ProjectAdo> CreateProject(ProjectAdo projectAdoCreate);
        Task<bool> ProjectExistInAdo(string organization, string projectName);       
        Task<List<string>> GetPermissions(string organization, string projectName);
        Task<List<string>> GetProjectNames(string organization);
    }

    public class ProjectAdoServices : IProjectAdoServices
    {
        //private readonly ITokenAcquisition _tokenAcquisition;
        private readonly AdoConfig _adoConfig;
        private readonly IMapper _mapper;
        private readonly IProcessAdoServices _processAdoServices;
        private readonly ILogger<ProjectAdoServices> _logger;        

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

        public async Task<List<string>> GetProjectNames(string organization)
        {
            if (string.IsNullOrWhiteSpace(organization))
                throw new ArgumentException($"'{nameof(organization)}' cannot be null or whitespace.", nameof(organization));

            VssConnection connection = null;
            try
            {
                string url = $"https://dev.azure.com/{organization}";

                if (_adoConfig.UsePta)
                    connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, _adoConfig.AdoPersonalAccessToken));
                else
                    //connection = new VssConnection(new Uri(url), new VssCredentials(true));
                    connection = new VssConnection(new Uri(url), new VssClientCredentials(true));

                // Get a client            
                ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
                var all = await projectClient.GetProjects();
                if (all.IsNullOrEmptyCollection())
                    return null;
                else
                    return all.OrderBy(a => a.Name).Select(a => a.Name).ToList();
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

        public async Task<bool> ProjectExistInAdo(string organization, string projectName)
        {
            if (string.IsNullOrWhiteSpace(organization))            
                throw new ArgumentException($"'{nameof(organization)}' cannot be null or whitespace.", nameof(organization));
            

            if (string.IsNullOrWhiteSpace(projectName))            
                throw new ArgumentException($"'{nameof(projectName)}' cannot be null or whitespace.", nameof(projectName));
            

            VssConnection connection = null;            
            try
            {
                string url = $"https://dev.azure.com/{organization}";

                if (_adoConfig.UsePta)
                    connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, _adoConfig.AdoPersonalAccessToken));
                else
                    //connection = new VssConnection(new Uri(url), new VssCredentials(true));
                    connection = new VssConnection(new Uri(url), new VssClientCredentials(true));

                // Get a client            
                ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
                var all = await projectClient.GetProjects();
                if (all.IsNullOrEmptyCollection())
                    return false;

                var exist = all.Any(a => string.Equals(a.Name, projectName, StringComparison.OrdinalIgnoreCase));
                return exist;
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
                Dictionary<string, string> versionControlProperties = new();
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

                Dictionary<string, string> processProperaties = new();
                processProperaties[TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityTemplateTypeIdAttributeName] =
                    processId.ToString();

                // Construct capabilities dictionary
                Dictionary<string, Dictionary<string, string>> capabilities = new();
                capabilities[TeamProjectCapabilitiesConstants.VersionControlCapabilityName] =
                    versionControlProperties;
                capabilities[TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityName] =
                    processProperaties;

                // Construct object containing properties needed for creating the project
                TeamProject projectCreateParameters = new()
                {
                    Name = projectName,
                    Description = projectDescription + " 'Created via CSRO Web Portal'",
                    Capabilities = capabilities,                     
                    //State = ProjectState.CreatePending //only from UI
                };

                // Get a client            
                ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();                                               
                _logger?.LogDebug("Queuing project creation...");

                // Queue the project creation operation 
                // This returns an operation object that can be used to check the status of the creation
                OperationReference operation = await projectClient.QueueCreateProject(projectCreateParameters).ConfigureAwait(false);

                //ClientSampleHttpLogger.SetSuppressOutput(Context, true);

                // Check the operation status every 5 seconds (for up to 30 seconds)
                Operation completedOperation = await WaitForLongRunningOperation(connection, operation.Id, 2, 30).ConfigureAwait(false);

                // Check if the operation succeeded (the project was created) or failed
                if (completedOperation.Status == OperationStatus.Succeeded)
                {
                    // Get the full details about the newly created project
                    var project = await projectClient.GetProject(
                                projectCreateParameters.Name,
                                includeCapabilities: true,
                                includeHistory: true);
                                        
                    _logger?.LogDebug("Project created (ID: {0})", project.Id);
                                        
                    result = _mapper.Map<ProjectAdo>(project);
                    result.Organization = organization;
                    result.ProcessName = processName;

                    //original props
                    result.Id = projectAdoCreate.Id;
                    result.RowVersion = projectAdoCreate.RowVersion;
                    result.CreatedAt = projectAdoCreate.CreatedAt;
                    result.CreatedBy = projectAdoCreate.CreatedBy;
                    result.ModifiedAt = projectAdoCreate.ModifiedAt;
                    result.ModifiedBy = projectAdoCreate.ModifiedBy;
                    result.IsDeleted = projectAdoCreate.IsDeleted;

                    //projectAdoCreate.Description = project.Description;
                    //projectAdoCreate.AdoId = project.Id;                    
                    //projectAdoCreate.Url = project.Url;
                    //projectAdoCreate.State = project.State;
                    //projectAdoCreate.Visibility = project.Visibility;
                    //return projectAdoCreate;
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

        public async Task<List<string>> GetPermissions(string organization, string projectName)
        {
            if (string.IsNullOrWhiteSpace(organization))
                throw new ArgumentException($"'{nameof(organization)}' cannot be null or whitespace.", nameof(organization));

            if (string.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException($"'{nameof(projectName)}' cannot be null or whitespace.", nameof(projectName));

            VssConnection connection = null;
            GraphHttpClient graphHttpClient = null;
            TeamHttpClient teamClient = null;

            Guid GitSecurityNamespace = Guid.Parse("2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87");
            try
            {
                string url = $"https://dev.azure.com/{organization}";

                if (_adoConfig.UsePta)
                    connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, _adoConfig.AdoPersonalAccessToken));
                else
                    //connection = new VssConnection(new Uri(url), new VssCredentials(true));
                    connection = new VssConnection(new Uri(url), new VssClientCredentials(true));

                projectName = "First-Ado";

                teamClient = connection.GetClient<TeamHttpClient>();
                var allteams = await teamClient.GetTeamsAsync(projectName, null, null, null, true);

                var defTeamGroupName = $"{projectName} Team";
                var defTeam = allteams?.FirstOrDefault(a => a.Name.Contains(defTeamGroupName));
                if (defTeam != null)
                {
                    var members = await teamClient.GetTeamMembersWithExtendedPropertiesAsync(projectName, defTeam.Id.ToString());                    
                    graphHttpClient = connection.GetClient<GraphHttpClient>();
                    List<SubjectDescriptor> groupSubjectDescriptors = new();
                    groupSubjectDescriptors.Add(SubjectDescriptor.FromString(defTeam.Identity.Descriptor.Identifier));
                    var contextCreate = new GraphUserPrincipalNameCreationContext {  PrincipalName = "dev@jansupolikhotmail.onmicrosoft.com"};
                    var added = await graphHttpClient.CreateUserAsync(contextCreate, groupSubjectDescriptors);

                    var membersAfter = await teamClient.GetTeamMembersWithExtendedPropertiesAsync(projectName, defTeam.Id.ToString());                    
                }

                // Get a client            
                SecurityHttpClient httpClient = connection.GetClient<SecurityHttpClient>();
                IEnumerable<SecurityNamespaceDescription> namespaces = await httpClient.QuerySecurityNamespacesAsync(GitSecurityNamespace);                
                SecurityNamespaceDescription gitNamespace = namespaces.FirstOrDefault();

                Dictionary<int, string> permission = new Dictionary<int, string>();
                foreach (ActionDefinition actionDef in gitNamespace.Actions)
                {
                    permission[actionDef.Bit] = actionDef.DisplayName;
                }
                return permission.Values?.ToList();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exception during create project: ", ex.Message);
                throw;
            }
            finally
            {
                connection?.Dispose();
                graphHttpClient?.Dispose();
                teamClient?.Dispose();
            }
        }
    }
}
