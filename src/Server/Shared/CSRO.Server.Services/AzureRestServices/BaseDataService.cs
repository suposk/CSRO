using CSRO.Server.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Server.Services.AzureRestServices
{
    public class BaseDataService
    {
        public HttpClient HttpClientBase { get; private set; }
        protected string ClientName { get; set; }
        protected string ApiPart { get; set; }
        protected string Scope { get; set; }

        public JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public readonly IHttpClientFactory HttpClientFactory;
        public readonly ITokenAcquisition _tokenAcquisition;
        public readonly IApiIdentity ApiIdentity;
        public readonly IConfiguration Configuration;

        public BaseDataService(
            IHttpClientFactory httpClientFactory,
            ITokenAcquisition tokenAcquisition,
            IApiIdentity apiIdentity,
            IConfiguration configuration)
        {
            HttpClientFactory = httpClientFactory;
            _tokenAcquisition = tokenAcquisition;

            ApiIdentity = apiIdentity;                        
            Configuration = configuration;
        }

        /// <summary>
        /// verify all params and create httpclient        
        /// </summary>
        public virtual void Init()
        {
            if (string.IsNullOrWhiteSpace(ClientName))
                throw new Exception($"{ClientName} must be set in before calling method.");

            if (string.IsNullOrWhiteSpace(ApiPart))
                throw new Exception($"{ApiPart} must be set in before calling method.");

            if (string.IsNullOrWhiteSpace(Scope))
                throw new Exception($"{Scope} must be set in before calling method.");

            if (HttpClientBase == null)
                HttpClientBase = HttpClientFactory.CreateClient(ClientName);

            //todo read from config
        }

        public virtual void HandleException(Exception ex)
        {
            Console.WriteLine($"{nameof(HandleException)}: {ex}");
        }

        public virtual async Task AddAuthHeaderAsync()
        {
            if (!ApiIdentity.IsAuthenticated())
                throw new AuthenticationException("CSRO: User or SPN is not Authenticated yet. Can not call downstream API.");

            //user_impersonation
            //var apiToken = await AuthCsroService.GetAccessTokenForUserAsync(Scope);
            var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new List<string> { Scope });
            HttpClientBase.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
        }
    }
}
