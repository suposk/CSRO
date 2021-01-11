using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public class AuthCsroService : IAuthCsroService
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly NavigationManager _navigationManager;
        private readonly IConfiguration _configuration;        

        private bool RunWithoutAuth => _configuration.GetValue<bool>("RunWithoutAuth");

        public AuthCsroService(ITokenAcquisition tokenAcquisition, AuthenticationStateProvider authenticationStateProvider,
            NavigationManager NavigationManager, IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _authenticationStateProvider = authenticationStateProvider;
            _navigationManager = NavigationManager;
            _configuration = configuration;
        }

        public async Task<string> GetAccessTokenForUserAsync(string scope)
        {
            if (RunWithoutAuth)
                return null;

            var auth = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (auth != null && auth.User.Identity.IsAuthenticated)
            {
                try
                {
                    var t = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });
                    return t;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException?.Message != null && ex.InnerException.Message.Contains("No account or login hint was passed to the AcquireTokenSilent call"))
                    { 
                        SignIn();
                        return null;
                    }
                    throw;
                }
            }
            else
            {
                SignIn();
                return null;
            }
        }

        public async Task<string> GetAccessTokenForUserAsync(List<string> scopes)
        {
            if (RunWithoutAuth)
                return null;

            var auth = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (auth != null && auth.User.Identity.IsAuthenticated)
            {
                try
                {
                    var t = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                    return t;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException?.Message != null && ex.InnerException.Message.Contains("No account or login hint was passed to the AcquireTokenSilent call"))
                    {
                        SignIn();
                        return null;
                    }
                    throw;
                }
            }
            else
            {
                SignIn();
                return null;
            }
        }

        private void SignIn()
        {
            _navigationManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
        }
    }
}
