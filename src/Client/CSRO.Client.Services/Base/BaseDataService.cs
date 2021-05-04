using AutoMapper;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

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

        public async Task<TModel> RestGetById<TModel, TDto>(string id = null, string route = null, CancellationToken cancelToken = default) where TModel : class
        {
            try
            {
                await AddAuthHeaderAsync();
                var url = string.IsNullOrWhiteSpace(route) ? $"{ApiPart}{id}" : $"{ApiPart}{route}/{id}";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<TDto>(stream, _options, cancelToken);
                    if (ser == null)
                        return null;

                    var result = Mapper.Map<TModel>(ser);
                    return result;
                }
                else
                    throw new Exception(GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public async Task<List<TModel>> RestGetListById<TModel, TDto>(string id = null, string route = null, CancellationToken cancelToken = default) where TModel : class
        {
            try
            {
                await AddAuthHeaderAsync();                
                var url = string.IsNullOrWhiteSpace(route) ? $"{ApiPart}{id}" : $"{ApiPart}{route}/{id}";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<List<TDto>>(stream, _options, cancelToken);
                    if (ser.IsNullOrEmptyCollection())
                        return new List<TModel>();

                    var result = Mapper.Map<List<TModel>>(ser);
                    return result;
                }
                else
                    throw new Exception(GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public async Task<TModel> RestSend<TModel, TDto, TParam>(HttpMethod httpMethod, TParam parameter, string route, CancellationToken cancelToken = default) where TModel : class
        {
            if (string.IsNullOrWhiteSpace(route))            
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace.", nameof(route));

            if (parameter == null)
                throw new ArgumentException($"'{nameof(parameter)}' cannot be null", nameof(parameter));

            try
            {
                await AddAuthHeaderAsync();
                var url = $"{ApiPart}{route}";                
                var httpcontent = new StringContent(JsonSerializer.Serialize(parameter, _options), Encoding.UTF8, "application/json");
                HttpRequestMessage requestMessage = new HttpRequestMessage { Method = httpMethod, Content = httpcontent, RequestUri = new Uri(HttpClientBase.BaseAddress + url) };
                var apiData = await HttpClientBase.SendAsync(requestMessage, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<TDto>(stream, _options, cancelToken);
                    if (ser == null)
                        return null;

                    var result = Mapper.Map<TModel>(ser);
                    return result;
                }
                else
                    throw new Exception(GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public async Task<TModel> RestAdd<TModel, TDto>(TModel model, string route = null, CancellationToken cancelToken = default) where TModel : class
        {
            if (model == null)
                throw new ArgumentException($"'{nameof(model)}' cannot be null", nameof(model));

            try
            {
                await AddAuthHeaderAsync();
                var url = string.IsNullOrWhiteSpace(route) ? $"{ApiPart}" : $"{ApiPart}{route}";
                var updateDto = Mapper.Map<TDto>(model);
                var httpcontent = new StringContent(JsonSerializer.Serialize(updateDto, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<TDto>(stream, _options, cancelToken);
                    if (ser == null)
                        return null;

                    var result = Mapper.Map<TModel>(ser);
                    return result;
                }
                else
                    throw new Exception(GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public async Task<bool> RestUpdate<TModel, TDto>(TModel model, CancellationToken cancelToken = default) where TModel : class
        {
            if (model == null)
                throw new ArgumentException($"'{nameof(model)}' cannot be null", nameof(model));

            try
            {
                await AddAuthHeaderAsync();
                var url = $"{ApiPart}";
                var updateDto = Mapper.Map<TDto>(model);
                var httpcontent = new StringContent(JsonSerializer.Serialize(updateDto, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PutAsync(url, httpcontent, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                    return true;
                else
                    throw new Exception(GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public async Task<bool> RestDeleteById(int id)
        {
            try
            {
                await AddAuthHeaderAsync();
                var url = $"{ApiPart}{id}";
                var apiData = await HttpClientBase.DeleteAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                    return true;
                else
                    throw new Exception(GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }            
        }


        public string GetErrorText(HttpResponseMessage httpResponse)
        {
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var message = httpResponse.Content.ReadAsStringAsync().Result;
                return $"BadRequest, {(string.IsNullOrWhiteSpace(message) ? "Wrong Input" : message)}";
            }
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return "Unauthorized, you are not autorized.";
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
                return "Forbidden, you don't have permision to perform operation.";
            else
                //return $"{httpResponse.ReasonPhrase} {httpResponse.Content.ReadAsStringAsync().Result}";
                return $"{httpResponse.Content.ReadAsStringAsync().Result}";
        }

        /// <summary>
        /// Not doing anything now.
        /// </summary>
        /// <param name="ex"></param>
        public virtual void HandleException(Exception ex)
        {
            //Console.WriteLine($"{nameof(HandleException)}: {ex}");
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
