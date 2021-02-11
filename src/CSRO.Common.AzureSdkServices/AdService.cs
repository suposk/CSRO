using Azure;
using Azure.Core;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Common.AzureSdkServices
{
    public interface IAdService
    {
        Task<List<object>> TryGetData();
    }

    public class AdService : IAdService
    {
        private readonly ICsroTokenCredentialProvider _csroTokenCredentialProvider;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILogger<AdService> _logger;
        private readonly AzureAd _azureAd;
        private readonly TokenCredential _tokenCredential;

        public AdService(
            ICsroTokenCredentialProvider csroTokenCredentialProvider,
            AuthenticationStateProvider authenticationStateProvider,
            ILogger<AdService> logger,
            IConfiguration configuration
            )
        {
            _csroTokenCredentialProvider = csroTokenCredentialProvider;
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
            _tokenCredential = _csroTokenCredentialProvider.GetCredential();
            _azureAd = configuration.GetSection(nameof(AzureAd)).Get<AzureAd>();            
        }

        public async Task<List<object>> TryGetData()
        {
            try
            {
                var auth = await _authenticationStateProvider.GetAuthenticationStateAsync();
                PrincipalContext context = new PrincipalContext(ContextType.Domain);
                UserPrincipal principal = new UserPrincipal(context);
                if (context != null)
                {
                    principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, auth.User.Identity.Name);
                    var gropus = principal.GetGroups();
                }
                return null;

            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(TryGetData), ex);
            }
            return null;
        }

        public class AdUser
        {
            public DateTime? AccountExpirationDate
            {
                get;
                set;
            }
            public DateTime? AccountLockoutTime
            {
                get;
                set;
            }
            public int BadLogonCount
            {
                get;
                set;
            }
            public string Description
            {
                get;
                set;
            }
            public string DisplayName
            {
                get;
                set;
            }
            public string DistinguishedName
            {
                get;
                set;
            }
            public string Domain
            {
                get;
                set;
            }
            public string EmailAddress
            {
                get;
                set;
            }
            public string EmployeeId
            {
                get;
                set;
            }
            public bool? Enabled
            {
                get;
                set;
            }
            public string GivenName
            {
                get;
                set;
            }
            public Guid? Guid
            {
                get;
                set;
            }
            public string HomeDirectory
            {
                get;
                set;
            }
            public string HomeDrive
            {
                get;
                set;
            }
            public DateTime? LastBadPasswordAttempt
            {
                get;
                set;
            }
            public DateTime? LastLogon
            {
                get;
                set;
            }
            public DateTime? LastPasswordSet
            {
                get;
                set;
            }
            public string MiddleName
            {
                get;
                set;
            }
            public string Name
            {
                get;
                set;
            }
            public bool PasswordNeverExpires
            {
                get;
                set;
            }
            public bool PasswordNotRequired
            {
                get;
                set;
            }
            public string SamAccountName
            {
                get;
                set;
            }
            public string ScriptPath
            {
                get;
                set;
            }
            public SecurityIdentifier Sid
            {
                get;
                set;
            }
            public string Surname
            {
                get;
                set;
            }
            public bool UserCannotChangePassword
            {
                get;
                set;
            }
            public string UserPrincipalName
            {
                get;
                set;
            }
            public string VoiceTelephoneNumber
            {
                get;
                set;
            }
        }
    }
}
