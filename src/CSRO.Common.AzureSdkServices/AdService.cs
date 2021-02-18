using Azure;
using Azure.Core;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Resources;
using CSRO.Common.AzureSdkServices.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSRO.Common.AzureSdkServices
{
    public interface IAdService
    {
        Task<AdUserSdk> GetCurrentAdUserInfo(bool includeGroups = false);
        bool IsAdminAccount(string accountName);
    }

    public class AdService : IAdService
    {
        private readonly ILogger<AdService> _logger;
        private readonly IHttpContextAccessor _context;
        //private readonly AzureAd _azureAd;

        public AdService(
            ILogger<AdService> logger,
            IHttpContextAccessor context,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _context = context;
            //_azureAd = configuration.GetSection(nameof(AzureAd)).Get<AzureAd>();            
        }

        public Task<AdUserSdk> GetCurrentAdUserInfo(bool includeGroups = false)
        {
            return Task.Run(() => getUserInfo(includeGroups));                      

            AdUserSdk getUserInfo(bool includeGroups)
            {
                try
                {
                    var claims = _context.HttpContext?.User?.Claims?.ToList();
                    if (claims == null)
                        return null;

                    var context = new PrincipalContext(ContextType.Domain);
                    var principal = new UserPrincipal(context);
                    if (context != null)
                    {
                        string upn = null;
                        upn = claims.FirstOrDefault(a => a.Type == System.Security.Claims.ClaimTypes.Upn)?.Value;
                        upn ??= claims.FirstOrDefault(a => a.Type == "preferred_username")?.Value;

                        if (string.IsNullOrWhiteSpace(upn))
                        {
                            _logger.LogWarning(nameof(GetCurrentAdUserInfo), "upn/preferred_username claim is null");
                            return null;
                        }

                        principal = UserPrincipal.FindByIdentity(context, IdentityType.UserPrincipalName, upn);
                        if (IsAdminAccount(upn) && principal == null)
                        {
                            //for admin account only GPN in name admin_4325....
                            principal = UserPrincipal.FindByIdentity(context, IdentityType.UserPrincipalName, upn);
                            if (principal == null)
                            {
                                //Supolik, Jan (Admin)
                                var name = claims.FirstOrDefault(a => a.Type == "name")?.Value;
                                if (string.IsNullOrWhiteSpace(name))
                                {
                                    _logger.LogWarning(nameof(GetCurrentAdUserInfo), "name claim is null");
                                    return null;
                                }

                                //consrtcut Supolik Jan 4235xxx
                                var coverted = convertName(name);
                                principal = UserPrincipal.FindByIdentity(context, IdentityType.Name, coverted);
                            }
                        }
                        if (principal != null)
                        {
                            AdUserSdk adUser = new AdUserSdk 
                            {
                                EmailAddress = principal.EmailAddress, 
                                SamAccountName = principal.SamAccountName, 
                                DisplayName = principal.DisplayName
                            };
                            if (includeGroups)
                            {
                                var groups = principal.GetGroups()?.ToList();
                                groups?.ForEach(a => adUser.AdGroups.Add(new AdUserSdk 
                                {
                                    Name = a.Name, 
                                    DisplayName = a.DisplayName,
                                }));
                            }
                            return adUser;
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(nameof(GetCurrentAdUserInfo), ex);
                }
                return null;
            }
        }

        string convertName(string name)
        {
            //admin_4325....
            var f = new StringBuilder();
            var l = new StringBuilder();
            int noneChar = 0;
            foreach (var letter in name)
            {
                if (noneChar >= 2)
                    break;

                if (Char.IsLetter(letter))
                {
                    switch (noneChar)
                    {
                        case 0:
                            l.Append(letter); break;
                        case 1:
                            f.Append(letter);break;
                    }
                }
                else
                    noneChar++;

            }
            string gpn = Regex.Replace(name, @"\d", "");
            //Supolik Jan 4235xxx
            return $"{l} {f} {gpn}";
        }

        public bool IsAdminAccount(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName))            
                throw new ArgumentException($"{nameof(IsAdminAccount)} '{nameof(accountName)}' cannot be null or empty", nameof(accountName));            

            return accountName.ToLower().Contains("admin");
        }
    }
    
}
