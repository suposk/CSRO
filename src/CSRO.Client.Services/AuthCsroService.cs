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
        private readonly IConfiguration _configuration;

        private bool RunWithoutAuth => _configuration.GetValue<bool>("RunWithoutAuth");

        public AuthCsroService(ITokenAcquisition tokenAcquisition, IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
        }

        public Task<string> GetAccessTokenForUserAsync(string scope)
        {
            if (RunWithoutAuth)
                return Task.FromResult<string>(null);

            var t = _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });            
            return t;
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
