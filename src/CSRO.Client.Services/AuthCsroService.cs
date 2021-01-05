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
                var t = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });
                return t;
            }
            else
            {
                //todo log in
                _navigationManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
                return null;
            }
        }

        public Task<string> GetAccessTokenForUserAsync(List<string> scopes)
        {
            if (RunWithoutAuth)
                return Task.FromResult<string>(null);

            var t = _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            return t;
        }
    }
}
