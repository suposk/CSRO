﻿using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public class BaseDataService
    {
        public HttpClient HttpClientBase { get; private set; }
        protected string ClientName { get; set; }
        protected string ApiPart { get; set; }
        protected string Scope { get; set; }

        //not valid
        //protected List<string> Scopes { get; set; } = new List<string> { "email", "offline_access", "openid", "profile", "https://graph.microsoft.com/.default" };
        //no email from ad        
        protected List<string> Scopes { get; set; } = new List<string> { "email", "offline_access", "openid", "profile", };
        //protected List<string> Scopes { get; set; } = new List<string> { "email", "offline_access", "openid", "profile", "User.Read" , "Directory.AccessAsUser.All" };                

        public JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public readonly IHttpClientFactory HttpClientFactory;
        public readonly IAuthCsroService AuthCsroService;
        public readonly IMapper Mapper;
        public readonly IConfiguration Configuration;

        public BaseDataService(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper, 
            IConfiguration configuration)
        {
            HttpClientFactory = httpClientFactory;
            AuthCsroService = authCsroService;
            Mapper = mapper;
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

            Scopes.Add(Scope);

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
            //user_impersonation
            //var apiToken = await AuthCsroService.GetAccessTokenForUserAsync(Scope);
            var apiToken = await AuthCsroService.GetAccessTokenForUserAsync(Scopes);
            HttpClientBase.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
        }
    }
}
