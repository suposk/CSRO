﻿using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{

    public interface IUserDataService : IBaseDataService<User>
    {
        Task<User> GetUserByUserName(string userName);
    }

    public class UserDataService : BaseDataService, IUserDataService
    {

        public UserDataService(
            IHttpClientFactory httpClientFactory,
            IAuthCsroService authCsroService,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/user/";
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Auth_Api);
            ClientName = ConstatCsro.EndPoints.ApiEndpointAuth;

            base.Init();
        }

        public async Task<User> AddItemAsync(User item)
        {
            try
            {
                await base.AddAuthHeaderAsync();                                
                
                var url = $"{ApiPart}";
                var httpcontent = new StringContent(JsonSerializer.Serialize(item, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);
                //HttpResponseMessage
                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<UserDto>(stream, _options);
                    var result = Mapper.Map<User>(ser);
                    return result;
                }
                else
                    throw new Exception(GetErrorText(apiData));

            }
            catch (Exception ex)
            {
                base.HandleException(ex);
                throw;
            }
        }


        public async Task<bool> UpdateItemAsync(User item)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                //var add = Mapper.Map<ProjectAdo>(item);
                var httpcontent = new StringContent(JsonSerializer.Serialize(item, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PutAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                    return true;
                else
                    throw new Exception(base.GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
                throw;
            }
            //return false;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{id}";
                var apiData = await HttpClientBase.DeleteAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                    return true;
                else
                    throw new Exception(base.GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
        }

        public Task<User> GetItemByIdAsync(int id)
        {
            return base.RestGetById<User, UserDto>(id.ToString());
        }

        public Task<List<User>> GetItemsAsync()
        {
            return base.RestGetListById<User, UserDto>();
        }

        public Task<User> GetUserByUserName(string userName)
        {
            return base.RestGetById<User, UserDto>(userName);
        }

    }

}
